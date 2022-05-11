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
    private float AL_KeyPointVelocityMargin = 1.0f;
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
    private List<Vector2> points;
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
        // get the current velocity
        CalculateLineDrawingSpeed();

        float keyPointWidth;
        float interpolatValue;
        
        // for the first key added
        if(curveWidthKeys_original.length == 0){
            // calculate the line width by the velocity interpolation
            interpolatValue = Mathf.InverseLerp(LW_ByVelocityMin, LW_ByVelocityMax, currentLineVelocity);
            keyPointWidth = Mathf.Lerp(normalLineWidht, 0, interpolatValue);

            // register the key that represents the begining of the curve
            Keyframe key = new Keyframe(0.0f, keyPointWidth);
            curveWidthKeys_original.AddKey(key);

            // register at witch time this key point was created
            key = new Keyframe(curvedistance, keyPointWidth);
            curveWidthKeys_original.AddKey(key);

            // updates the LastKeyVelocity reference            
            LastKeyPointVelocity = currentLineVelocity;
        }
        // only add a new key point to the animation line if velocity change is above a min value
        // to cover the first iteraction the variable AL_LastKeyPointVelocity was set to -5. So it's always true
        else if(Mathf.Abs(LastKeyPointVelocity - currentLineVelocity) > AL_KeyPointVelocityMargin){
            // key width value calculation conditions.
            if(currentLineVelocity < LW_ByVelocityMin && LastKeyPointVelocity > LW_ByVelocityMin){
                keyPointWidth = normalLineWidht;
            }
            else if (currentLineVelocity > LW_ByVelocityMax && LastKeyPointVelocity <LW_ByVelocityMax){
                keyPointWidth = 0;
            }
            else if(currentLineVelocity > LW_ByVelocityMin && currentLineVelocity < LW_ByVelocityMax){
                interpolatValue = Mathf.InverseLerp(LW_ByVelocityMin, LW_ByVelocityMax, currentLineVelocity);
                keyPointWidth = Mathf.Lerp(normalLineWidht, 0, interpolatValue);
            }
            // if there is a change in velocity above margin but it's still in the range of < LW_ByVelocityMin 
            // or > LW_ByVelocityMax in comparation with last key added, then there is no need to create
            // another key, because the width value will be the same.
            else
                return;

            // register at witch time this key point was created.
            Keyframe key = new Keyframe(curvedistance, keyPointWidth);
            curveWidthKeys_original.AddKey(key);

            // updates the LastKeyVelocity reference  
            LastKeyPointVelocity = currentLineVelocity;
        }
    }
    
    /// <summary>
    /// Sets the current velocity that the line is being drawn on 'currentLineVelocity' variable.
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
    /// Updates the time value in the keyframes to mantain the correct position of the width variation with the addiction of the curve.
    /// Reads from 'curveWidthKeys_original' and saves in 'curveWidthKeys_updated'.
    /// </summary>
    private void UpdateCurveWidth(){
        curveWidthKeys_updated = new AnimationCurve();

        for(int i = 0; i < curveWidthKeys_original.length; i++){
            // calculates the time for values between 0 and 1. Required for the lineRenderer.widthCurve format
            float keyframeTime = curveWidthKeys_original[i].time / curvedistance;
            // sets the curve animation b
            Keyframe key = curveWidthKeys_original[i];
            key.time = keyframeTime;
            key.weightedMode = WeightedMode.None;
            // key.weightedMode = WeightedMode.In;
            curveWidthKeys_updated.AddKey(key);
        }

        if(curveWidthKeys_updated.length > 0)
            lineRenderer.widthCurve = curveWidthKeys_updated;
    }
    private void UpdateForDebug(){
        lineRenderer.widthCurve = curveWidthKeys_updated;
    }
}
