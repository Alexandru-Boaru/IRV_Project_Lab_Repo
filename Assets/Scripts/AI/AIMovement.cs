using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : CharacterMotion
{
    [SerializeField] float attackRange = 5f;
    [SerializeField] float sightRange = 10f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] AIPathfinding pathfinding;
    [SerializeField] Transform player;

    Vector3 targetPosition;
    bool playerInSight = false;
    bool playerInRange = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        targetPosition = transform.position;
    }

    

    // Update is called once per frame
    void Update()
    {
        LookForPlayer();
        pathfinding.FindPath(transform.position, targetPosition);

        // Get closer to target point until player is in range or until close to target point
        if((playerInSight && !playerInRange) || (!playerInSight && Vector3.Distance(transform.position, targetPosition) > 1f))
        {
            CalculateMoveDir();
            ApplyMovement();
        }

        CheckGrounded();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.InverseTransformPoint(player.position));
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

    //    Debug.Log("Player in sight: " + playerInSight + "   Player in range: " + playerInRange);
    }

    void CalculateMoveDir()
    {
        moveDir = new Vector3(pathfinding.nextStop.x, transform.position.y, pathfinding.nextStop.z); // Get world point of direction
        moveDir = transform.InverseTransformPoint(moveDir); // Convert to local
        moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized;
    }
}
