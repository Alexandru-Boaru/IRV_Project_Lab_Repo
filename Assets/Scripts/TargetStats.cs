using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Rewards
 * 
 * 0 None
 * 1 Ammo
 * 2 Ammo
 * 3 Ammo
 * 4 10 hp
 * 5 24 hp
 * 6 50 hp
 * 7 100 hp
 */

public enum TargetReward {NONE, SG_AMMO, MG_AMMO, R_AMMO, SMALL_HP, QUARTER_HP, HALF_HP, FULL_HP}

public class TargetStats : EntityStats
{
    public int scoreReward;
    public AnimationCurve rewardChance;
    public int numberOfRewards = 7;
    protected override void Die()
    {
        GameplayManager.instance.UpdateScore(scoreReward);
        GetRandomReward();
        base.Die();
    }

    public void GetRandomReward()
    {
        float randVal = Random.Range(0f, 1f);
        TargetReward rewardID = (TargetReward)Mathf.FloorToInt(Mathf.Clamp01(rewardChance.Evaluate(randVal)) * numberOfRewards);
        if (rewardID == TargetReward.NONE)
            return;
        PlayerStats pst = FindObjectOfType<PlayerStats>();
        PlayerShooter psh = FindObjectOfType<PlayerShooter>();

        switch(rewardID)
        {
            case TargetReward.SG_AMMO:
                psh.AddAmmo(1, 5);
                break;
            case TargetReward.MG_AMMO:
                psh.AddAmmo(2, 50);
                break;
            case TargetReward.R_AMMO:
                psh.AddAmmo(3, 6);
                break;
            case TargetReward.SMALL_HP:
                pst.Heal(10 * pst.maxHitPoints / 100);
                break;
            case TargetReward.QUARTER_HP:
                pst.Heal(25 * pst.maxHitPoints / 100);
                break;
            case TargetReward.HALF_HP:
                pst.Heal(50 * pst.maxHitPoints / 100);
                break;
            case TargetReward.FULL_HP:
                pst.Heal(maxHitPoints);
                break;
        }
    }
}
