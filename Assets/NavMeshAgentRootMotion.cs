using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentRootMotion : MonoBehaviour
{
    private NavMeshAgent agent;
    public bool applyRootMotion { get; set; } = false;
    public bool hasDestination { get; private set;} = false;
    public bool hasTargetRotation { get; private set; } = false;
    private bool destinationNear = false;
    public Vector3 destination { get; private set; }
    public Quaternion targetRotation { get;private set; }
    public float stoppingDistance { get; set; }
    public float speed { get => agent.speed; }
    public System.Action onDestinationReached, onDestinationNear, onDepart, onStopMoving;    
    private void Start()
    {        
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = true;
        agent.updateRotation = false;
        agent.acceleration = 99999f;
    }
    public void SetDestination(Vector3 destination, float stoppingDistance = 0f)
    {
        hasDestination = true;
        destinationNear = false;
        this.stoppingDistance = stoppingDistance;
        //agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(destination);
        onDepart?.Invoke();        
    }   
    public void SetDestinationAndTargetRotation(Vector3 destination, Quaternion targetRotation, float stoppingDistance = 0f)
    {
        hasTargetRotation = true;
        this.targetRotation = targetRotation;
        SetDestination(destination, stoppingDistance);                
    }
    public void Stop()
    {
        hasDestination = false;
        agent.isStopped = true;
        agent.ResetPath();
        onStopMoving?.Invoke();
    }
    private void Update()
    {        
        if (hasDestination)
        {            
            if (agent.steeringTarget != agent.transform.position)
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation((agent.steeringTarget - agent.transform.position).normalized), agent.speed/10f);

            if (!agent.pathPending)
            {
                if (!destinationNear && agent.remainingDistance <= 0.1f + stoppingDistance) // + agent.stoppingDistance)
                {
                    destinationNear = true;
                    onDestinationNear?.Invoke();                    
                }
                if(agent.remainingDistance <= stoppingDistance)
                {
                    if (!agent.isStopped) { agent.isStopped = true; agent.ResetPath(); }
                    if (!hasTargetRotation)
                    {
                        hasDestination = false;
                        onDestinationReached?.Invoke();                        
                    }
                    else
                    {
                        if (agent.transform.rotation != targetRotation) agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, 5f); 
                        else hasTargetRotation = false;
                    }
                }
            }
        }        
    }
    public void AnimatorMove(Animator animator)
    {        
        if(applyRootMotion)
        {
            transform.position += animator.deltaPosition;
            transform.rotation = animator.deltaRotation * transform.rotation;
        }
        if (hasDestination)
        {
            agent.speed = (animator.deltaPosition / Time.deltaTime).magnitude;
        }
    }
    public static Vector3 GetRandomPositionOnNavMesh(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }
}
