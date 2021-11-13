using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooledObject
{
    void OnObjectSpawn(Vector3 position, Quaternion rotation, Transform parent = null);
}
