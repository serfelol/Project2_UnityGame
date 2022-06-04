using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineController))]
public class LineCollisions : MonoBehaviour
{
    #region fields
    // line builder manager
    LineController lc_ref;
    // the points that will make the polygon collider 2D
    List<Vector2> colliderPoints = new List<Vector2>();
    // reference to the collider used
    private PolygonCollider2D pc2D;
    // reference to the last point used to add collider
    private Vector2 lastPointAdded;
    private float widthLastPoint;
    #endregion

    private void Start()
    {
        if(!(lc_ref = GetComponent<LineController>())){
            Debug.Log("Line Controller not found");
        }
    }

    // Add a new segment to the polygoncollider
    public void NewCollisionSegment(Vector2 newPoint, float widthPoint)
    {
        // when collider don't exist yet
        if(lastPointAdded == null)
        {
            lastPointAdded = newPoint;
            widthLastPoint = widthPoint;
        }
        // create four colliders points because line is new
        else if(colliderPoints.Count == 0)
        {
            //m = (y2 -y1) / (x2 -x1)
            float m = (newPoint.y - lastPointAdded.y) / (newPoint.x - lastPointAdded.x);
            float deltaX_point0 = (widthLastPoint / 2f) * (m / Mathf.Pow(m * m + 1, 0.5f));
            float deltaY_point0 = (widthLastPoint / 2f) * (1 / Mathf.Pow(1 + m * 1, 0.5f));
            float deltaX_point1 = (widthPoint / 2f) * (m / Mathf.Pow(m * m + 1, 0.5f));
            float deltaY_point1 = (widthPoint / 2f) * (1 / Mathf.Pow(1 + m * 1, 0.5f));

            // calculate the offset of each point based by the width to line slope
            Vector2[] offSets = new Vector2[4];
            offSets[0] = new Vector3(-deltaX_point0, deltaY_point0);
            offSets[1] = new Vector3(deltaX_point0, -deltaY_point0);
            offSets[2] = new Vector3(-deltaX_point1, deltaY_point1);
            offSets[3] = new Vector3(deltaX_point1, -deltaY_point1);

            // generate the collider vertices
            colliderPoints.Add(lastPointAdded + offSets[0]);
            colliderPoints.Add(newPoint + offSets[2]);
            colliderPoints.Add(newPoint + offSets[3]);
            colliderPoints.Add(lastPointAdded + offSets[1]);

        }
        // after the first segment is made we only need to add the two collider for the new point
        else
        {
            //m = (y2 -y1) / (x2 -x1)
            float m = (newPoint.y - lastPointAdded.y) / (newPoint.x - lastPointAdded.x);
            float deltaX = (widthPoint / 2f) * (m / Mathf.Pow(m * m + 1, 0.5f));
            float deltaY = (widthPoint / 2f) * (1 / Mathf.Pow(1 + m * 1, 0.5f));

            // calculate the offset of each point based by the width to line slope
            Vector2[] offSets = new Vector2[2];
            offSets[0] = new Vector3(-deltaX, deltaY);
            offSets[1] = new Vector3(deltaX, -deltaY);

            // generate the collider vertices
            int listIndex = colliderPoints.Count / 2;
            colliderPoints.Insert(listIndex, newPoint + offSets[0]);
            colliderPoints.Insert(listIndex + 1, newPoint + offSets[1]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if (colliderPoints != null) colliderPoints.ForEach(p => Gizmos.DrawSphere(p, 0.1f));
    }

}
