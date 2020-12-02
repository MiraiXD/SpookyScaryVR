using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
public class StandToSit : GoapAction
{
    private Sittable sittable;
    private Mortal mortal;
    private Animator animator;
    private FullBodyBipedIK ik;
    private int standToSitHash = Animator.StringToHash("StandToSit");
    private int sitToStandHash = Animator.StringToHash("SitToStand");
    private bool finished;
    public override bool CanPerform(GameObject agent)
    {
        return sittable.isFree;
    }

    public override void Init(GameObject agent)
    {
        sittable = GetComponent<Sittable>();
        targetPosition = sittable.interactionTransform.position;
        targetRotation = sittable.interactionTransform.rotation;
        hasTargetRotation = true;

        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == false; });
        AddActionEffect("isSitting", (isSitting) => { return true; });
        AddActionEffect("position", (position) => { return sittable.interactionTransform.position; });
    }

    public override bool IsFinished(GameObject agent)
    {
        return finished;
    }
    public override bool IsInRange(GameObject agent)
    {
        return (agent.transform.position - targetPosition).magnitude < 0.1f && agent.transform.rotation == targetRotation;
    }
    private bool isSitting;
    public override bool Perform(GameObject agent)
    {
        if (!isSitting)
        {
            isSitting = true;
            animator.Play(standToSitHash);
            ik.solver.bodyEffector.target.position = sittable.bodyEffector.position;
            ik.solver.leftHandEffector.target.position = sittable.leftHandEffector.position;
            ik.solver.leftShoulderEffector.target.position = sittable.leftShoulderEffector.position;
            ik.solver.rightHandEffector.target.position = sittable.rightHandEffector.position;
            ik.solver.rightShoulderEffector.target.position = sittable.rightShoulderEffector.position;
        }
        else
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            //float a = 1 / ((1 - f) * (1 - f));
            if (stateInfo.normalizedTime >= 0.6f && stateInfo.normalizedTime <= 1f)
            {
                float weight = 2.5f * (stateInfo.normalizedTime - 0.6f) * 2.5f * (stateInfo.normalizedTime - 0.6f); // simple quadratic function to remove linearity
                ik.solver.bodyEffector.positionWeight = weight;
                ik.solver.leftShoulderEffector.positionWeight = weight;
                ik.solver.rightShoulderEffector.positionWeight = weight;
                //ik.solver.leftHandEffector.positionWeight = weight;
                //ik.solver.rightHandEffector.positionWeight = weight;                
            }
            if (stateInfo.normalizedTime >= 1f) finished = true;
        }

        return true;
    }

    public override bool AfterPerform(GameObject agent)
    {
        return true;
    }

    public override bool Set(GameObject agent)
    {
        isSitting = false;
        finished = false;
        mortal = agent.GetComponent<Mortal>();
        animator = mortal.animator;
        ik = mortal.ik;
        return animator != null && ik != null;
    }

    public override bool BeforePerform(GameObject agent)
    {        
        if (sittable.isFree) { sittable.isFree = false; return true; }
        else return false;
    }

    public override bool Abort(GameObject agent)
    {
        if (isSitting)
        {
            isSitting = false;
            if (mortal.mood.safety.value < 1f) animator.speed = 1.5f; // stand up faster if in danger 
            animator.CrossFade(sitToStandHash, 0.1f);
            return false;
        }
        else
        {
            bool finished = animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
            if (finished) animator.speed = 1f;
            return finished;
        }
    }
    //private Sittable[] sittables;
    //private Sittable currentSittable;
    //private Mortal mortal;
    //private Animator animator;
    //private FullBodyBipedIK ik;
    //private int standToSitHash = Animator.StringToHash("StandToSit");
    //private int sitToStandHash= Animator.StringToHash("SitToStand");
    //private float sitTimer;
    //private float initialCost;
    //public override bool CanPerform()
    //{
    //    return sittables.Length != 0;
    //}

    //public override void Init(GameObject agent)
    //{
    //    base.Init(agent);
    //    sittables = FindObjectsOfType<Sittable>();
    //    mortal = agent.GetComponent<Mortal>();
    //    animator = agent.GetComponentInChildren<Animator>();
    //    ik = agent.GetComponentInChildren<FullBodyBipedIK>();
    //    initialCost = cost;
    //    AddActionEffect("energy", (energy) => { return Mathf.Clamp01((float)energy + 0.3f); });
    //    AddActionEffect("isSitting", (isSitting) => { return true; });
    //    AddActionEffect("position", (position) => { return intera })
    //}

    //public override bool IsFinished()
    //{
    //    return sitTimer >= 5f;
    //}

    //public override bool IsInRange()
    //{
    //    if (currentSittable == null) return false;
    //    else
    //    {
    //        return (transform.position - targetPosition).magnitude < 0.1f;
    //    }
    //}
    //public static float CastRange(float value, float minIn, float maxIn, float minOut, float maxOut)
    //{
    //    float percentage = (value - minIn) / (maxIn - minIn);
    //    return minOut + percentage * (maxOut - minOut);
    //}
    //[Range(0f,1f)]
    //public float f = 0.6f;    
    //private bool isSitting;
    //public override bool Perform()
    //{
    //    if (!isSitting)
    //    {
    //        isSitting = true;
    //        animator.Play(standToSitHash);
    //        ik.solver.bodyEffector.target.position = currentSittable.bodyEffector.position;
    //        ik.solver.leftHandEffector.target.position = currentSittable.leftHandEffector.position;
    //        ik.solver.leftShoulderEffector.target.position = currentSittable.leftShoulderEffector.position;
    //        ik.solver.rightHandEffector.target.position = currentSittable.rightHandEffector.position;
    //        ik.solver.rightShoulderEffector.target.position = currentSittable.rightShoulderEffector.position;
    //    }
    //    else
    //    {
    //        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

    //        //float a = 1 / ((1 - f) * (1 - f));
    //        if (stateInfo.normalizedTime >= f && stateInfo.normalizedTime <= 1f)
    //        {
    //            float weight = Mathf.Sin(CastRange(stateInfo.normalizedTime, f, 1f, 0f, Mathf.PI / 2f)); //a * (stateInfo.normalizedTime - f) * (stateInfo.normalizedTime - f);//stateInfo.normalizedTime * stateInfo.normalizedTime * stateInfo.normalizedTime * stateInfo.normalizedTime;
    //            ik.solver.bodyEffector.positionWeight = weight;
    //            ik.solver.leftShoulderEffector.positionWeight = weight;
    //            ik.solver.leftHandEffector.positionWeight = weight;
    //            ik.solver.rightHandEffector.positionWeight = weight;
    //            ik.solver.rightShoulderEffector.positionWeight = weight;
    //        }
    //    }
    //    sitTimer += Time.deltaTime;

    //    return true;
    //}

    //public override bool AfterPerform()
    //{
    //    cost = 0f; // already sitting
    //    return true;
    //}

    //public override bool Set()
    //{
    //    currentSittable = sittables[0];
    //    targetPosition = currentSittable.interactionTransform.position;
    //    targetRotation = currentSittable.interactionTransform.rotation;
    //    hasTargetRotation = true;        
    //    return currentSittable != null;
    //}

    //public override bool BeforePerform()
    //{
    //    sitTimer = 0f;
    //    isSitting = animator.GetCurrentAnimatorStateInfo(0).shortNameHash == standToSitHash;
    //    return true;
    //}

    //public override bool Abort()
    //{
    //    cost = initialCost; // if aborted, stand up, thus the cost increases again
    //    if (isSitting)
    //    {
    //        isSitting = false;
    //        if (mortal.mortalData.safety.value > 0f) animator.speed = 1.5f; // stand up faster if in danger 
    //        animator.CrossFade(sitToStandHash, 0.1f);
    //        return false;
    //    }
    //    else
    //    {
    //        bool finished = animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    //        if (finished) animator.speed = 1f;
    //        return finished;
    //    }
    //}
}
