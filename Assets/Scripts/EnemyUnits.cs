using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyPair
{
    public EnemyType enemyType;
    public GameObject enemyPrefab;
}

[System.Serializable]
public struct EnemyChance
{
    public float chance;
    public GameObject enemyRootPrefab;
}

[CreateAssetMenu(fileName = "EnemyUnits", menuName = "ScriptableObjects/EnemyUnits")]

public class EnemyUnits : ScriptableObject
{
    public List<EnemyPair> enemies;
    public List<GameObject> enemyRoots;
    public AnimationCurve enemyChanceCurve;

    public GameObject GetRandomEnemy()
    {
        float randVal = Random.Range(0f, 1f);
        return enemyRoots[Mathf.FloorToInt(Mathf.Clamp01(enemyChanceCurve.Evaluate(randVal)) * (enemyRoots.Count-1))];
    }
}
