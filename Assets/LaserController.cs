using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private CatmullRomSpline laserSpline;
    public LayerMask laserMask;
    public Transform start;
    public Transform end;
    public bool ShootLaser { get { return shootLaser; } set { shootLaser = value; laserRenderer.enabled = shootLaser; if (shootLaser) { UpdateLaser(); ShrinkLaser(); } } }
    private bool shootLaser;
    public bool shrink = false;
    // shrinking the laser
    public float minShrinkAngle = 15f;
    public float maxShrinkAngle = 60f;
    public float minShrinkDistance = 5f;
    public float maxShrinkDistance = 8f;

    // smooth edges of the line
    public float smoothLength = 0.1f; // 0.1m from the start and from the end towards the middle will be faded out gradually
    public float fadeOutLength = 0.1f;
    private GradientAlphaKey[] startAlphaKeys;
    private void Start()
    {
        Gradient gradient = laserRenderer.colorGradient;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[4];
        alphaKeys[0] = new GradientAlphaKey(0f, fadeOutLength);
        alphaKeys[1] = new GradientAlphaKey(1f, fadeOutLength + smoothLength);
        alphaKeys[2] = new GradientAlphaKey(1f, 1f - fadeOutLength - smoothLength);
        alphaKeys[3] = new GradientAlphaKey(0f, 1f - fadeOutLength);

        gradient.alphaKeys = alphaKeys;
        laserRenderer.colorGradient = gradient;

        startAlphaKeys = laserRenderer.colorGradient.alphaKeys;
    }
    private void Update()
    {
        if (shootLaser)
        {
            UpdateLaser();
            SmoothLaser();
            if (shrink) ShrinkLaser();
        }
    }
    private void UpdateLaser()
    {
        Vector3[] laserPoints = laserSpline.UpdateSpline(start, end);
        laserRenderer.positionCount = laserPoints.Length;
        laserRenderer.SetPositions(laserPoints);
    }
    private void SmoothLaser()
    {
        Gradient gradient = laserRenderer.colorGradient;
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
        alphaKeys[0].time = fadeOutLength / laserSpline.approximateSplineLength;
        alphaKeys[1].time = (fadeOutLength + smoothLength) / laserSpline.approximateSplineLength;
        alphaKeys[2].time = 1f - (fadeOutLength + smoothLength) / laserSpline.approximateSplineLength;
        alphaKeys[3].time = 1f - fadeOutLength / laserSpline.approximateSplineLength;
        gradient.alphaKeys = alphaKeys;
        laserRenderer.colorGradient = gradient;
    }
    private void ShrinkLaser()
    {
        float angle = laserSpline.forwardAngle * Mathf.Rad2Deg;
        float distance = (end.position - start.position).magnitude;
        if (angle > maxShrinkAngle) { ShootLaser = false; return; }

        float angleMultiplier = Mathf.Clamp01(1f - (angle - minShrinkAngle) / (maxShrinkAngle - minShrinkAngle));
        float distanceMultiplier = Mathf.Clamp01(1f - (distance - minShrinkDistance) / (maxShrinkDistance - minShrinkDistance));
        float multiplier = angleMultiplier * distanceMultiplier;
        laserRenderer.widthMultiplier = Mathf.Lerp(0.3f, 1f, multiplier);

        Gradient gradient = laserRenderer.colorGradient;
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].alpha = Mathf.Lerp(0.3f, startAlphaKeys[i].alpha, multiplier);
        }
        gradient.alphaKeys = alphaKeys;
        laserRenderer.colorGradient = gradient;
    }
}
