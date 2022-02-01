using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : CharacterShooter
{
    protected override void Start()
    {
        base.Start();
        origin = transform;
        gun = transform;
        muzzlePoint = transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        origin = transform;
    }
}
