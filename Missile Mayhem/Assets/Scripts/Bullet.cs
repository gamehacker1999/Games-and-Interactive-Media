using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Author- Shubham Sachdeva
//Class that controls the movement and positioning of bullets
//errors or restrictions - NONE
public class Bullet : MonoBehaviour
{
    //Fields for the movement of the bullet
    public Vector3 velocty;
    public Vector3 direction;
    public Vector3 acceleration;
    public float accelRate;
    public float maxSpeed;
    public Vector3 bulletPosition;
    //fields to associate ship to a bullet
    private GameObject ship;
    Vehicle shipManager;

     

    // Use this for initialization
    void Start ()
    {
        //Getting the ship GameObject and using it to set the initial position and velocity of the bullet
        ship = GameObject.Find("Ship");
        shipManager = ship.GetComponent<Vehicle>();
        transform.position = ship.transform.position;
        bulletPosition = ship.transform.position;
        direction = shipManager.direction;
        accelRate = shipManager.accelRate * 2;
        velocty = shipManager.velocity;
        //Play a sound when the bullet is created
        AudioManager.PlayBulletSound();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Moving the bullet and setting its position
        Movement();
        SetTransform();	}

    /// <summary>
    /// Sets the transform of the bullet
    /// </summary>
    void SetTransform()
    {
        transform.position = bulletPosition;
    }

    /// <summary>
    /// Moves the bullet forward in its direction
    /// </summary>
    void Movement()
    {
        //calculating the acceleration
        acceleration = accelRate * direction;
        velocty += acceleration;
        //Clamping the magnitude of the bullet so that it doesn't go really fast
        Vector3.ClampMagnitude(velocty, 1);
        //Moving the bullet
        bulletPosition += velocty;
    }
}
