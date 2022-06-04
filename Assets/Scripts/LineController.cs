using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates the lines that will be drawn and establishing the behavior of its creation.
/// Reads points for the lines by input (mouse/finger).
/// </summary>
public class LineController : Singleton<LineController>
{
    // line fields
    public GameObject LinePrefab;
    private LineMaker lineMakerRef;
    private bool isLineActive;

    // input point
    private Vector2 inputPoint;

    // properties
    public bool SetLineState{
        set{if(isLineActive && value == false){
                isLineActive = false;
                lineMakerRef = null;
            }
        } 
    }
    

    private void Update() {  
        GetInputPoint();
    }

    /// <summary>
    /// checks for mouse input and converts to game position
    /// </summary>
    private void GetInputPoint(){
        if(Input.GetButtonDown("Fire1") && isLineActive == false){
            CreateLine();
        }
        else if(Input.GetButtonDown("Fire2") && isLineActive){
            lineMakerRef.DestroyLine();
            SetLineState = false;
        }
        else if(Input.GetButton("Fire1")){
            Vector2 inputPosition = Input.mousePosition;
            inputPoint = Camera.main.ScreenToWorldPoint(inputPosition);
            if(isLineActive){
                // this means a line exist
                lineMakerRef.AddPointToLine(inputPoint);
            }
            // always reset the inputPoint to 0 from last frame.
            inputPoint = Vector2.zero;
        }
    }

    private void CreateLine(){
        GameObject InstantiatedLine = Instantiate(LinePrefab);
        isLineActive = true;
        if((lineMakerRef = InstantiatedLine.GetComponent<LineMaker>()) == null){
            Debug.Log("Line Maker script was now found on the creation of the line");
        }
    }
}
