using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sit : GoapAction
{
    private float sitTimer;
    public override void Init(GameObject agent)
    {
        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == true; });
        AddActionEffect("energy", (energy) => { return Mathf.Clamp01((float)energy + 0.5f); });
    }

    public override bool CanPerform(GameObject agent)
    {
        return true;
    }

    public override bool Set(GameObject agent)
    {
        sitTimer = 0f;
        return true;
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
        sitTimer += Time.deltaTime;
        return true;
    }

    public override bool AfterPerform(GameObject agent)
    {
        return true;
    }

    public override bool IsFinished(GameObject agent)
    {
        return sitTimer >= 5f;
    }

    public override bool Abort(GameObject agent)
    {
        return true;
    }
}
