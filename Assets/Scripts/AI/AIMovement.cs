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
    [SerializeField] float rotationSpeed = 3f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] public Transform player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] EnemyShooter shooter;
    [SerializeField] Transform body;
    [SerializeField] float shootingOffset = 1f;
    [SerializeField] Vector3 worldPos;
    public LayerMask VisibleLayers;

    Vector3 targetPosition;
    Vector3 patrolPoint;
    bool playerInSight = false;
    bool playerInRange = false;

    private Quaternion _lookRotation;
    private Vector3 _direction;

    void OnEnable()
    {
        targetPosition = transform.position;
        agent.updateRotation = false;
        SetPatrolPoint();
        shooter = GetComponentInChildren<EnemyShooter>();
    }


    private void Update()
    {
        worldPos = transform.position;
        if (player == null)
            player = PlayerStats.instance.gameObject.transform;
        if (player == null)
            return;
        LookForPlayer();
        

        if (playerInSight && playerInRange)
        {
            LookAtTarget(player.position);
            TryFire();
        }
        else
        {
            LookAtTarget(targetPosition);
            ApplyMovement();
        }
        CheckGrounded();
    }

    void LookAtTarget(Vector3 lookAt)
    {
        _direction = (lookAt - transform.position);
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        if (_direction != Vector3.zero)
        {
            //create the rotation we need to be in to look at the target
            _lookRotation = Quaternion.LookRotation(_direction);
            //rotate us over time according to speed until we are in the required rotation
            body.rotation = Quaternion.Slerp(body.rotation, _lookRotation, Time.deltaTime * rotationSpeed);
            //body.rotation = Quaternion.LookRotation(_direction);
        }

    }

    void UpdatePatrolDestination()
    {
        Vector3 targetPositionAttempt = patrolPoint + new Vector3(Random.Range(-patrolRange, patrolRange), 0, Random.Range(-patrolRange, patrolRange));

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPositionAttempt, out hit, 5f, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
        }
        else
        {
            //targetPosition = transform.position + transform.forward;
            UpdatePatrolDestination();
        }
    }

    void SetPatrolPoint()
    {
        patrolPoint = transform.position;
    }


    public override void ApplyMovement()
    {
        Vector2 transform2D = new Vector2 (transform.position.x, transform.position.z);
        Vector2 target2D = new Vector2(targetPosition.x, targetPosition.z);
        Vector2 player2D = new Vector2(player.position.x, player.position.z);

        // Get closer to target point:
        if ((!playerInSight && Vector2.Distance(transform2D, target2D) > 1f) || // If not seeing player
            (playerInSight && Vector2.Distance(transform2D, player2D) > attackRange - 1)) // If seeing player but not close enough to shoot
                                                                                                        // (also some extra distance to be able to keep shooting)
        {
            agent.speed = moveSpeed;
            agent.destination = targetPosition;
        }
        else if (!playerInSight)
        {
            //agent.destination = transform.position;
            agent.speed = moveSpeed * patrolSpeedPercentage;
            UpdatePatrolDestination();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.InverseTransformPoint(player.position));
        Gizmos.DrawSphere(targetPosition, 0.1f);
    }

    void LookForPlayer()
    {
        RaycastHit firstHit;
        // player is in range and not behind obstacles
        if(Physics.Raycast(transform.position, player.position-transform.position, out firstHit, sightRange, VisibleLayers))
        {
            playerInSight = (1 << firstHit.transform.gameObject.layer == playerLayer.value);
        }
        else
        {
            playerInSight = false;
        }

        

        playerInRange = Vector3.Distance(transform.position, player.position) < attackRange;

        if (playerInSight && playerInRange)
        {
            targetPosition = transform.position;
        }
        else if (playerInSight)
        {
            targetPosition = player.position;
        }
    }

    void TryFire()
    {
        agent.destination = transform.position; // Make character stop moving if player is in range

        // Fire logic
        // Shoot towards player + offset
        shooter.shootDir = player.position - shooter.transform.position + new Vector3 (Random.value * shootingOffset, Random.value * shootingOffset, Random.value * shootingOffset);
        shooter.Shoot();
        //shooter.Recharge();
    }

}
