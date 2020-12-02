using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomSpline : MonoBehaviour
{
    public static int n_Points = 20;    
    public float forwardAngle { get; private set; }
    public float approximateSplineLength { get; private set; }
    private List<Vector3> points = new List<Vector3>();
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(p0, 0.08f);
        Gizmos.DrawSphere(p1, 0.08f);
        Gizmos.DrawSphere(p2, 0.08f);
        Gizmos.DrawSphere(p3, 0.08f);
        Gizmos.DrawSphere(p4, 0.08f);
        Gizmos.DrawSphere(p5, 0.08f);
    }
    Vector3 p0, p1, p2, p3, p4, p5;    
    public Vector3[] UpdateSpline(Transform start, Transform end)
    {           
        Vector3 target = end.position - start.position;
        forwardAngle = Mathf.Acos(Vector3.Dot(target.normalized, start.forward));
        float forwardAngle_Modified = forwardAngle;// * (1f - Mathf.Log(forwardAngle / Mathf.PI + 1f, 2f) * 2f / 3f);

        Vector3 startForward = Vector3.Lerp(target.normalized, start.forward, forwardAngle_Modified / forwardAngle);

        float dist = Mathf.Sqrt(Mathf.Pow((target / 2f).magnitude, 2f) + Mathf.Pow(Mathf.Tan(forwardAngle_Modified) * (target / 2f).magnitude, 2f));
        Vector3 referencePos = start.position + startForward * dist;
        Vector3 endForward = (referencePos - end.position).normalized;

        float controlPointDistance_Z = (target / 3f).magnitude * (1f - Mathf.Log(forwardAngle_Modified / (Mathf.PI*2f) + 1f, 2f));
        float controlPointDistance = Mathf.Sqrt(Mathf.Pow(Mathf.Tan(forwardAngle_Modified) * controlPointDistance_Z, 2f) + Mathf.Pow(controlPointDistance_Z, 2f));
         p0 = start.position - startForward * controlPointDistance;
         p1 = start.position;
         p2 = start.position + startForward * controlPointDistance;
         p3 = end.position + endForward * controlPointDistance;
         p4 = end.position;
         p5 = end.position - endForward * controlPointDistance;

        approximateSplineLength = (p1 - p0).magnitude + (p2 - p1).magnitude + (p3 - p2).magnitude + (p4 - p3).magnitude + (p5 - p4).magnitude;

        points.Clear();
        points.AddRange(GetSplineBetweenPoints(p0, p1, p2, p3));
        points.AddRange(GetSplineBetweenPoints(p1, p2, p3, p4));
        points.AddRange(GetSplineBetweenPoints(p2, p3, p4, p5));

        return points.ToArray();
    }
    
    public static Vector3[] GetSplineBetweenPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        Vector3[] points = new Vector3[n_Points];
        float resolution = 1f / (n_Points - 1);
        for (int i = 0; i < n_Points; i++)
        {
            float x = i * resolution;
            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            points[i] = 0.5f * (a + (b * x) + (c * x * x) + (d * x * x * x));
        }

        return points;
    }
}
