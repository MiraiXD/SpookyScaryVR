using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentRootMotion : MonoBehaviour
{
    private NavMeshAgent agent;
    public bool applyRootMotion { get; set; } = false;
    public bool hasDestination { get; private set; } = false;
    public bool hasTargetRotation { get; private set; } = false;
    private bool destinationNear = false;
    public Vector3 destination { get; private set; }
    public Quaternion targetRotation { get; private set; }
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
        this.destination = destination;
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
            // rotates the agent toward its current steering target
            if (agent.steeringTarget != agent.transform.position)
            {
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation((agent.steeringTarget - agent.transform.position).normalized), agent.speed / 10f);                
            }

            if (!agent.pathPending)
            {
                if (!destinationNear && agent.remainingDistance <= 0.1f + stoppingDistance) // + agent.stoppingDistance)
                {
                    print("NEAR");
                    destinationNear = true;
                    onDestinationNear?.Invoke();
                }
                if (agent.remainingDistance <= stoppingDistance || agent.isStopped)
                {
                    if (!agent.isStopped) { agent.isStopped = true; agent.ResetPath(); agent.transform.position = new Vector3(destination.x, agent.transform.position.y, destination.z); }
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
        if (applyRootMotion)
        {
            transform.position += animator.deltaPosition;
            transform.rotation = animator.deltaRotation * transform.rotation;
        }
        if (hasDestination)
        {
            agent.speed = (animator.deltaPosition / Time.deltaTime).magnitude;
        }
    }
    public static bool RandomPointOnNavMesh(Vector3 origin, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = origin + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
