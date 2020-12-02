using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate bool Condition(object obj);
public delegate object ActionEffect(object obj);
public interface IGoapDataProvider
{
    Goal GetGoal();
    Dictionary<string, object> GetStates();
    void PlanFound(Queue<GoapAction> plan, Goal goal);
    void PlanNotFound(Goal goal);
    void PlanFinished();
    void ActionFinished(GoapAction action);
    void PlanAborted(GoapAction abortedAction);
    bool MoveAgent(GoapAction action);
}

public class Goal
{
    public Dictionary<string, Condition> subgoals;
    public int priority = 0;
    public Goal(int priority)
    {
        subgoals = new Dictionary<string, Condition>();
        this.priority = priority;
    }
    public void AddSubGoal(string ID, Condition subGoal)
    {
        subgoals.Add(ID, subGoal);
    }
    public bool IsAchievableGiven(Dictionary<string, object> states)
    {
        foreach (KeyValuePair<string, Condition> subgoal in subgoals)
        {
            if (!states.ContainsKey(subgoal.Key) || (!subgoal.Value(states[subgoal.Key]))) return false;
        }
        return true;
    }
}

public class GoapAgent : MonoBehaviour
{
    private FSM fsm;
    private FSMState idleState, moveState, performState;
    private IGoapDataProvider dataProvider;
    private GoapPlanner planner;
    private GoapAction[] actions;
    private Queue<GoapAction> currentPlan;
    private GoapAction currentPerformedAction;
    public void Init(IGoapDataProvider dataProvider, GoapAction[] actions, GoapPlanner planner)
    {
        fsm = new FSM();
        this.dataProvider = dataProvider;
        this.actions = actions;
        this.planner = planner;
        CreateIdleState();
        CreatePerformState();
        CreateMoveState();

        fsm.PushState(idleState);
    }
    public void UpdateAgent(GameObject go)
    {
        fsm.Update(go);
    }
    private void CreateIdleState()
    {
        idleState = (fsm, agent) =>
        {
            Goal goal = dataProvider.GetGoal();
            Dictionary<string, object> states = dataProvider.GetStates();

            currentPlan = planner.Plan(agent, actions, states, goal);
            if (currentPlan != null)
            {
                dataProvider.PlanFound(currentPlan, goal);
                fsm.PopState();
                fsm.PushState(performState);
            }
            else
            {
                dataProvider.PlanNotFound(goal);
                fsm.PopState();
                fsm.PushState(idleState);
            }
        };
    }
    private void AbortPlan()
    {
        dataProvider.PlanAborted(currentPerformedAction);
        fsm.PopState();
        fsm.PushState(idleState);
    }
    private bool isPerforming = false;
    private bool isSet = false;
    private void CreatePerformState()
    {
        performState = (fsm, agent) =>
        {
            // invokes ActionFinished on data provider to let him apply the effects of the action
            // also removes it from the queue to proceed with the next action
            if (currentPerformedAction != null && currentPerformedAction.IsFinished(agent))
            {
                if (!currentPerformedAction.AfterPerform(agent)) { AbortPlan(); return; }
                dataProvider.ActionFinished(currentPerformedAction);
                currentPlan.Dequeue();
                currentPerformedAction = null;
                isPerforming = false;
                isSet = false;
            }

            if (currentPlan.Count > 0)
            {
                GoapAction action = currentPlan.Peek();
                if (!isPerforming)
                {
                    currentPerformedAction = action;
                    if (!isSet)
                    {
                        isSet = true;
                        if (!currentPerformedAction.Set(agent)) { AbortPlan(); return; }
                    }

                    if (currentPerformedAction.IsInRange(agent))
                    {
                        isPerforming = true;
                        if (!currentPerformedAction.BeforePerform(agent))
                        {
                            AbortPlan(); return;
                        }
                        return;
                    }
                    else
                    {
                        //currentPerformedAction = null;
                        fsm.PushState(moveState);
                        return;
                    }
                }
                if (currentPerformedAction.IsInRange(agent))
                {
                    if (!currentPerformedAction.Perform(agent)) { AbortPlan(); return; }
                }
                else
                {
                    fsm.PushState(moveState);
                }
            }
            else
            {
                dataProvider.PlanFinished();
                fsm.PopState();
                fsm.PushState(idleState);
            }
        };
    }
    private void CreateMoveState()
    {
        moveState = (fsm, agent) =>
            {
                if (dataProvider.MoveAgent(currentPlan.Peek()))
                    fsm.PopState();
            };
    }
}
