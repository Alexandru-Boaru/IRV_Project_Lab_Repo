using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { BASIC, RANDOM}

public class EnemySpawner : MonoBehaviour
{
    public EnemyType enemyType;
    public EnemyUnits enemyUnits;

    public GameObject spawnedEnemy;
 
    // Start is called before the first frame update
    public void Spawn()
    {
        GameObject pref;
        if (enemyType == EnemyType.RANDOM)
        {
            pref = enemyUnits.enemies[Random.Range(0, enemyUnits.enemies.Count)].enemyPrefab;
        }
        else
        {
            pref = enemyUnits.enemies.Find(x => (x.enemyType == enemyType)).enemyPrefab;
        }
        spawnedEnemy = Instantiate(pref, transform);
    }

    public void Activate(bool state)
    {
        if (spawnedEnemy != null)
        {
            //Debug.Log(state);
            spawnedEnemy.SetActive(state);
        }
    }
}
