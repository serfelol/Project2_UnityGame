using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// creates the lines based on points received;
/// </summary>
public class LineMaker : MonoBehaviour
{
    #region fields
    [SerializeField] private AnimationCurve ACVelocity;
    private float timePassedUntilLastKeyAdded;
    private float normalLineWidht = 0.4f;
    private float velocityThreshold;
    [SerializeField]private float currentLineVelocity;
    private float lastPointPlacedTime;
    private Vector2 lastPointPlacedPosition;

    //---------------------------------------------------------
    private bool wasCurveCreated;
    private float timePassedSinceCurveCreation;

    //---------------------------------------------------------
    private LineRenderer lineRenderer;
    private List<Vector2> points;
    [SerializeField] private float pointMinDistanceToAddLine;
    #endregion

    private void Awake() {
        if(!(lineRenderer = GetComponent<LineRenderer>())){
            Debug.Log("Line Renderer Component not found on object!");
        }
        wasCurveCreated = false;
    }

    private void Update() {
        // keep track of the time passed since line started to be created
        if(wasCurveCreated)
            timePassedSinceCurveCreation += Time.deltaTime;

    }

    /// <summary>
    /// add a new point to the list of points.
    /// </summary>
    /// <param name="point"></param>
    public void AddPointToLine(Vector2 point){
        // if line don't exist still
        if(points == null){
            points = new List<Vector2>();
            points.Add(point);
            DrawLine();

            // start the curve creation time count.
            wasCurveCreated = true;

                // save last point info for reference.
                lastPointPlacedPosition = points.Last();
                lastPointPlacedTime = Time.time;
        }
        else{
            // prevent points from beeing created too closed
            float distance = MathFormulas.DistanceBetweenTwoPoints(point.x, point.y, points[points.Count - 1].x, points[points.Count -1].y);
            if(distance >= pointMinDistanceToAddLine){
                points.Add(point);
                DrawLine();
                SetLineWidth();

                // save last point info for reference.
                lastPointPlacedPosition = points.Last();
                lastPointPlacedTime = Time.time;
            }
        }
    }

    /// <summary>
    /// set the points in the array to the lineRenderer Component.
    /// </summary>
    private void DrawLine(){
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, points.Last());
    }
    
    /// <summary>
    /// updates the line width based on the velocity of input written points.
    /// </summary>
    private void SetLineWidth(){
        GetCurrentLineVelocity();
        // calculate the animationCurve based on velocity.


        // sets calculated values to the lineRenderer component.
        //lineRenderer.widthCurve = ACVelocity;
    }

    private void GetCurrentLineVelocity(){
        // need reference to the position and time of last point placed on the line.
        // need the current placed point position and time.
        float currentTime = Time.time;
        // calculate the time that has passed
        float passedTime = currentTime - lastPointPlacedTime;
        // get the velocity
        currentLineVelocity = MathFormulas.VelocityBetweenTwoPoints2D(lastPointPlacedPosition, points.Last(), passedTime);
        Debug.Log("Last Position: time: " + lastPointPlacedTime + " position: " + lastPointPlacedPosition + "Current Point: time: " + currentTime + "position: " + points.Last());
    }

/*
                // distribution by distance and time
                if(animCurve.length ==0){
                    animCurve.AddKey(timePassedSinceCurveCreation, distance);
                    timePassedUntilLastKeyAdded = timePassedSinceCurveCreation;
                }
                else{
                    animCurve.AddKey((timePassedSinceCurveCreation - timePassedUntilLastKeyAdded), distance);
                }    

                // distribution by velocity
                float velocity = distance / timePassedUntilLastKeyAdded;
                animCurveVelocity.AddKey(animCurve.length, velocity);

                timePassedUntilLastKeyAdded = timePassedSinceCurveCreation;
            }   
        }
    }*/
}
