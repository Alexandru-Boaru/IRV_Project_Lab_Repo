using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBehaviour : CollectableBehaviour
{
    public int healPercent;
    public override void Collect()
    {
        PlayerStats ps = FindObjectOfType<PlayerStats>();
        ps.Heal(ps.maxHitPoints * healPercent / 100);
        base.Collect();
    }
}
