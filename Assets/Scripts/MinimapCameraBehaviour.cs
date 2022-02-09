using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraBehaviour : MonoBehaviour
{
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerStats.instance != null)
        {
            mainCamera = PlayerStats.instance.GetComponentInChildren<Camera>();
        }
    }

    private void LateUpdate() {
        if (mainCamera != null)
        {
            transform.rotation = Quaternion.Euler(90f, mainCamera.transform.rotation.eulerAngles.y, 0);
            transform.position = new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z);
        }
    }
}
