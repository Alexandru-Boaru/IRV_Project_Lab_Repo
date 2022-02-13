using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class RollerCoasterTracksGenerator : MonoBehaviour
{
    private BezierPath bezierPath;
    // Start is called before the first frame update
    private bool loaded = false;
    void Start()
    {
        bezierPath = new BezierPath(Vector3.zero);
        bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;
        bezierPath.AutoControlLength = 0;
        bezierPath.NotifyPathModified();
        StraightTracks();
        for (int i = 0; i < 15; ++i) {
            if (Random.Range(0, 10) < 5)
                StraightTracks();
            else if (Random.Range(0, 4) < 2) {
                ElevationChange();
                StraightTracks();
            } else {
                SideLoop();
                StraightTracks();
                StraightTracks();
            }
        }
        StraightTracks();
        bezierPath.ResetNormalAngles();
        bezierPath.DeleteSegment(0);
        bezierPath.NotifyPathModified();
        gameObject.GetComponent<PathCreator>().bezierPath = bezierPath;
        gameObject.GetComponent<RoadMeshCreator>().TriggerUpdate();
        loaded = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void StraightTracks() {
        int noPoints = Random.Range(3, 5);
        Vector3 lastPoint = bezierPath.GetPoint(bezierPath.NumPoints - 1);
        for (int i = 0; i < noPoints; ++i) {
            lastPoint = new Vector3(
                lastPoint.x,
                lastPoint.y,
                lastPoint.z + 1
            );
            bezierPath.AddSegmentToEnd(lastPoint);
            bezierPath.NotifyPathModified();
        }
    }

    /*

    private void ElevationChange() {
        Vector3 lastPoint = bezierPath.GetPoint(bezierPath.NumPoints - 1);
        float elevation = Random.Range(2.5f, 3.5f) * (Random.Range(0, 2) == 0 ? -1 : 1);
        int noPointsRamp = 10;
        for (int j = 0; j < 2; ++j) {
            for (int i = 0; i < noPointsRamp; ++i) {
                lastPoint = new Vector3(
                    lastPoint.x,
                    lastPoint.y + elevation / noPointsRamp * (j == 0 ? 1 : -1),
                    lastPoint.z + 0.2f
                );
                bezierPath.AddSegmentToEnd(lastPoint);
                bezierPath.NotifyPathModified();
            }
            for (int i = 0; i < 2; ++i) {
                lastPoint = new Vector3(
                    lastPoint.x,
                    lastPoint.y,
                    lastPoint.z + 1
                );
                bezierPath.AddSegmentToEnd(lastPoint);
                bezierPath.NotifyPathModified();
            }
        }
    }

    */

    private void ElevationChange() {
        Vector3 lastPoint = bezierPath.GetPoint(bezierPath.NumPoints - 1);
        Vector3 lastPointCache = bezierPath.GetPoint(bezierPath.NumPoints - 1);
        float elevation = Random.Range(2.5f, 3.5f) * (Random.Range(0, 2) == 0 ? -1 : 1);
        int noPointsRamp = 50;
        for (int j = 0; j < 2; ++j) {
            for (int i = 0; i <= noPointsRamp; ++i) {
                lastPoint = new Vector3(
                    lastPoint.x,
                    lastPointCache.y + elevation * Mathf.Sin(Mathf.PI / 2 * i / noPointsRamp) * (j == 0 ? 1 : -1),
                    lastPointCache.z + 3.0f * i / noPointsRamp
                );
                bezierPath.AddSegmentToEnd(lastPoint);
                bezierPath.NotifyPathModified();
            }
            for (int i = 0; i < 2; ++i) {
                lastPoint = new Vector3(
                    lastPoint.x,
                    lastPoint.y,
                    lastPoint.z + 1
                );
                bezierPath.AddSegmentToEnd(lastPoint);
                bezierPath.NotifyPathModified();
            }
            lastPointCache = lastPoint;
        }
    }

    private void SideLoop() {
        Vector3 lastPoint = bezierPath.GetPoint(bezierPath.NumPoints - 1);
        Vector3 lastPointCache = bezierPath.GetPoint(bezierPath.NumPoints - 1);
        int noPointsCircle = 50;
        float elevation = Random.Range(2.5f, 3.0f) * (Random.Range(0, 2) == 0 ? -1 : 1);
        float offset = Random.Range(2.0f, 3.0f);
        int changeOnXAxis = Random.Range(0, 2) == 0 ? -1 : 1;
        for (int i = 0; i <= noPointsCircle; ++i) {
            lastPoint = new Vector3(
                lastPointCache.x + offset * Mathf.Sin(2f * Mathf.PI * i / noPointsCircle) * changeOnXAxis,
                lastPoint.y + elevation / (noPointsCircle + 1),
                lastPointCache.z + offset * Mathf.Cos(2f * Mathf.PI * i / noPointsCircle)
            );
            bezierPath.AddSegmentToEnd(lastPoint);
            bezierPath.NotifyPathModified();
        }
    }

    public bool CheckCompletion() {
        return loaded;
    }
}
