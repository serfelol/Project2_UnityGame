using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFormulas
{
    /// <summary>
    /// calculates the distance between two points.
    /// </summary>
    public static float DistanceBetweenTwoPoints(float p1x, float p1y, float p2x, float p2y){
        float distance = Mathf.Sqrt((Mathf.Pow(p1x - p2x, 2) + Mathf.Pow(p1y - p2y, 2)));
        return distance;
    }

    /// <summary>
    /// calculates the velocity between two points.
    /// </summary>
    public static float VelocityBetweenTwoPoints2D(Vector2 p1, Vector2 p2, float time){
        float distance = MathFormulas.DistanceBetweenTwoPoints(p1.x, p1.y, p2.x, p2.y);
        float velocity = distance / time;
        return velocity;
    }

    /// <summary>
    /// checks if the difference between two values is greater than the margin.
    /// </summary>
    public static bool CompareDifference(float value1, float value2, float margin){
        return Mathf.Abs(value1 - value2) > margin? true : false;
    }
}
