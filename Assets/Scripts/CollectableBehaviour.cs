using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehaviour : MonoBehaviour
{
    public float rotationSpeed = 100.0f;
    public bool collected = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.layer + " " + LayerMask.NameToLayer("Player"));
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !collected)
        {
            collected = true;
            Collect();
        }
    }

    public virtual void Collect()
    {
        Destroy(gameObject);
    }

}
