using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAround : GoapAction
{
    public float funGain = 0.1f;
    private float timer;
    private Mortal mortal;
    public override bool Abort(GoapAgent agent)
    {
        return true;
    }

    public override bool BeginPerform(GoapAgent agent)
    {
        mortal.animator.CrossFadeInFixedTime(mortal.idle, 0.1f);
        return true;
    }

    public override bool Set(GoapAgent agent)
    {        
        timer = 0f;        
        if (!NavMeshAgentRootMotion.RandomPointOnNavMesh(agent.transform.position, 15f, out Vector3 result)) return false;
        targetPosition = result;
        return true;
    }


    public override void Init(GoapAgent agent)
    {
        mortal = agent.GetComponentInChildren<Mortal>();
        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == false; });
        AddActionEffect("fun", (fun) => { return (float)(fun) + funGain; });
        //AddActionEffect("energy", (energy) => { return 0f; });
    }
    public override float GetCost(GoapAgent agent)
    {
        return 100f / funGain; // high cost, last resort action
    }
    public override bool IsFinished(GoapAgent agent)
    {
        return timer >= 2f;
    }

    public override bool LoopPerform(GoapAgent agent)
    {
        timer += Time.deltaTime;
        return true;
    }
}
