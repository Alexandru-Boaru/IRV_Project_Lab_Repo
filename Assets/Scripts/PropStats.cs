using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropStats : EntityStats
{
    public GameObject brokenProp;
    public Rigidbody rigidB;

    void Start()
    {
        rigidB = GetComponent<Rigidbody>();
    }

    protected override void Die()
    {
        GameObject go = Instantiate(brokenProp, transform.position, transform.rotation) as GameObject;
        Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {   
            rb.velocity = rigidB.velocity;
            rb.GetComponent<PropExplosion>().explosionOrigin = transform.position;
        }
        base.Die();
    }

}
