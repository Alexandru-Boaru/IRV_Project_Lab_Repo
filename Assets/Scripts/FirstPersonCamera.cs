using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float mouseSensitivity;
    public float cameraRotX;
    public float cameraRotY;
    public Transform playerBody;
    public Transform hands;
    public Vector3 cameraOffset;
    float xrot = 0f;

    public bool vrEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!vrEnabled)
        {
            PCfpc();
        }
    }

    private void LateUpdate()
    {
        transform.position = transform.parent.position + transform.TransformDirection(cameraOffset);
    }

    void PCfpc()
    {
        cameraRotX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        cameraRotY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xrot -= cameraRotY;
        xrot = Mathf.Clamp(xrot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xrot, 0f, 0f);
        playerBody.Rotate(Vector3.up * cameraRotX);
    }
}
