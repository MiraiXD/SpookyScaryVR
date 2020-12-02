using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Talk : GoapAction
{
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
    public override void Init(GameObject agent)
    {
        mortal = GetComponent<Mortal>();
        talkActionsAround = FindObjectsOfType<Talk>().Where(t => t != this).ToArray();

        AddPreCondition("isSitting", (isSitting) => { return (bool)isSitting == false; });
        AddActionEffect("companionship", (companionship) => { return (float)companionship + 0.5f; });
    }

    public override bool CanPerform(GameObject agent)
    {
        return true;
    }

    public override bool Set(GameObject agent)
    {
        currentInterlocutor = null;
        isWaitingForInterlocutor = false;
        foreach (Talk talk in talkActionsAround)
        {
            if (talk.isLookingForInterlocutor && talk.TalkWithMe(this))
            {
                currentInterlocutor = talk;
                mortal.stoppingDistance = talkingDistance;                
                targetPosition = currentInterlocutor.transform.position;
            }
        }

        if (currentInterlocutor == null)
            isLookingForInterlocutor = true;
        
        return true;
    }

    public override bool IsInRange(GameObject agent)
    {
        if (currentInterlocutor == null || isWaitingForInterlocutor) { /*print(targetPosition); mortal.followTarget = false; */return true; }
        else
        {
            targetPosition = currentInterlocutor.transform.position;
            float distance = (targetPosition - agent.transform.position).magnitude;                                               
            bool isInRange = distance <= talkingDistance;
            if (currentInterlocutor.isWaitingForInterlocutor && isInRange) currentInterlocutor.InterlocutorArrived();
            return isInRange;
        }
    }
   
    public override bool BeforePerform(GameObject agent)
    {
        isTalking = false;
        isListening = false;
        
        return true;
    }
    float timer = 0f;
    public override bool Perform(GameObject agent)
    {
        if (currentInterlocutor == null || isWaitingForInterlocutor) return true;
        else
        {
            Vector3 looAtInterlocutor = (currentInterlocutor.transform.position - agent.transform.position).normalized;
            agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, Quaternion.LookRotation(looAtInterlocutor), 5f);
            if (currentInterlocutor.isTalking)
            {
                //listen
                if (!isListening)
                {
                    isListening = true;
                    print("Listening " + gameObject.name);
                }
            }
            else
            {
                //speak
                if (!isTalking)
                {
                    isListening = false;
                    isTalking = true;
                    timer = 0f;
                    print("Talking " + gameObject.name);
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer >= 2f) isTalking = false;
                }
            }
        }
        return true;
    }

    public override bool AfterPerform(GameObject agent)
    {
        mortal.stoppingDistance = 0f;
        return true;
    }

    public override bool IsFinished(GameObject agent)
    {
        return false;
    }

    public override bool Abort(GameObject agent)
    {
        return true;
    }
}
