using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : CharacterShooter
{
    public new Transform camera;
    public float angle;
    public PlayerInput input;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        origin = camera;
        shootDir = camera.forward;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        shootDir = camera.forward;
        if (autoFire && input.shootAuto)
        {
            Shoot();
        }
        else if(!autoFire && input.shootOnce)
        {
            Shoot();
            input.shootOnce = false;
        }

        if (input.mustRecharge)
        {
            Recharge();
            input.mustRecharge = false;
        }
    }

    protected override void OnDrawGizmos()
    {
        origin = camera;
        shootDir = camera.forward;
        base.OnDrawGizmos();
    }
}
