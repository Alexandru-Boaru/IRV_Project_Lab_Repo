using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CrosshairScaler : MonoBehaviour
{
    public PlayerShooter ps;
    public new Camera camera;
    public Vector3 origin;
    public Vector3 destination;
    public float radius;
    public RectTransform crosshair;
    // Update is called once per frame
    void Update()
    {
        origin = new Vector3(Screen.width / 2, Screen.height / 2) - camera.WorldToScreenPoint(ps.origin.position + ps.shootDir * ps.accuracyRingDistance);
        destination = new Vector3(Screen.width/2, Screen.height/2) - camera.WorldToScreenPoint(ps.origin.position + ps.shootDir * ps.accuracyRingDistance + ps.origin.up * ps.accuracyRingRadius);
        radius = (origin - destination).magnitude;
        //crosshair.localPosition = origin;
        crosshair.sizeDelta = new Vector2(radius*Mathf.Sqrt(2), radius * Mathf.Sqrt(2));
    }
}
