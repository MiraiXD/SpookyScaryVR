using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAndHide : GoapAction
{
    private GoapAgent agent;
    private Mortal mortal;
    private NavMeshAgentRootMotion navMeshAgent;
    private bool destinationReached;
    
    public override void Init(GoapAgent agent)
    {        
        this.agent = agent;
        navMeshAgent = agent.GetComponent<NavMeshAgentRootMotion>();
        AddPreCondition("threat", (threat) => { return (float)threat >= 0.5f; });
        AddActionEffect("threat", (threat) => { float t = (float)threat; t -= 0.5f; return t; });
    }

    public override bool IsFinished(GoapAgent agent)
    {
        return destinationReached;
    }
    
    public override bool Set(GoapAgent agent)
    {
        destinationReached = false;
        navMeshAgent.onDestinationReached += OnDestinationReached;
        return true;
    }

    public override bool EndPerform(GoapAgent agent)
    {
        navMeshAgent.onDestinationReached -= OnDestinationReached;        
        return true;
    }

    private void OnDestinationReached()
    {
        destinationReached = true;
    }
    
    public override bool Abort(GoapAgent agent)
    {
        throw new System.NotImplementedException();
    }
}
