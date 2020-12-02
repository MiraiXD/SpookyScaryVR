using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;
[RequireComponent(typeof(NavMeshAgentRootMotion))]
public class Mortal : MonoBehaviour, IGoapDataProvider
{
    public Mood mood = new Mood();
    public Animator animator { get; private set; }
    public FullBodyBipedIK ik { get; private set; }
    private NavMeshAgentRootMotion moveAgent;
    private ForwardAnimatorMove forwardAnimatorMove;
    private GoapAgent goapAgent;
    private GoapAction[] availableActions;
    public Goal GetGoal()
    {
        Goal goal = new Goal(0);
        goal.AddSubGoal("companionship", (companionship) => { return (float)companionship > mood.companionship.value; });
        //if (mood.energy.value == 0f)
        //    goal.AddSubGoal("energy", (energy) => { return (float)energy > 0f; });        
        //else
        //    goal.AddSubGoal("fun", (fun) => { return (float)fun > 0f; });

        return goal;
    }
    public Dictionary<string, object> GetStates()
    {
        return mood.ToStates();
    }
    public void PlanFound(Queue<GoapAction> plan, Goal goal)
    {

    }

    public void PlanNotFound(Goal goal)
    {
    }

    public void PlanFinished()
    {
    }

    public void PlanAborted(GoapAction abortedAction)
    {
    }

    private bool isMoving = false;
    private bool destinationReached = false;
    public bool followTarget { get; set; }    
    public float stoppingDistance { get; set; }
    public bool MoveAgent(GoapAction action)
    {
        if ((!isMoving && !destinationReached) || (followTarget))// && Time.frameCount % 30 == 0))
        {
            if (action.hasTargetRotation) moveAgent.SetDestinationAndTargetRotation(action.GetTargetPosition(), action.GetTargetRotation(), stoppingDistance);
            else { moveAgent.SetDestination(action.GetTargetPosition(), stoppingDistance);}
        }

        if (destinationReached) // prepare for next movement
        {
            isMoving = false;
            destinationReached = false;
            return true;
        }
        else return false;
    }

    public void ActionFinished(GoapAction action)
    {
        var states = action.GetStatesWithAppliedEffects(GetStates());
        mood.FromStates(states);
    }
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ik = GetComponentInChildren<FullBodyBipedIK>();
        forwardAnimatorMove = GetComponentInChildren<ForwardAnimatorMove>();
        forwardAnimatorMove.onAnimatorMove += AnimatorMove;
        forwardAnimatorMove.onAnimationEvent += AnimationEvent;
        moveAgent = GetComponent<NavMeshAgentRootMotion>();
        moveAgent.onDepart += OnDepart;
        moveAgent.onDestinationNear += OnDestinationNear;
        moveAgent.onDestinationReached += OnDestinationReached;

        goapAgent = GetComponent<GoapAgent>();
        availableActions = GetComponents<GoapAction>();
        foreach (var action in availableActions) action.Init(gameObject);
        goapAgent.Init(this, availableActions, new GoapPlanner());
    }
    private void AnimationEvent(string animationEvent)
    {

    }
    private void AnimatorMove(Animator animator)
    {
        moveAgent.AnimatorMove(animator);
    }

    private void OnDestinationReached()
    {
        isMoving = false;
        destinationReached = true;
        
    }

    private void OnDestinationNear()
    {        
        animator.CrossFadeInFixedTime("Idle", 0.19f);///moveAgent.speed);
    }

    private void OnDepart()
    {
        if (!isMoving)
        {
            isMoving = true;
            destinationReached = false;
            animator.CrossFadeInFixedTime("Locomotion", 0.2f);
        }        
    }
    private bool walk = true;
    private void Update()
    {
        goapAgent.UpdateAgent(gameObject);

    }
}
