using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoapAction : MonoBehaviour
{
    public Dictionary<string, Condition> PreConditions { get; } = new Dictionary<string, Condition>();
    public Dictionary<string, ActionEffect> ActionEffects { get; } = new Dictionary<string, ActionEffect>();        
    protected Vector3 targetPosition { get; set; }
    protected Quaternion targetRotation { get; set; }
    public bool hasTargetRotation { get; set; }

    protected bool isInRange = false;
    protected bool isFinished = false;
    public virtual void Init(GoapAgent agent) { }
    public virtual bool CanPerform(GoapAgent agent) { return true; }
    public virtual bool Set(GoapAgent agent) { return true; }
    public virtual bool BeginPerform(GoapAgent agent) { return true; }
    public virtual bool LoopPerform(GoapAgent agent) { return true; }
    public virtual bool EndPerform(GoapAgent agent) { return true; }
    public virtual bool IsFinished(GoapAgent agent) { return isFinished; }
    public virtual bool Abort(GoapAgent agent) { return true; }
    public virtual bool IsInRange(GoapAgent agent)
    {
        return isInRange;
    }
    public virtual bool Traverse(GoapAgent agent)
    {
        isInRange = agent.dataProvider.MoveAgent(this);
        return true;
    }
    public virtual float GetCost(GoapAgent agent) { return 0f; }
    public void ResetAction()
    {        
        isInRange = false;
        isFinished = false;
    }
    public virtual Vector3 GetTargetPosition()
    {
        return targetPosition;
    }
    public virtual Quaternion GetTargetRotation()
    {
        return targetRotation;
    }
    public Dictionary<string,object> GetStatesWithAppliedEffects(Dictionary<string,object> states)
    {
        return ApplyActionEffects(states, ActionEffects);        
    }

    public static Dictionary<string, object> ApplyActionEffects(Dictionary<string, object> states, Dictionary<string, ActionEffect> effects)
    {
        foreach (KeyValuePair<string, ActionEffect> effect in effects)
        {
            if (states.TryGetValue(effect.Key, out object obj))
                states[effect.Key] = effect.Value(obj);
            else
                states.Add(effect.Key, effect.Value(null));
        }
        return states;
    }

    public void AddPreCondition(string ID, Condition condition)
    {
        if (PreConditions.ContainsKey(ID)) PreConditions[ID] = condition;
        else PreConditions.Add(ID, condition);
    }
    public void RemovePreCondition(string ID)
    {
        PreConditions.Remove(ID);
    }
    public void AddActionEffect(string ID, ActionEffect actionEffect)
    {
        if (ActionEffects.ContainsKey(ID)) ActionEffects[ID] = actionEffect;
        else ActionEffects.Add(ID, actionEffect);
    }
    public void RemoveActionEffect(string ID)
    {
        ActionEffects.Remove(ID);
    }
}
