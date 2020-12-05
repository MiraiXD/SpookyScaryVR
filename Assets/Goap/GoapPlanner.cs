using System;
using UnityEngine;
using System.Collections.Generic;
//public class LowestCostPlanner : GoapPlanner
//{

//}
public class GoapPlanner
{
    public GoapPlanner()
    {
    }

    public Queue<GoapAction> Plan(GoapAgent agent, GoapAction[] availableActions, Dictionary<string, object> worldStates, Goal goal)
    {
        List<GoapAction> usableActions = new List<GoapAction>();
        foreach (var action in availableActions)
        {
            //action.ResetAction();
            if (action.CanPerform(agent)) usableActions.Add(action);
        }

        List<Node> leaves = new List<Node>();
        Node parent = new Node(null, 0f, worldStates, null);
        bool success = BuildGraph(parent, leaves, usableActions, goal);
        if (!success) { Debug.LogError("No plan"); return null; }

        Node cheapest = leaves[0];
        foreach (Node node in leaves)
        {
            if (node.runningCost < cheapest.runningCost) cheapest = node;
        }

        List<GoapAction> actions = new List<GoapAction>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
                actions.Insert(0, n.action);
            n = n.parent;
        }
        Queue<GoapAction> plan = new Queue<GoapAction>();
        foreach (GoapAction action in actions) plan.Enqueue(action);

        return plan;

    }
    private bool BuildGraph(Node parent, List<Node> leaves, List<GoapAction> usableActions, Goal goal)
    {
        bool foundOne = false;
        foreach (GoapAction action in usableActions)
        {
            if (MatchConditions(parent.states, action.PreConditions))
            {
                var currentStates = DeepCloneDictionary(parent.states);
                currentStates = GoapAction.ApplyActionEffects(currentStates, action.ActionEffects);
                Node node = new Node(parent, parent.runningCost + action.cost, currentStates, action);
                if (MatchConditions(currentStates, goal.subgoals))
                {
                    leaves.Add(node);
                    foundOne = true;
                }
                else
                {
                    var usableActionsCopy = new List<GoapAction>(usableActions);
                    usableActionsCopy.Remove(action);
                    foundOne = BuildGraph(node, leaves, usableActionsCopy, goal);
                }
            }
        }

        return foundOne;

    }
    private bool MatchConditions(Dictionary<string, object> states, Dictionary<string, Condition> conditions)
    {
        foreach (KeyValuePair<string, Condition> kvp in conditions)
        {// if a single condition is not met
            if (!kvp.Value(states[kvp.Key])) return false;
        }
        // all conditions are met
        return true;
    }
    
    public static Dictionary<TKey, TValue> DeepCloneDictionary<TKey, TValue>(Dictionary<TKey, TValue> original)
    {
        Dictionary<TKey, TValue> copy = new Dictionary<TKey, TValue>();
        foreach (KeyValuePair<TKey, TValue> kvp in original)
        {
            ICloneable cloneable = kvp.Value as ICloneable;
            if (cloneable != null) copy.Add(kvp.Key, (TValue)cloneable.Clone());
            else copy.Add(kvp.Key, kvp.Value);
        }
        return copy;
    }

    private class Node
    {
        public Node parent;
        public float runningCost;
        public Dictionary<string, object> states;
        public GoapAction action;

        public Node(Node parent, float runningCost, Dictionary<string, object> states, GoapAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.states = states;
            this.action = action;
        }
    }
}
