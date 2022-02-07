using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnTrigger : MonoBehaviour
{
    public List<EnemySpawner> spawners = new List<EnemySpawner>();
    public bool spawned = false;
    void Start()
    {
        spawners = new List<EnemySpawner>(GetComponentsInChildren<EnemySpawner>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!spawned)
            {
                foreach (EnemySpawner es in spawners)
                {
                    es.Spawn();
                }
                spawned = true;
            }
            else
            {
                foreach (EnemySpawner es in spawners)
                {
                    es.Activate(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Debug.Log("gosh");
            foreach (EnemySpawner es in spawners)
            {
                es.Activate(false);
            }
        }
    }
}
