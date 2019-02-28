using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author - Shubham Sachdeva
//Class that that is used for the positioning and movement of enemies
//Restrictions or errors - NONE
public class Enemy : MonoBehaviour
{
    //These fields are for the movement of the missiles
    public float speed;
    public Vector3 velocty;
    public Vector3 direction;
    public Vector3 acceleration;
    public Vector3 position;
    public float accelRate;
    public float maxSpeed;
    //field for the camera
    public new Camera camera;

	// Use this for initialization
	void Start ()
    {
         position = transform.position;
         transform.right = direction; //this makes sure that the enemies are facing the direction they are moving
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Move the enemy
        Movement();
        //Set the position of the enemy
        SetTransform();
        //Wrap the enemy around the screen
        WrapAround();
	}

    /// <summary>
    /// Set the movement of the enemy
    /// </summary>
    void Movement()
    {
        //Multiplying the speed and direction to find velocity and moving the enemy
        velocty = speed * direction;
        position += velocty;
    }

    /// <summary>
    /// Set the positioning of the enemy
    /// </summary>
    void SetTransform()
    {
        //Setting the position of the missile
        transform.position = position;
    }

    /// <summary>
    /// Wrapping around the enemies across the scene
    /// </summary>
    void WrapAround()
    {
        //Finding the camera and using it to find the width and height of the scene
        Camera cameraObject = GameObject.FindObjectOfType<Camera>();
        float totalCamHeight = cameraObject.orthographicSize * 2f;
        float totalCamWidth = totalCamHeight * cameraObject.aspect;

        //If enemy is either less than or greater than the width
        if (position.x-transform.localScale.x/2 > totalCamWidth / 2 || transform.position.x+transform.localScale.x / 2 < -totalCamWidth / 2)
        {
            //Wrap around the x axis
            float xPos = Mathf.Clamp(transform.position.x, totalCamWidth / 2, -totalCamWidth / 2);
            //set the position
            position.x = xPos;
        }

        //If the enemy is less than or greater then the camera height
        if (position.y-transform.localScale.y/2 > totalCamHeight / 2 || transform.position.y+transform.localScale.y/2 < -totalCamHeight / 2)
        {
            //Wrap it around
            float yPos = Mathf.Clamp(transform.position.y, totalCamHeight / 2, -totalCamHeight / 2);
            //Set the position
            position.y = yPos;
        }
    }
}
