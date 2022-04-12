using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//set borders for the game
public class Borders : MonoBehaviour
{
    private float verticalGap;
    private float horizontalGap;
    public GameObject borderPrefab;

    // reference variables
    private float screenLeft;
    private float screenTop;

    // time update variables
    private float timer;
    private float timerTime = 0.5f;

    private void OnEnable() {
        screenLeft = ScreenUtils.ScreenLeft;
        screenTop = ScreenUtils.ScreenTop;

        CreateBorders();
    }

    private void Update(){

        #region Update Borders

        if(timer <= 0){
            if(screenLeft != ScreenUtils.ScreenLeft || screenTop != ScreenUtils.ScreenTop){
                CreateBorders();
            }

            timer = timerTime;
        }

        timer -= Time.deltaTime;

        #endregion
    }

    private void CreateBorders(){
        //Create a parent GameObject to organize hierarchy
        GameObject bordersContainer = new GameObject("Game Borders");


        // ---------------------Right border------------------------
        //----------------------------------------------------------
        GameObject rightBorder = Instantiate(borderPrefab);
        rightBorder.name = "Right Border";
        rightBorder.transform.SetParent(bordersContainer.transform);

        // set the correct position
        Vector2 positionRB = new Vector2(ScreenUtils.ScreenRight, 0);
        rightBorder.transform.position = positionRB;

        // set the correct scale
        Vector2 scaleRB = new Vector2(1,ScreenUtils.ScreenTop * 2);
        rightBorder.transform.localScale = scaleRB;


        // -----------------------Left border------------------------
        //-----------------------------------------------------------
        GameObject leftBorder = Instantiate(borderPrefab);
        leftBorder.name = "Left Border";
        leftBorder.transform.SetParent(bordersContainer.transform);

        // set the correct position
        Vector2 positionLB = new Vector2(ScreenUtils.ScreenLeft, 0);
        leftBorder.transform.position = positionLB;

        // set the correct scale
        Vector2 scaleLB = new Vector2(1,ScreenUtils.ScreenTop * 2);
        leftBorder.transform.localScale = scaleLB;


        // -----------------------Top border-------------------------
        //-----------------------------------------------------------
        GameObject topBorder = Instantiate(borderPrefab);
        topBorder.name = "Top Border";
        topBorder.transform.SetParent(bordersContainer.transform);

        // set the correct position
        Vector2 positionTB = new Vector2(0, ScreenUtils.ScreenTop);
        topBorder.transform.position = positionTB;

        // set the correct scale
        Vector2 scaleTB = new Vector2(ScreenUtils.ScreenRight * 2, 1);
        topBorder.transform.localScale = scaleTB;


        // ---------------------Bottom border-------------------------
        //------------------------------------------------------------
        GameObject bottomBorder = Instantiate(borderPrefab);
        bottomBorder.name = "Bottom Border";
        bottomBorder.transform.SetParent(bordersContainer.transform);

        // set the correct position
        Vector2 positionBB = new Vector2(0, ScreenUtils.ScreenBottom);
        bottomBorder.transform.position = positionBB;

        // set the correct scale
        Vector2 scaleBB = new Vector2(ScreenUtils.ScreenRight * 2, 1);
        bottomBorder.transform.localScale = scaleBB;

        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name +
         " method was called! From: " + this.GetType().FullName);
    }

}
