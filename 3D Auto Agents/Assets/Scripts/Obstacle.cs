using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that represents the obstacle
//Author - Shubham Sachdeva
//Restrictions or Errors - NONE
public class Obstacle : MonoBehaviour
{
    //Fields
    public float radius; //Radius of the obstacle

	// Use this for initialization
	void Start ()
    {
        //Initializing the radius
        radius = GetComponent<Renderer>().bounds.extents.magnitude;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
