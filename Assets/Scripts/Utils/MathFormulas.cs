using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFormulas
{
    /// <summary>
    /// calculates the distance between two points.
    /// </summary>
    public static float DistanceBetweenTwoPoints(float x1, float y1, float x2, float y2){
        float distance = Mathf.Sqrt((Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2)));
        return distance;
    }

    /// <summary>
    /// calculates the velocity between two points.
    /// </summary>
    public static float VelocityBetweenTwoPoints2D(Vector2 p1, Vector2 p2, float time){
        float distance = MathFormulas.DistanceBetweenTwoPoints(p2.x, p1.x, p2.y, p1.y);
        float velocity = distance / time;
        return velocity;
    }
}
