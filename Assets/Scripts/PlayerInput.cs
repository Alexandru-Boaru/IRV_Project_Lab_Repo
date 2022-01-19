using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float forward;
    public float right;
    public float up;
    public bool mustJump;
    public bool mustRun;

    public bool shootOnce;
    public bool shootAuto;
    public bool mustRecharge;
    public bool nextWeapon;
    public bool prevWeapon;

    public bool vrEnabled;
    public bool inCart = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!vrEnabled)
        {
            if (!inCart)
            {
                forward = Input.GetAxis("Vertical");
                right = Input.GetAxis("Horizontal");
                up = Input.GetAxis("UpAxis");
                if (Input.GetKeyDown(KeyCode.Space))
                    mustJump = true;
                if (Input.GetKey(KeyCode.LeftShift))
                    mustRun = true;
                else
                    mustRun = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
                shootOnce = true;
            if (Input.GetKey(KeyCode.Mouse0))
                shootAuto = true;
            else
                shootAuto = false;
            if (Input.GetKeyDown(KeyCode.R))
                mustRecharge = true;
            if (Input.GetKeyDown(KeyCode.Q))
                prevWeapon = true;
            if (Input.GetKeyDown(KeyCode.E))
                nextWeapon = true;

        }
    }

    

    public void ResetJump()
    {
        mustJump = false;
    }
}
