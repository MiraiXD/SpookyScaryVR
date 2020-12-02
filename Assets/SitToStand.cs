using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class SitToStand : GoapAction
{
    private Sittable sittable;
    private Mortal mortal;
    private Animator animator;
    private FullBodyBipedIK ik;
    private int standToSitHash = Animator.StringToHash("StandToSit");
    private int sitToStandHash = Animator.StringToHash("SitToStand");
    private bool finished;
    private bool isSitting;
    public override void Init(GameObject agent)
    {
        sittable = GetComponent<Sittable>();
        
        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == true; });
        AddActionEffect("isSitting", (isSitting) => { return false; });
        AddActionEffect("position", (position) => { return sittable.interactionTransform.position; });
    }

    public override bool CanPerform(GameObject agent)
    {
        return true;
    }

    public override bool Set(GameObject agent)
    {
        isSitting = true;
        finished = false;
        mortal = agent.GetComponent<Mortal>();
        animator = mortal.animator;
        ik = mortal.ik;
        return animator != null && ik != null;
    }

    public override bool IsInRange(GameObject agent)
    {
        return true;
    }

    public override bool BeforePerform(GameObject agent)
    {        
        return true;
    }
    
    public override bool Perform(GameObject agent)
    {
        if (isSitting)
        {
            isSitting = false;            
            animator.Play(sitToStandHash);
        }
        else
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime <= 0.4f && stateInfo.normalizedTime >= 0f)
            {
                float weight = 1f - (2.5f * (stateInfo.normalizedTime ) * 2.5f * (stateInfo.normalizedTime)); // simple quadratic function to remove linearity
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
        ik.solver.bodyEffector.positionWeight = 0f;
        ik.solver.leftShoulderEffector.positionWeight = 0f;
        ik.solver.rightShoulderEffector.positionWeight = 0f;
        ik.solver.bodyEffector.target.position = agent.transform.position;
        ik.solver.leftHandEffector.target.position = agent.transform.position;
        ik.solver.leftShoulderEffector.target.position = agent.transform.position;
        ik.solver.rightHandEffector.target.position = agent.transform.position;
        ik.solver.rightShoulderEffector.target.position = agent.transform.position;
        sittable.isFree = true;
        return true;
    }

    public override bool IsFinished(GameObject agent)
    {
        return finished;
    }

    public override bool Abort(GameObject agent)
    {
        return true;
    }
}
