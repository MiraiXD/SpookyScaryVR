using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sit : GoapAction
{
    private float sitTimer;
    public override void Init(GoapAgent agent)
    {
        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == true; });
        AddActionEffect("energy", (energy) => { return (float)energy + 0.5f; });
    }

    public override bool CanPerform(GoapAgent agent)
    {
        return true;
    }

    public override bool Set(GoapAgent agent)
    {
        sitTimer = 0f;
        return true;
    }

    public override bool IsInRange(GoapAgent agent)
    {
        return true;
    }

    public override bool BeforePerform(GoapAgent agent)
    {
        return true;
    }

    public override bool Perform(GoapAgent agent)
    {
        sitTimer += Time.deltaTime;
        return true;
    }

    public override bool AfterPerform(GoapAgent agent)
    {
        return true;
    }

    public override bool IsFinished(GoapAgent agent)
    {
        return sitTimer >= 5f;
    }

    public override bool Abort(GoapAgent agent)
    {
        return true;
    }
}
