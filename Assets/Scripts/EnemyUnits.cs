using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyPair
{
    public EnemyType enemyType;
    public GameObject enemyPrefab;
}

[CreateAssetMenu(fileName = "EnemyUnits", menuName = "ScriptableObjects/EnemyUnits")]

public class EnemyUnits : ScriptableObject
{
    public List<EnemyPair> enemies;
}
