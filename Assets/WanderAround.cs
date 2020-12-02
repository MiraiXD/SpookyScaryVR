using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAround : GoapAction
{    
    private float timer;
    private Animator animator;
    private int idleHash = Animator.StringToHash("Idle");
    public override bool Abort(GameObject agent )
    {
        return true;
    }

    public override bool AfterPerform(GameObject agent)
    {
        return true;
    }

    public override bool BeforePerform(GameObject agent )
    {         
        animator.CrossFade(idleHash, 0.1f);
        return true;
    }

    public override bool Set(GameObject agent )
    {
        timer = 0f;
        targetPosition = NavMeshAgentRootMotion.GetRandomPositionOnNavMesh(agent.transform.position, 3f, -1);
        return true;
    }

    public override bool CanPerform(GameObject agent )
    {
        return true;
    }

    public override void Init(GameObject agent )
    {        
        animator = agent.GetComponentInChildren<Animator>();
        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == false; });
        AddActionEffect("fun", (fun) => { return Mathf.Clamp01((float)(fun) + 0.1f); });
        AddActionEffect("energy",(energy)=>{ return 0f; });
    }

    public override bool IsFinished(GameObject agent)
    {        
        return timer >= 0f;
    }

    public override bool IsInRange(GameObject agent)
    {
        //print((destination - agent.transform.position).magnitude < 0.1f);
        return (targetPosition - agent.transform.position).magnitude < 0.1f;
    }

    public override bool Perform(GameObject agent )
    {        
        timer += Time.deltaTime;
        return true;
    }
}
