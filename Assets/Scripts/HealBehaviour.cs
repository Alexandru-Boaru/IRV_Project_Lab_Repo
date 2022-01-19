using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBehaviour : CollectableBehaviour
{
    public int healPercent;
    public override void Collect()
    {
        PlayerStats ps = FindObjectOfType<PlayerStats>();
        Debug.Log(ps.maxHitPoints * healPercent / 100);
        ps.Heal(ps.maxHitPoints * healPercent / 100);
        base.Collect();
    }
}
