using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class SitToStand : GoapAction
{
    private Sittable sittable;
    private Mortal mortal;    
    private FullBodyBipedIK ik;    
    private bool finished;
    private bool isSitting;
    public override void Init(GoapAgent agent)
    {
        sittable = GetComponent<Sittable>();
        
        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == true; });
        AddActionEffect("isSitting", (isSitting) => { return false; });
        AddActionEffect("position", (position) => { return sittable.interactionTransform.position; });
    }
    public override bool IsInRange(GoapAgent agent)
    {
        return true;
    }
    public override bool Set(GoapAgent agent)
    {
        isSitting = true;
        finished = false;
        mortal = agent.GetComponent<Mortal>();        
        ik = mortal.ik;
        return mortal.animator != null && ik != null;
    }

    public override bool BeginPerform(GoapAgent agent)
    {
        finishedIK = false;

        ik.solver.bodyEffector.target.position = sittable.bodyEffector.position;
        ik.solver.leftHandEffector.target.position = sittable.leftHandEffector.position;
        ik.solver.leftShoulderEffector.target.position = sittable.leftShoulderEffector.position;
        ik.solver.rightHandEffector.target.position = sittable.rightHandEffector.position;
        ik.solver.rightShoulderEffector.target.position = sittable.rightShoulderEffector.position;

        ik.solver.bodyEffector.positionWeight = 1f;
        ik.solver.leftShoulderEffector.positionWeight = 1f;
        ik.solver.rightShoulderEffector.positionWeight = 1f;
        ik.solver.leftHandEffector.positionWeight = 1f;
        ik.solver.rightHandEffector.positionWeight = 1f;

        mortal.SetAnimation(mortal.sitToStand, 0f); // anything bigger than 0f shifts the normalized time
        isSitting = false;
        return true;
    }
    private bool finishedIK;
    public override bool LoopPerform(GoapAgent agent)
    {
        //if (isSitting)
        //{
        //    isSitting = false;            
        //    //mortal.animator.Play(mortal.sitToStand);            
        //}
        //else
        //{                
            var stateInfo = mortal.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime <= 0.4f && stateInfo.normalizedTime >= 0f)
            {
                float weight = 1f - (2.5f * (stateInfo.normalizedTime ) * 2.5f * (stateInfo.normalizedTime)); // simple quadratic function to remove linearity                
                ik.solver.bodyEffector.positionWeight = weight;
                ik.solver.leftShoulderEffector.positionWeight = weight;
                ik.solver.rightShoulderEffector.positionWeight = weight;
                ik.solver.leftHandEffector.positionWeight = weight;
                ik.solver.rightHandEffector.positionWeight = weight;                
            }       
            else if(!finishedIK && stateInfo.normalizedTime > 0.4f)
            {
                finishedIK = true;
                ik.solver.bodyEffector.positionWeight = 0f;
                ik.solver.leftShoulderEffector.positionWeight = 0f;
                ik.solver.rightShoulderEffector.positionWeight = 0f;
                ik.solver.leftHandEffector.positionWeight = 0f;
                ik.solver.rightHandEffector.positionWeight = 0f;

                ik.solver.bodyEffector.target.position = agent.transform.position;
                ik.solver.leftHandEffector.target.position = agent.transform.position;
                ik.solver.leftShoulderEffector.target.position = agent.transform.position;
                ik.solver.rightHandEffector.target.position = agent.transform.position;
                ik.solver.rightShoulderEffector.target.position = agent.transform.position;
            }
            if (stateInfo.normalizedTime >= 1f) finished = true;
        

        return true;
    }

    public override bool EndPerform(GoapAgent agent)
    {        
        sittable.isFree = true;
        return true;
    }

    public override bool IsFinished(GoapAgent agent)
    {
        return finished;
    }

    public override bool Abort(GoapAgent agent)
    {
        return true;
    }
}
