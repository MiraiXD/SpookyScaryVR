using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sit : GoapAction
{
    public float energyGain = 0.2f;
    private float sitTimer;
    public override void Init(GoapAgent agent)
    {
        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == true; });
        AddActionEffect("energy", (energy) => { return (float)energy + energyGain; });
    }
    public override bool IsInRange(GoapAgent agent)
    {
        return true;
    }
    public override bool BeginPerform(GoapAgent agent)
    {        
        sitTimer = 0f;
        return true;
    }
    
    public override bool LoopPerform(GoapAgent agent)
    {
        sitTimer += Time.deltaTime;
        return true;
    }
    
    public override bool IsFinished(GoapAgent agent)
    {
        if (sitTimer >= 2f) return true;
        else return false;
    }

    public override bool Abort(GoapAgent agent)
    {
        return true;
    }
}
