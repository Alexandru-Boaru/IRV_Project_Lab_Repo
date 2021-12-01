using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : CharacterShooter
{
    void Start()
    {
        origin = transform;
        gun = transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        origin = transform;
        if (fireRateCooldown <= 0)
        {
            Recharge();
        }
    }
}
