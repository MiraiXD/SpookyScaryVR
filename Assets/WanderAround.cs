using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAround : GoapAction
{    
    private float timer;
    private Mortal mortal;    
    public override bool Abort(GoapAgent agent )
    {
        return true;
    }

    public override bool AfterPerform(GoapAgent agent)
    {
        return true;
    }

    public override bool BeforePerform(GoapAgent agent )
    {         
        mortal.animator.CrossFade(mortal.idle, 0.1f);
        return true;
    }

    public override bool Set(GoapAgent agent )
    {
        timer = 0f;
        targetPosition = NavMeshAgentRootMotion.GetRandomPositionOnNavMesh(agent.transform.position, 3f, -1);
        return true;
    }

    public override bool CanPerform(GoapAgent agent )
    {
        return true;
    }

    public override void Init(GoapAgent agent )
    {        
        mortal = agent.GetComponentInChildren<Mortal>();
        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == false; });
        AddActionEffect("fun", (fun) => { return Mathf.Clamp01((float)(fun) + 0.1f); });
        AddActionEffect("energy",(energy)=>{ return 0f; });
    }

    public override bool IsFinished(GoapAgent agent)
    {        
        return timer >= 0f;
    }

    public override bool IsInRange(GoapAgent agent)
    {
        //print((destination - agent.transform.position).magnitude < 0.1f);
        return (targetPosition - agent.transform.position).magnitude < 0.1f;
    }

    public override bool Perform(GoapAgent agent )
    {        
        timer += Time.deltaTime;
        return true;
    }
}
