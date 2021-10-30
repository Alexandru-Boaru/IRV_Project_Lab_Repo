using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropStats : EntityStats
{
    public GameObject brokenProp;
    public Rigidbody rigidB;
    //public Vector3 impactDirection;
    // Start is called before the first frame update
    void Start()
    {
        rigidB = GetComponent<Rigidbody>();
    }

    protected override void Die()
    {
        GameObject go = Instantiate(brokenProp, transform.position, transform.rotation) as GameObject;
        Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();
        Debug.Log("Rie" + transform.position + " " + rbs.Length);
        foreach (Rigidbody rb in rbs)
        {   
            rb.velocity = rigidB.velocity;
            //rb.AddExplosionForce(10, transform.position, 1);
            //Debug.Log(rb.gameObject.name);
            rb.GetComponent<PropExplosion>().explosionOrigin = transform.position;
            //Debug.Log(rb.GetComponent<PropExplosion>().explosionOrigin);
        }
        base.Die();
    }

}
