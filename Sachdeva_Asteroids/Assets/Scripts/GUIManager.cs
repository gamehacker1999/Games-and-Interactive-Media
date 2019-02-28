using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author Shuham Sachdeva
//Class that displayes the score and health on the canvas
//Restrictions - Not reusable between scenes, only shows health and the score
public class GUIManager : MonoBehaviour
{
    //Fields for the GUI Manager
    [SerializeField]Text score;
    [SerializeField]Text health;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        //If the scene has a health in it show it
        if (health != null)
        {
            health.text = Vehicle.shipHealth.ToString();
        }
        //Display the score in the game scene
        score.text = Vehicle.score.ToString();
    }
}
