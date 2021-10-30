using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : CharacterMotion
{
    public PlayerInput input;
    public new Transform camera;
    public PhysicMaterial sliperryMaterial;
   

    // Start is called before the first frame update
    void Start()
    {
        speedMultipler = 1;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyMovement();
        CheckGrounded();
        if (input.mustJump)
            ApplyJump();
        input.ResetJump();
        moveDir = (input.right * camera.right + input.forward * camera.forward).normalized;
        
        moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized; // y e 0, miscare in xOz
                                                                            //transform.position += moveDir * Time.deltaTime * moveSpeed; // pentru non-rigidBody
                                                                            //moveDir *= Mathf.Abs(input.right + input.forward / 2);
        if ((input.right * camera.right + input.forward * camera.forward).magnitude < 0.95)
        {
            moveDir *= (input.right * camera.right + input.forward * camera.forward).magnitude;
        }
        if (input.mustRun)
            speedMultipler = 2;
        else
            speedMultipler = 1;
        
    }
}
