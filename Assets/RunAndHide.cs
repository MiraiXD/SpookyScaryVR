using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAndHide : GoapAction
{
    private GameObject agent;
    private Mortal mortal;
    private NavMeshAgentRootMotion navMeshAgent;
    private bool destinationReached;
    public override bool CanPerform(GameObject agent)
    {
        return true;
    }

    public override void Init(GameObject agent)
    {        
        this.agent = agent;
        navMeshAgent = agent.GetComponent<NavMeshAgentRootMotion>();
        AddPreCondition("threat", (threat) => { return (float)threat >= 0.5f; });
        AddActionEffect("threat", (threat) => { float t = (float)threat; t -= 0.5f; return t; });
    }

    public override bool IsFinished(GameObject agent)
    {
        return destinationReached;
    }

    public override bool IsInRange(GameObject agent)
    {
        return true;
    }

    public override bool Perform(GameObject agent)
    {
        return true;
    }

    public override bool Set(GameObject agent)
    {
        destinationReached = false;
        navMeshAgent.onDestinationReached += OnDestinationReached;
        return true;
    }

    public override bool AfterPerform(GameObject agent)
    {
        navMeshAgent.onDestinationReached -= OnDestinationReached;        
        return true;
    }

    private void OnDestinationReached()
    {
        destinationReached = true;
    }

    public override bool BeforePerform(GameObject agent)
    {
        throw new System.NotImplementedException();
    }

    public override bool Abort(GameObject agent)
    {
        throw new System.NotImplementedException();
    }
}
