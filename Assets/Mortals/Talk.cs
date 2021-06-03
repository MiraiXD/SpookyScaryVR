using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Talk : GoapAction
{
    public float companionshipGain = 0.5f;
    public bool isLookingForInterlocutor { get; private set; }
    public bool isTalking { get; private set; }
    public bool isListening { get; private set; }
    private bool isWaitingForInterlocutor;
    private Mortal mortal;
    private Talk[] talkActionsAround;
    private Talk currentInterlocutor;

    private float talkingDistance = 1f; // distance between talking people    
    // for waiting interlocutors only
    public bool TalkWithMe(Talk talk)
    {
        isLookingForInterlocutor = false;
        currentInterlocutor = talk;
        isWaitingForInterlocutor = true;
        return true;
    }
    // for waiting interlocutors only
    public void InterlocutorArrived()
    {
        isWaitingForInterlocutor = false;
    }
    public override void Init(GoapAgent agent)
    {
        mortal = GetComponent<Mortal>();
        talkActionsAround = FindObjectsOfType<Talk>().Where(t => t != this).ToArray();

        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == false; });
        AddActionEffect("companionship", (companionship) => { return (float)companionship + companionshipGain; });

    }
    public override bool CanPerform(GoapAgent agent)
    {
        return talkActionsAround != null && talkActionsAround.Length > 0;
    }
    public override float GetCost(GoapAgent agent)
    {
        return 1f / companionshipGain;
    }
    public int talkCyclesLeft;

    public override bool Set(GoapAgent agent)
    {        
        currentInterlocutor = null;
        isWaitingForInterlocutor = false;
        isTalking = false;
        isListening = false;
        foreach (Talk talk in talkActionsAround)
        {
            if (talk.isLookingForInterlocutor && talk.TalkWithMe(this))
            {
                talkCyclesLeft = 2;
                talk.talkCyclesLeft = 2;
                
                currentInterlocutor = talk;
                mortal.stoppingDistance = talkingDistance;
                targetPosition = currentInterlocutor.transform.position;
            }
        }

        if (currentInterlocutor == null)
            isLookingForInterlocutor = true;

        return true;
    }

    public override bool IsInRange(GoapAgent agent)
    {
        if (currentInterlocutor == null || isWaitingForInterlocutor) { /*print(targetPosition); mortal.followTarget = false; */return true; }
        else
        {
            targetPosition = currentInterlocutor.transform.position;
            float distance = (targetPosition - agent.transform.position).magnitude;
            isInRange = distance <= talkingDistance;
            if (currentInterlocutor.isWaitingForInterlocutor && isInRange)
            {
                currentInterlocutor.InterlocutorArrived();
            }
            return isInRange;
        }
    }

    public override bool LoopPerform(GoapAgent agent)
    {
        if (currentInterlocutor == null || isWaitingForInterlocutor) return true;
        else
        {
            if (talkCyclesLeft <= 0) { isFinished = true; return true; }

            Vector3 looAtInterlocutor = (currentInterlocutor.transform.position - agent.transform.position).normalized;
            agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, Quaternion.LookRotation(looAtInterlocutor), 5f);
            if (currentInterlocutor.isTalking)
            {
                //listen
                if (!isListening)
                {                    
                    isListening = true;
                    mortal.animator.CrossFadeInFixedTime(mortal.idle, 0.1f);
                }
            }
            else
            {
                //speak
                if (!isTalking)
                {                    
                    isListening = false;
                    isTalking = true;
                    mortal.animator.CrossFadeInFixedTime(mortal.talk, 0.3f);
                }
                else
                {
                    if (mortal.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        isTalking = false;

                        talkCyclesLeft--;
                        currentInterlocutor.talkCyclesLeft--;
                    }
                }
            }
        }
        return true;
    }

    public override bool EndPerform(GoapAgent agent)
    {
        mortal.stoppingDistance = 0f;
        return true;
    }

    public override bool Abort(GoapAgent agent)
    {
        return true;
    }
}
