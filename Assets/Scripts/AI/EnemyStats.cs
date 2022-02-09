using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    public int rewardPoints = 100;
    protected override void Die()
    {
        GameplayManager.instance.UpdateScore(rewardPoints);
        base.Die();
    }

}
