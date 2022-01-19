using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropBehaviour : CollectableBehaviour
{
    public int weaponId;
    public int ammo;

    public override void Collect()
    {
        PlayerShooter ps = FindObjectOfType<PlayerShooter>();
        ps.AddAmmo(weaponId, ammo);
        base.Collect();
    }
}
