using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get points position by input (mouse/finger).
/// </summary>
public class LineInputValues : MonoBehaviour
{
    [SerializeField]private LineMaker lm;

    private void Update() {
        // check for mouse input to register point
        if(Input.GetButton("Fire1")){
            Vector2 inputPosition = Input.mousePosition;
            inputPosition = Camera.main.ScreenToWorldPoint(inputPosition);
            lm.AddPointToLine(inputPosition);
        }
    }
}
