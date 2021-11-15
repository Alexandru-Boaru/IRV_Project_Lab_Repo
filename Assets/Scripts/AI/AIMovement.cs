using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : CharacterMotion
{
    [SerializeField] float attackRange = 5f;
    [SerializeField] float sightRange = 10f;
    [SerializeField] float patrolRange = 4f;
    [SerializeField] float patrolSpeedPercentage = 0.4f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Transform player;
    [SerializeField] NavMeshAgent agent;

    Vector3 targetPosition;
    Vector3 patrolPoint;
    bool playerInSight = false;
    bool playerInRange = false;

    void OnEnable()
    {
        targetPosition = transform.position;
        agent.updateRotation = false;
        SetPatrolPoint();
    }


    private void Update()
    {
        LookForPlayer();
        ApplyMovement();
        CheckGrounded();
    }
    void UpdatePatrolDestination()
    {
        Vector3 targetPositionAttempt = patrolPoint + new Vector3(Random.Range(-patrolRange, patrolRange), 0, Random.Range(-patrolRange, patrolRange));

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPositionAttempt, out hit, 1f, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
        }
        else
        {
            UpdatePatrolDestination();
        }
    }

    void SetPatrolPoint()
    {
        patrolPoint = transform.position;
    }


    public override void ApplyMovement()
    {
        // Get closer to target point until player is in range or until close to target point
        if ((playerInSight && !playerInRange))
        {
            agent.speed = moveSpeed;
            agent.destination = targetPosition;
        }
        else if (!playerInSight && Vector3.Distance(transform.position, targetPosition) > 1f)
        {
            agent.speed = moveSpeed * patrolSpeedPercentage;
            agent.destination = targetPosition;
        }
        else
        {
            //agent.destination = transform.position;
            UpdatePatrolDestination();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.InverseTransformPoint(player.position));
        Gizmos.DrawSphere(player.position, 0.5f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    void LookForPlayer()
    {
        RaycastHit firstHit;
        // player is in range and not behind obstacles
        if(Physics.Raycast(transform.position, transform.InverseTransformPoint(player.position), out firstHit, sightRange))
        {
            playerInSight = (1 << firstHit.transform.gameObject.layer == playerLayer.value);
        }
        else
        {
            playerInSight = false;
        }

        if (playerInSight)
        {
            targetPosition = player.position;
        }

        playerInRange = Physics.Raycast(transform.position, transform.InverseTransformPoint(player.position), attackRange, playerLayer.value);

    }

}
