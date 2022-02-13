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
    private Quaternion targetRotation = Quaternion.identity;
    private RollerCoasterTracksGenerator rctg;
    private bool ready = false;
    public GameObject levelEnd;
    private float bumper = 8e-1f;
    private GameObject cubeLevelEnd;
    // Start is called before the first frame update
    void Start()
    {
        rctg = pathCreator.gameObject.GetComponent<RollerCoasterTracksGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ready) {
            if (rctg.CheckCompletion()) {
                ready = true;
                transform.position = pathCreator.path.GetPointAtDistance(0);
                transform.rotation = pathCreator.path.GetRotation(0);
                GameObject go = Instantiate(levelEnd,
                                            pathCreator.path.GetPointAtDistance(pathCreator.path.length - bumper),
                                            Quaternion.identity);
                go.transform.localScale *= 3;
                cubeLevelEnd = go.GetComponentInChildren<CardBehaviour>().gameObject;
            } else {
                return;
            }
        }
        if (cubeLevelEnd != null)
            cubeLevelEnd.transform.localPosition = Vector3.zero;
        distanceTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        Vector3 newPos = pathCreator.path.GetPointAtDistance(distanceTravelled + speed * Time.deltaTime);
        speed += (transform.position.y - newPos.y) * velocityDampening - friction * Time.deltaTime;
        speed = Mathf.Max(2.0f, speed);
        Vector3 lookVector = transform.position - pathCreator.path.GetPointAtDistance(distanceTravelled + speed * Time.deltaTime + 0.75f);
        if (targetRotation == Quaternion.identity || Mathf.Abs(Quaternion.Angle(targetRotation, transform.rotation)) < 0.1f) {
            if (lookVector != Vector3.zero) {
                targetRotation = Quaternion.LookRotation(lookVector, Vector3.up);
            }
        }
        if (Mathf.Abs(distanceTravelled - pathCreator.path.length) <= bumper) {
            LevelManager.instance.EndLevel();
            return;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * speed * Time.deltaTime);
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.gameObject.transform.position = transform.position + transform.up;
            PlayerStats.instance.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
