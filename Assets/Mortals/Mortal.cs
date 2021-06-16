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
    public Mood mood;
    public Animator animator { get; private set; }

    //animator hashes
    public readonly int idle = Animator.StringToHash("Idle");
    public readonly int locomotion = Animator.StringToHash("Locomotion");
    public readonly int standToSit = Animator.StringToHash("StandToSit");
    public readonly int sitToStand = Animator.StringToHash("SitToStand");
    public readonly int talk = Animator.StringToHash("Talk");
    public void SetAnimation(int animationHash, float fixedTransitionDuration)
    {
        animator.CrossFadeInFixedTime(animationHash, fixedTransitionDuration);
    }
    public FullBodyBipedIK ik { get; private set; }
    private NavMeshAgentRootMotion moveAgent;
    private ForwardAnimatorMove forwardAnimatorMove;
    private GoapAgent goapAgent;
    private GoapAction[] availableActions;

    private Queue<string> needs;
    private List<string> failedNeeds;
    private string currentNeed;
    public Goal CreateGoal()
    {
        Goal goal = new Goal(0);
        //KeyValuePair<string,float> biggestNeed = mood.GetBiggestNeeds();
        //if (needs.Count == 0)
        //{
        //    foreach (var need in mood.GetBiggestNeed())
        //    {
        //        needs.Enqueue(need.Key);
        //    }
        //}
        //string biggestNeed = needs.Dequeue();

        bool foundNeed = false;
        foreach (var need in mood.GetBiggestNeeds())
        {
            if (!failedNeeds.Contains(need.Key)) { currentNeed = need.Key; foundNeed = true; break; }
        }
        if (!foundNeed) Debug.LogError("No need can be satisfied!");

        print("BiggestNeed: " + currentNeed);
        goal.AddSubGoal(currentNeed, (need) => { return (float)need > mood.GetNeed(currentNeed).Value; });
        return goal;
    }
    public Dictionary<string, object> GetStates()
    {
        return mood.ToStates();
    }
    public void PlanFound(Queue<GoapAction> plan, Goal goal)
    {
        foreach (var a in plan)
        {
            print(a.GetType());
        }
    }

    public void PlanNotFound(Goal goal)
    {
        failedNeeds.Add(currentNeed);
        if(moveAgent.hasDestination)
        moveAgent.Stop(); // stop moving 
    }

    public void PlanFinished()
    {
        failedNeeds.Clear();// prepare for next use
        //needs.Clear(); // prepare for next use
    }

    public void PlanAborted(GoapAction abortedAction)
    {
        if(moveAgent.hasDestination)
        moveAgent.Stop(); // stop moving 
    }

    private bool isMoving = false;
    private bool destinationReached = false;
    public float stoppingDistance { get; set; }
    public bool MoveAgent(GoapAction action)
    {
        if (!isMoving && !destinationReached)
        {
            if (action.hasTargetRotation) moveAgent.SetDestinationAndTargetRotation(action.GetTargetPosition(), action.GetTargetRotation(), stoppingDistance);
            else { moveAgent.SetDestination(action.GetTargetPosition(), stoppingDistance); }
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

        mood = new Mood();
        needs = new Queue<string>();

        failedNeeds = new List<string>();

        goapAgent = GetComponent<GoapAgent>();
        availableActions = FindObjectsOfType<GoapAction>().Where(action => action.GetComponentInParent<Mortal>() == null || action.GetComponentInParent<Mortal>() == this).ToArray();
        foreach (var action in availableActions) { action.Init(goapAgent); }
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
        print("REACHED");
        isMoving = false;
        destinationReached = true;
    }

    private void OnDestinationNear()
    {
        //animator.CrossFadeInFixedTime(idle, 0.1f);///moveAgent.speed);
    }

    private void OnDepart()
    {
        print("OnDepart " + gameObject);
        if (!isMoving)
        {
            isMoving = true;
            destinationReached = false;
            animator.CrossFadeInFixedTime(locomotion, 0.2f);
        }
    }
    private void OnStopMoving()
    {
        print("STOPMOVING");
        isMoving = false;
        destinationReached = false;
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != idle)
            animator.CrossFadeInFixedTime(idle, 0.1f);///moveAgent.speed);
    }
    private void Update()
    {
        goapAgent.UpdateAgent(gameObject);
        mood.UpdateMood(Time.deltaTime);
    }

}
