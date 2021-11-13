using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public Transform root;
    }

    #region Singleton

    public static ObjectPooler Instance;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(pool.root);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
        if(pooledObject != null)
            pooledObject.OnObjectSpawn(position, rotation, parent);
        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

}
