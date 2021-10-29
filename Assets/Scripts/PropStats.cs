using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropStats : EntityStats
{
    public GameObject brokenProp;
    public Rigidbody rigidB;
    // Start is called before the first frame update
    void Start()
    {
        rigidB = GetComponent<Rigidbody>();
    }

    protected override void Die()
    {
        GameObject go = Instantiate(brokenProp, transform.position, transform.rotation);
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb in rbs)
        {
            Debug.Log("Rie"+rigidB.velocity);
            rb.velocity = rigidB.velocity;
        }
        base.Die();
    }

}
