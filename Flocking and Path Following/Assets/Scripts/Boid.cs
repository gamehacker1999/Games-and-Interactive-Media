using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that represents a flocking boid
//Inherits from the vehicle class
//Author - Shubham Sachdeva
//Restrictions - None
public class Boid : Vehicle
{
    //Fields for weights
    public float seperationWeight;
    public float alignmentWeight;
    public float cohesionWeight;
    public float obstacleAvoidanceWeight;

	// Use this for initialization
	void Start ()
    {
        base.Start();
	}
	
	// Update is called once per frame
	void Update ()
    {
        base.Update();

        //Adjustimg the cohesion weight based on how far they are from the center of the flock
        if (Vector3.Distance(transform.position, centroid) > 10)
        {
            cohesionWeight = 8;
        }

        else
        {
            cohesionWeight = 0.2f;
        }

    }

    /// <summary>
    /// Method to calculate the steering forces
    /// </summary>
    public override void CalcSteeringForces()
    {
        Vector3 ultimateForce = Vector3.zero;

        //If flocking is turned on
        if (flocking)
        { 
            //Add the seperation, cohesion and alignment forces
            ultimateForce += Seperation() * seperationWeight;
            ultimateForce += Cohesion() * cohesionWeight;
            ultimateForce += Alignment() * alignmentWeight;
        }

        //Add the obstacle avoidance force
        ultimateForce += ObstacleAvoidance()*(obstacleAvoidanceWeight);

        //Check if objects are leaving the park
        bool stayInPark = StayInPark();

        //If yes then stay in the park
        if(stayInPark)
        {
            //Getting the center of the terrain and seeking it
            Terrain terrain = Terrain.activeTerrain;
            Vector3 center = terrain.terrainData.bounds.center;
            center.y = 25;
            ultimateForce += Seek(center);
        }

        //scaling the ultimate force
        ultimateForce *= maxSpeed;

        //Applying the ultimate force
        ApplyForce(ultimateForce);
    }
}
