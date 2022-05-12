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

    //----------------line width-----------------------------
    [SerializeField] private AnimationCurve curveWidthKeys_updated;
    private AnimationCurve curveWidthKeys_original;
    [SerializeField]private float LastKeyPointVelocity;
    private List<float> timePassedSinceLastKeyAdded;
    private float normalLineWidht = 0.4f;
    [SerializeField] private float AC_WidthMargin = 0.02f;
    private float LW_ByVelocityMin = 5.0f;
    private float LW_ByVelocityMax = 10.0f;

    //----------------velocity calculation---------------------
    [SerializeField]private float currentLineVelocity;
    private float lastPointTime;
    private Vector2 lastPointPosition;

    //-----------------curve check-----------------------------
    private bool wasCurveCreated;
    private float timePassedSinceCurveCreation;
    private float curvedistance;

    //------------------curve creation-------------------------
    private LineRenderer lineRenderer;
    [SerializeField] private float pointMinDistanceToAddLine;
    #endregion

    private void Awake() {
        if(!(lineRenderer = GetComponent<LineRenderer>())){
            Debug.Log("Line Renderer Component not found on object!");
        }
        else{
            lineRenderer.positionCount = 0;
        }

        wasCurveCreated = false;
        curveWidthKeys_updated = new AnimationCurve();
        curveWidthKeys_original = new AnimationCurve();
        timePassedSinceLastKeyAdded = new List<float>();
        LastKeyPointVelocity = -5; // setting this for validation purpose in the first interaction.
        
    }

    private void Update() {
        // keep track of the time passed since line started to be created
        if(wasCurveCreated)
            timePassedSinceCurveCreation += Time.deltaTime; 
        
        //UpdateForDebug();
    }

    /// <summary>
    /// Add a new point to the list of points that make up the line.
    /// </summary>
    /// <param name="point"></param>
    public void AddPointToLine(Vector2 point){
        // if line don't exist still
        if(lineRenderer.positionCount  == 0){
            AddPointToTheLineRenderer(point);

            // start the curve creation time count
            wasCurveCreated = true;

            // save last point info for reference
            lastPointPosition = point;
            lastPointTime = timePassedSinceCurveCreation;
        }
        else{
            // do not allow two points to be created too close together
            float distance = MathFormulas.DistanceBetweenTwoPoints(point.x, point.y, lastPointPosition.x, lastPointPosition.y);
            if(distance >= pointMinDistanceToAddLine){
                UpdateCurveDistance(point);
                AddPointToTheLineRenderer(point);
                SetCurveWidth();
                UpdateCurveWidth();

                // save last point info for reference
                lastPointPosition = point;
                lastPointTime = timePassedSinceCurveCreation;
            }
        }
    }

    /// <summary>
    /// Add a selected point to the lineRenderer component.
    /// </summary>
    private void AddPointToTheLineRenderer(Vector2 point){
        lineRenderer.positionCount += 1;
        lineRenderer.SetPosition(lineRenderer.positionCount -1, point);
    }
    private void UpdateCurveDistance(Vector2 point){
        var distance = MathFormulas.DistanceBetweenTwoPoints(point.x, point.y, lastPointPosition.x, lastPointPosition.y);
        curvedistance += distance;
    }
    
    /// <summary>
    /// Creates a new keyframe that represent a change in the curve width having as reference the velocity.
    /// The Animation Curve holding this keyframes is 'VelocityCurveChanges_keys'.
    /// </summary>
    private void SetCurveWidth(){

        float keyPointWidth;
        float interpolatValue;
        Keyframe key;

        // get the current velocity
        CalculateLineDrawingSpeed();

        // calculate the line width by the velocity interpolation
        interpolatValue = Mathf.InverseLerp(LW_ByVelocityMin, LW_ByVelocityMax, currentLineVelocity);
        keyPointWidth = Mathf.Lerp(normalLineWidht, 0, interpolatValue);

        // register the key that represents the beginning of the curve
        if(curveWidthKeys_original.length == 0){
            key  = new Keyframe(0.0f, keyPointWidth);
            curveWidthKeys_original.AddKey(key);
        }
        
        // register the new keypoint
        key = new Keyframe(curvedistance, keyPointWidth);
        curveWidthKeys_original.AddKey(key);

        // updates the LastKeyVelocity reference            
        LastKeyPointVelocity = currentLineVelocity; 
    }
    
    /// <summary>
    /// Calculates the speed at which the line is being drawn and saves it on 'currentLineVelocity' variable.
    /// </summary>
    private void CalculateLineDrawingSpeed(){
        // need the current placed point position and time.
        float currentPointTime = Time.time;
        Vector2 currentPointPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        // calculate the time that has passed
        float passedTime = currentPointTime - lastPointTime;
        // need reference to the position and time of last point placed on the line.
        currentLineVelocity = MathFormulas.VelocityBetweenTwoPoints2D(lastPointPosition, currentPointPosition, passedTime);
    }

    /// <summary>
    /// Updates the 'time' value on each keyframes to follow the growth of the curve.
    /// Reads from 'curveWidthKeys_original' and saves in 'curveWidthKeys_updated'.
    /// </summary>
    private void UpdateCurveWidth(){
        // list that will be filled with the new values for the keys
        curveWidthKeys_updated = new AnimationCurve();

        // populate the list with the updated values
        for(int i = 0; i < curveWidthKeys_original.length; i++){
            // interpolates the distance for values between 0 and 1
            float keyframedistance = curveWidthKeys_original[i].time / curvedistance;

            Keyframe key = curveWidthKeys_original[i];
            key.time = keyframedistance;
            key.weightedMode = WeightedMode.None;
            curveWidthKeys_updated.AddKey(key);
        }

        // clear keyframes without relevance for the AC
        // keys between keys with similar width are removed
        if(curveWidthKeys_updated.length >= 3){
            bool condition1 = Mathf.Abs(curveWidthKeys_updated[curveWidthKeys_updated.length - 1].value - curveWidthKeys_updated[curveWidthKeys_updated.length - 2].value) < AC_WidthMargin;
            if(condition1){
                bool condition2 = Mathf.Abs(curveWidthKeys_updated[curveWidthKeys_updated.length - 1].value - curveWidthKeys_updated[curveWidthKeys_updated.length - 3].value) < AC_WidthMargin;
                if(condition2){
                    var positionToRemove = curveWidthKeys_updated.length - 2;
                    curveWidthKeys_updated.RemoveKey(positionToRemove);
                    curveWidthKeys_original.RemoveKey(positionToRemove);
                }
            }
        }
        
        // assigning the calculated values to the LR component
        lineRenderer.widthCurve = curveWidthKeys_updated;
    }
    private void UpdateForDebug(){
        lineRenderer.widthCurve = curveWidthKeys_updated;
    }
}
