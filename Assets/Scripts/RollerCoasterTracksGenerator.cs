using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class RollerCoasterTracksGenerator : MonoBehaviour
{
    private BezierPath bezierPath;
    // Start is called before the first frame update
    void Start()
    {
        bezierPath = new BezierPath(Vector3.zero);
        for (int i = 0; i < 15; ++i) {
            if (Random.Range(0, 10) < 5)
                NonLoopTrackGenerator(Random.Range(7, 10), Random.Range(-3.5f, 3.5f));
            else if (Random.Range(0, 4) != 3) {
                CurvatureTrackGenerator(Random.Range(7, 10), Random.Range(0, 2), Random.Range(0, 2));
                NonLoopTrackGenerator(Random.Range(7, 10), 0);
            } else {

            }
        }
        bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;
        bezierPath.NotifyPathModified();
        gameObject.GetComponent<PathCreator>().bezierPath = bezierPath;
        gameObject.GetComponent<RoadMeshCreator>().TriggerUpdate();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void NonLoopTrackGenerator(int noPoints, float inclination) {
        Vector3 lastPoint = bezierPath.GetPoint(bezierPath.NumPoints - 1);
        for (int i = 0; i < noPoints; ++i) {
            lastPoint = new Vector3(
                lastPoint.x,
                lastPoint.y + inclination / noPoints,
                lastPoint.z + 1.0f
            );
            bezierPath.AddSegmentToEnd(lastPoint);
            bezierPath.NotifyPathModified();
        }
    }

    private void CurvatureTrackGenerator(int noPoints, float inclination, float curvature) {
        Vector3 lastPoint = bezierPath.GetPoint(bezierPath.NumPoints - 1);
        Vector3 lastPointCache = bezierPath.GetPoint(bezierPath.NumPoints - 1);
        inclination = Random.Range(3.0f, 5f) * (inclination == 0 ? -1 : 1);
        for (int i = 0; i < noPoints; ++i) {
            lastPoint = new Vector3(
                lastPointCache.x + noPoints * Mathf.Sin(Mathf.PI / 2 / noPoints * i) * (curvature == 0 ? -1 : 1),
                lastPoint.y + inclination / noPoints,
                lastPointCache.z + noPoints * Mathf.Cos(Mathf.PI / 2 / noPoints * i)
            );
            bezierPath.AddSegmentToEnd(lastPoint);
            bezierPath.NotifyPathModified();
        }
    }
}
