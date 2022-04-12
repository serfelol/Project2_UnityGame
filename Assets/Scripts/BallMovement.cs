using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes the movement of the ball
/// </summary>
/// 
public class BallMovement : MonoBehaviour
{
    public float forceToApply;

    // debug field for velocity
    public float currentVelocity;
    public string currentVeloString;

    //

    private Vector2 initialDirection;

    private void Start() {
        InitialDirection();
        MoveBall();
    }

    private void Update(){
        currentVelocity = Mathf.Sqrt(Mathf.Pow( GetComponent<Rigidbody2D>().velocity.x, 2) + Mathf.Pow(GetComponent<Rigidbody2D>().velocity.y, 2));
        currentVeloString = GetComponent<Rigidbody2D>().velocity.ToString("R");
    }

    /// <summary>
    /// Sets a random Initial Direction.
    /// </summary>
    private void InitialDirection(){
        initialDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f,1f)).normalized;
    }

    /// <summary>
    /// Gives force to the ball so it can move.
    /// </summary>
    private void MoveBall(){
        transform.GetComponent<Rigidbody2D>().AddForce(initialDirection * forceToApply, ForceMode2D.Force);
    }
}
