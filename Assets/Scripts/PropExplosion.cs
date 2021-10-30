using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropExplosion : MonoBehaviour
{
    public float explosionForce;
    public float explosionRadius;
    public Vector3 explosionOrigin;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionOrigin, explosionRadius);
    }
}
