using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotion : MonoBehaviour
{
    public float moveSpeed;
    [Min(0)]
    public float speedMultipler;
    public Vector3 moveDir;
    public new Rigidbody rigidbody;
    public CapsuleCollider capsule;

    public float groundedThreshold;
    public bool grounded;
    public Vector3 groundOffset;

    public float jumpUpPower;
    public float jumpPower;

    public float slopeForce;
    public float slopeForceRayLength;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //capsule = GetComponent<CapsuleCollider>();
    }

    public void CheckGrounded()
    {
        Ray ray = new Ray();
        ray.direction = Vector3.down;
        Vector3 rayOrigin = transform.position + groundedThreshold * Vector3.up + groundOffset;
        grounded = false;
        for (float xOffset = -1f; xOffset <= 1f; xOffset += 1f)
        {
            for (float zOffset = -1f; zOffset <= 1f; zOffset += 1f)
            {
                Vector3 offset = new Vector3(xOffset, 0f, zOffset).normalized * capsule.radius;
                ray.origin = rayOrigin + offset;
                if (Physics.Raycast(ray, 2f * groundedThreshold))
                {
                    grounded = true;
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f * groundedThreshold, Color.green);
                }
                else
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f * groundedThreshold, Color.red);
            }
        }
        //animator.SetBool("Grounded", grounded);
    }

    public void ApplyMovement()
    {
        Vector3 finalMoveDir = moveDir * moveSpeed * speedMultipler;
        if (moveDir!=Vector3.zero && rigidbody.velocity.y<0 && OnSlope())
        {
            finalMoveDir += Vector3.down * slopeForce;
        }
        /*
        rigidbody.velocity = moveDir * moveSpeed * Time.deltaTime;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY,
                                         rigidbody.velocity.z);
                                         */
        float velY = rigidbody.velocity.y;

        rigidbody.MovePosition(transform.position + finalMoveDir);
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY,
                                         rigidbody.velocity.z);

    }

    public void ApplyJump()
    {
        if (!grounded)
            return;
        Vector3 jumpDir = (moveDir + Vector3.up * jumpUpPower).normalized;
        rigidbody.AddForce(jumpDir * jumpPower, ForceMode.VelocityChange);
    }

    public void OnDrawGizmos()
    {
        capsule = GetComponent<CapsuleCollider>();
        Ray ray = new Ray();
        ray.direction = Vector3.down;
        Vector3 rayOrigin = transform.position + groundedThreshold * Vector3.up + groundOffset;
        for (float xOffset = -1f; xOffset <= 1f; xOffset += 1f)
        {
            for (float zOffset = -1f; zOffset <= 1f; zOffset += 1f)
            {
                Vector3 offset = new Vector3(xOffset, 0f, zOffset).normalized * capsule.radius;

                ray.origin = rayOrigin + offset;
                Gizmos.DrawSphere(ray.origin, 0.1f);
                Gizmos.DrawLine(ray.origin, ray.origin + Vector3.down * 2f * groundedThreshold);
                /*
                if (Physics.Raycast(ray, 2f * groundedThreshold))
                {
                    grounded = true;
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f * groundedThreshold, Color.green);
                }
                else
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f * groundedThreshold, Color.red);
                    */
            }
        }
        Gizmos.color = Color.blue;
        rayOrigin = transform.position + slopeForceRayLength * Vector3.up + groundOffset;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * 2f * slopeForceRayLength);
    }

    public bool OnSlope()
    {
        if (!grounded)
            return false;
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + slopeForceRayLength * Vector3.up + groundOffset;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 2f * slopeForceRayLength))
        {
            if (hit.normal != Vector3.up)
                return true;
        }
        return false;
    }

}
