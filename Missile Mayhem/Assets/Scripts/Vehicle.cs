using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author Shubham Sachdeva
//Class to control movement of the vehicle
//Restriction or errors - NONE
public class Vehicle : MonoBehaviour
{
    //Fields
    public float accelRate;         // Small, constant rate of acceleration
    public Vector3 vehiclePosition; // Local vector for movement calculation
    public Vector3 direction;       // Way the vehicle should move
    public Vector3 velocity;        // Change in X and Y
    public Vector3 acceleration;    // Small accel vector that's added to velocity
    public float angleOfRotation;   // 0 
    public float maxSpeed;          // 0.5 per frame, limits mag of velocity
    public float deccelRate;        //Deaccelerates the vehicle

    public Camera cameraObject;    //Camera of the 2D Scene
    //Static fields to track the health and score of the vehicle
    public static int shipHealth;
    public static int score;
    //Field to check if the vehicle is damaged
    public bool damaged;

	// Use this for initialization
	void Start ()
    {
        shipHealth = 3;
        score = 0;
        damaged = false;
        vehiclePosition = new Vector3(0, 0, 0);     // Or you could say Vector3.zero
        direction = new Vector3(1, 0, 0);           // Facing right
        velocity = new Vector3(0, 0, 0);            // Starting still (no movement)
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    /// <summary>
    /// Changes / Sets the transform component
    /// </summary>
    public void SetTransform()
    {
        // Rotate vehicle sprite
        transform.rotation = Quaternion.Euler(0, 0, angleOfRotation);

        // Set the transform position
        transform.position = vehiclePosition;    
    }

    /// <summary>
    /// Used to accelerate the vehicle
    /// </summary>
    public void Drive()
    {
        // Accelerate
        // Small vector that's added to velocity every frame

        //If user presses forward, move forward
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //Finding the rate of acceleration
            acceleration = accelRate * direction;
            // We used to use this, but acceleration will now increase the vehicle's "speed"
            // Velocity will remain intact from one frame to the next
            velocity += acceleration;
            // Limit velocity so it doesn't become too large
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        }

        //Keep multiplying the deacceleration which is less than 1
        else 
        {
            velocity = velocity * deccelRate;
        }

         // Add velocity to vehicle's position
         vehiclePosition += velocity;
    }

    /// <summary>
    /// Method to rotate the vehicle
    /// </summary>
    public void RotateVehicle()
    {
        // Player can control direction
        // Left arrow key = rotate left by 2 degrees
        // Right arrow key = rotate right by 2 degrees
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angleOfRotation += 2;
            direction = Quaternion.Euler(0, 0, 2) * direction;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            angleOfRotation -= 2;
            direction = Quaternion.Euler(0, 0, -2) * direction;
        }
    }

    /// <summary>
    /// Method to wrap the vehicle wrap around
    /// </summary>
    public void WrapAround()
    {
        //Using the camera gameobject to find height and width of screen
        float totalCamHeight = cameraObject.orthographicSize*2f;
        float totalCamWidth = totalCamHeight * cameraObject.aspect;

        //If the ship is greater or less than the width of the screen 
        if(vehiclePosition.x>totalCamWidth/2||transform.position.x<-totalCamWidth/2)
        {
            //Wrap around the x direction
            float xPos = Mathf.Clamp(transform.position.x, totalCamWidth / 2, -totalCamWidth / 2);
            vehiclePosition.x = xPos;
        }

        //If the sip is greater or less than the height of the screen
        if (vehiclePosition.y > totalCamHeight / 2 || transform.position.y < -totalCamHeight / 2)
        {
            //Wrap around the y axis
            float yPos = Mathf.Clamp(transform.position.y, totalCamHeight / 2, -totalCamHeight / 2);
            vehiclePosition.y = yPos;
        }
    }
}
