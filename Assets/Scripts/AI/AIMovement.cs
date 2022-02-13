using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : CharacterMotion
{
    [SerializeField] EnemyStats stats;
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
    public float resetPatrolTime = 10f;
    public float patrolTime = 0f;
    public float level = 1;
    [SerializeField] float movementSpeedIncrease = 0.1f;
    [SerializeField] float attackSpeedIncrease = 0.1f;
    [SerializeField] float attackDamageIncrease = 0.25f;
    [SerializeField] float sightRangeIncrease = 0.2f;
    [SerializeField] float attackRangeIncrease = 0.2f;
    [SerializeField] float healthIncrease = 0.2f;

    Vector3 targetPosition;
    Vector3 patrolPoint;
    bool playerInSight = false;
    bool playerInRange = false;

    private Quaternion _lookRotation;
    private Vector3 _direction;

    public AudioController audioController;

    LevelManager manager;

    void OnEnable()
    {
        targetPosition = transform.position;
        agent.updateRotation = false;
        SetPatrolPoint();
        shooter = GetComponentInChildren<EnemyShooter>();
        if (audioController == null)
        {
            audioController = GetComponentInChildren<AudioController>();
        }

        manager = FindObjectOfType<LevelManager>();
        if (manager)
        {
            level = manager.currentLevel - 1;
        }
    }


    void Start()
    {
        // Update stats based on level
        shooter.fireRate = Mathf.Clamp(shooter.fireRate - shooter.fireRate * level * attackSpeedIncrease, 0.3f, shooter.fireRate);
        shooter.damage += (int)(shooter.damage * level * attackDamageIncrease);
        sightRange += sightRange * level * sightRangeIncrease;
        attackRange += attackRange * level * attackRangeIncrease;
        stats.maxHitPoints += (int)(stats.hitPoints * level * healthIncrease);
        stats.hitPoints = stats.maxHitPoints;
        //rigidbody = GetComponent<Rigidbody>();
        //if (floatingCharacter)
        //    rigidbody.useGravity = false;
        //StartCoroutine(OffMeshMovement());
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
        if (patrolTime > 0)
            patrolTime -= Time.deltaTime;
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
            //UpdatePatrolDestination();
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
        if ((!playerInSight && (Vector2.Distance(transform2D, target2D) > 1f) || patrolTime <= 0) || // If not seeing player
            (playerInSight && Vector2.Distance(transform2D, player2D) > attackRange - 1)) // If seeing player but not close enough to shoot
                                                                                                        // (also some extra distance to be able to keep shooting)
        {
            agent.speed = moveSpeed + moveSpeed * level * movementSpeedIncrease;
            agent.destination = targetPosition;
            patrolTime = resetPatrolTime;
        }
        else if (!playerInSight)
        {
            //agent.destination = transform.position;
            agent.speed = moveSpeed * patrolSpeedPercentage;
            patrolTime = resetPatrolTime;
            UpdatePatrolDestination();
        }
    }

    /*
    IEnumerator OffMeshMovement()
    {
        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                yield return StartCoroutine(NormalSpeed(agent));
            }
            yield return null;
        }
    }

    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }
    */
    private void OnDrawGizmos()
    {
        //Gizmos.DrawRay(transform.position, transform.InverseTransformPoint(player.position));
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
        if (shooter.fireRateCooldown < 0)
        {
            if (audioController) audioController.Play("P_Shoot");
        }
        shooter.Shoot();
        //shooter.Recharge();
    }

}
