using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner2 : MonoBehaviour
{
    Transform player;
    Color tBlue = new Color(0, 0, 1, 0.5f);
    Color tRed = new Color(1, 0, 0, 0.5f);

    List<GameObject> enemies;
    [SerializeField] LayerMask obstacleLayerMask;
    [SerializeField] int maxEnemies = 3;
    [SerializeField] float radius = 5;
    [SerializeField] GameObject enemyPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<GameObject>();
    }

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerMotion>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(enemies.Count);
        instantiateEnemy();
    }

    void instantiateEnemy()
    {
        if (enemies.Count < maxEnemies)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            AIMovement enemyMovement = newEnemy.GetComponent<AIMovement>();
            
            if (enemyMovement)
            {
                enemyMovement.player = player;
                enemies.Add(newEnemy);
            }
        }
    }

    // Get a random position around transform in a circle of given radius
    Vector3 randomPositionAroundCenter()
    {
        Vector2 randomPosition = Random.insideUnitCircle * radius;
        Vector3 newPosition = new Vector3(randomPosition.x, 0, randomPosition.y);

        return newPosition;
    }

    private void OnDrawGizmos()
    {
        if (Physics.CheckSphere(transform.position, radius, obstacleLayerMask))
        {
            Gizmos.color = tRed;
        }
        else
        {
            Gizmos.color = tBlue;
        }
        Gizmos.DrawSphere(transform.position, radius);
    }
}
