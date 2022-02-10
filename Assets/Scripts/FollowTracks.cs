using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class FollowTracks : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 5.0f;
    float distanceTravelled = 0;
    float velocityDampening = 3.5f;
    float friction = 3.0f;
    public bool gotPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        distanceTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        Vector3 newPos = pathCreator.path.GetPointAtDistance(distanceTravelled + speed * Time.deltaTime);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled) * Quaternion.Euler(0, 0, 90.0f);
        speed += (transform.position.y - newPos.y) * velocityDampening - friction * Time.deltaTime;
        speed = Mathf.Max(2.0f, speed);
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.gameObject.transform.position = transform.position + transform.up;
            PlayerStats.instance.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //PlayerStats.instance.gameObject.transform.SetParent(transform);
            //gotPlayer = true;
        }
    }
}
