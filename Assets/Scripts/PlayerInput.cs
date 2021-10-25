using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float forward;
    public float right;
    public bool mustJump;
    public bool mustRun;

    public bool vrEnabled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!vrEnabled)
        {
            forward = Input.GetAxis("Vertical");
            right = Input.GetAxis("Horizontal");
            if (Input.GetKeyDown(KeyCode.Space))
                mustJump = true;
            if (Input.GetKey(KeyCode.LeftShift))
                mustRun = true;
            else
                mustRun = false;
        }
    }

    

    public void ResetJump()
    {
        mustJump = false;
    }
}
