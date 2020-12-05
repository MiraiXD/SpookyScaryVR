using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;
using System.Linq;
[RequireComponent(typeof(NavMeshAgentRootMotion))]
public class Mortal : MonoBehaviour, IGoapDataProvider
{
    public Mood mood = new Mood();
    public Animator animator { get; private set; }

    //animator hashes
    public readonly int idle = Animator.StringToHash("Idle");
    public readonly int locomotion = Animator.StringToHash("Locomotion");
    public readonly int standToSit = Animator.StringToHash("StandToSit");
    public readonly int sitToStand = Animator.StringToHash("SitToStand");
    public readonly int talk = Animator.StringToHash("Talk");
    public FullBodyBipedIK ik { get; private set; }
    private NavMeshAgentRootMotion moveAgent;
    private ForwardAnimatorMove forwardAnimatorMove;
    private GoapAgent goapAgent;
    private GoapAction[] availableActions;
    public Goal CreateGoal()
    {
        Goal goal = new Goal(0);
        goal.AddSubGoal("energy", (energy) => { return (float)energy > 0.4f; });
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
        moveAgent.Stop(); // stop moving 
    }

    public void PlanFinished()
    {
    }

    public void PlanAborted(GoapAction abortedAction)
    {
        moveAgent.Stop(); // stop moving 
    }

    private bool isMoving = false;
    private bool destinationReached = false;    
    public float stoppingDistance { get; set; }
    public bool MoveAgent(GoapAction action)
    {
        if (!isMoving && !destinationReached)// && Time.frameCount % 30 == 0))
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
        moveAgent.onStopMoving += OnStopMoving;

        goapAgent = GetComponent<GoapAgent>();
        availableActions = FindObjectsOfType<GoapAction>().Where(action => action.GetComponentInParent<Mortal>() == null || action.GetComponentInParent<Mortal>() == this).ToArray();
        foreach (var action in availableActions) action.Init(goapAgent);
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
        animator.CrossFadeInFixedTime(idle, 0.19f);///moveAgent.speed);
    }

    private void OnDepart()
    {
        if (!isMoving)
        {
            isMoving = true;
            destinationReached = false;
            animator.CrossFadeInFixedTime(locomotion, 0.2f);
        }
    }
    private void OnStopMoving()
    {
        animator.CrossFadeInFixedTime(idle, 0.19f);///moveAgent.speed);
    }
    private bool walk = true;
    private void Update()
    {
        goapAgent.UpdateAgent(gameObject);

    }
}
