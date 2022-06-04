using UnityEngine;

/// <summary>
/// create a line based on points received;
/// </summary>
public class LineMaker : MonoBehaviour
{
    #region fields
    // line state
    private bool isLineActive;

    //----------------line width-----------------------------
    [SerializeField] private AnimationCurve curveWidthKeys_updated;
    private AnimationCurve curveWidthKeys_original;
    private float normalLineWidht = 1;
    private float LW_ByVelocityMin = 5.5f;
    private float LW_ByVelocityMax = 28;
    private float LW_Subdivisions = 10;
    private int lastWidthSubdivision = -2;

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

        isLineActive = true;

        wasCurveCreated = false;
        curveWidthKeys_updated = new AnimationCurve();
        curveWidthKeys_original = new AnimationCurve();
        
    }

    private void Update() {
        // keep track of the time passed since line started to be created
        if(wasCurveCreated && isLineActive)
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
                CalculateLineDrawingSpeed();
                CheckVelocityLimit();
                if(isLineActive){
                    CheckChangeInWidth();
                    UpdateCurveWidth();

                    // save last point info for reference
                    lastPointPosition = point;
                    lastPointTime = timePassedSinceCurveCreation;
                }
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
    private void CheckChangeInWidth(){
        Debug.Log("called");
        float keyPointWidth;
        float interpolatValue;
        Keyframe key;
        int currentWidthSubdivision = -3;

        #region width calculation
        // calculate the interpolation between the line width and velocity 
        interpolatValue = Mathf.InverseLerp(LW_ByVelocityMin, LW_ByVelocityMax, currentLineVelocity);
        // calculate range for each subdivision
        float rangeBetweenSubdivions = 1 / LW_Subdivisions;
        // set the interpolation values to the subdivision value
        if (interpolatValue == 0)
            currentWidthSubdivision = 0;
        else if (interpolatValue == 1)
            currentWidthSubdivision = -1;
        else if(interpolatValue > 0 && interpolatValue < 1)
        {
            for (int i = 0; i < LW_Subdivisions; i++)
            {
                if (interpolatValue < rangeBetweenSubdivions + i * rangeBetweenSubdivions)
                {
                    interpolatValue = i * rangeBetweenSubdivions;
                    currentWidthSubdivision = i;
                    break;
                }
            }
        }
        #endregion

        if (currentWidthSubdivision == -3)
        {
            Debug.Log("Error on this line, 'currentWidthSubdivision' not supposed to be -3");
            return;
        }
        // No need to register two consecutive keys with the same value
        else if(lastWidthSubdivision != currentWidthSubdivision)
        {
            Debug.Log("Current velocity: " + currentLineVelocity + ", Interpolate Value: " + interpolatValue);
            keyPointWidth = Mathf.Lerp(normalLineWidht, 0, interpolatValue);

            // register the new keypoint
            float curveDistanceWithMargin = curvedistance * 0.999f; // This way the keyframe is always before the point.
            key = new Keyframe(curveDistanceWithMargin, keyPointWidth);
            key.weightedMode = WeightedMode.In;
            key.inTangent = Mathf.Infinity;
            curveWidthKeys_original.AddKey(key);
        }
         
        lastWidthSubdivision = currentWidthSubdivision;
    }
    
    /// <summary>
    /// Calculates the speed at which the line is being drawn and saves it on 'currentLineVelocity' variable.
    /// </summary>
    private void CalculateLineDrawingSpeed(){
        // need the current placed point position and time.
        float currentPointTime = timePassedSinceCurveCreation;
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
        int length = curveWidthKeys_original.length;

        // populate the list with the updated values
        for(int i = 0; i < length; i++){
            // interpolates the distance for values between 0 and 1
            float keyframedistance = curveWidthKeys_original[i].time / curvedistance;

            Keyframe key = curveWidthKeys_original[i];
            key.time = keyframedistance;
            curveWidthKeys_updated.AddKey(key);
        }
        
        // assigning the calculated values to the LR component
        lineRenderer.widthCurve = curveWidthKeys_updated;
    }
    
    private void CheckVelocityLimit(){
        if(currentLineVelocity > LW_ByVelocityMax){
            LineController.GetInstance().SetLineState = false;
            DestroyLine();
        } 
    }
    
    public void DestroyLine(){
        isLineActive = false;
        Destroy(this.gameObject);
    }
}
