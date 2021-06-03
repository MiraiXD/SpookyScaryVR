using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
public class StandToSit : GoapAction
{
    private Sittable sittable;
    private Sit sitAction;
    private Mortal mortal;    
    private FullBodyBipedIK ik;    
    
    public override bool CanPerform(GoapAgent agent)
    {
        return sittable.isFree;
    }
    public override float GetCost(GoapAgent agent)
    {
        return 1f;// (sittable.transform.position - agent.transform.position).magnitude / sitAction.energyGain;
    }
    public override void Init(GoapAgent agent)
    {
        sittable = GetComponent<Sittable>();
        sitAction = GetComponent<Sit>();
        targetPosition = sittable.interactionTransform.position;
        targetRotation = sittable.interactionTransform.rotation;
        hasTargetRotation = true;

        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == false; });
        AddActionEffect("isSitting", (isSitting) => { return true; });
        AddActionEffect("position", (position) => { return sittable.interactionTransform.position; });
    }
    public override bool Set(GoapAgent agent)
    {        
        isSitting = false;
        return true;
    }
    
    public override bool Traverse(GoapAgent agent)
    {
        if (sittable.isFree)
        {            
            base.Traverse(agent);
            print(isInRange + ", " + agent.gameObject);
            return true;
        }
        else
        {            
            return false;
        }
    }
    
    private bool isSitting;
    public override bool LoopPerform(GoapAgent agent)
    {
        if (!isSitting)
        {
            isSitting = true;
            mortal.animator.Play(mortal.standToSit);
            ik.solver.bodyEffector.target.position = sittable.bodyEffector.position;
            ik.solver.leftHandEffector.target.position = sittable.leftHandEffector.position;
            ik.solver.leftShoulderEffector.target.position = sittable.leftShoulderEffector.position;
            ik.solver.rightHandEffector.target.position = sittable.rightHandEffector.position;
            ik.solver.rightShoulderEffector.target.position = sittable.rightShoulderEffector.position;
        }
        else
        {
            var stateInfo = mortal.animator.GetCurrentAnimatorStateInfo(0);

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
            if (stateInfo.normalizedTime >= 1f) isFinished = true;
        }

        return true;
    } 

    public override bool BeginPerform(GoapAgent agent)
    {        
        if (sittable.isFree) {
            sittable.isFree = false;                        
            mortal = agent.GetComponent<Mortal>();
            ik = mortal.ik;
            return mortal.animator != null && ik != null;
        }
        else return false;
    }

    public override bool Abort(GoapAgent agent)
    {
        if (isSitting)
        {
            isSitting = false;
            if (mortal.mood.safety.Value < 1f) mortal.animator.speed = 1.5f; // stand up faster if in danger 
            mortal.animator.CrossFade(mortal.sitToStand, 0.1f);
            return false;
        }
        else
        {
            bool finished = mortal.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
            if (finished) mortal.animator.speed = 1f;
            return finished;
        }
    }   
}
