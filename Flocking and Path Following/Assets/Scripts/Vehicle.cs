using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class to implement the generic behavior for autonomous agents
//Author - Shubham Sachdeva
//Restrictions - NONE
public abstract class Vehicle : MonoBehaviour
{
    // Vectors necessary for force-based movement
    protected Vector3 vehiclePosition;
    public Vector3 acceleration;
    public Vector3 direction;
    public Vector3 velocity;

    // Floats
    public float mass;
    public float maxSpeed;

    //Fields For obstacle avoidance
    private float radius;
    [SerializeField] float safeDistance;

    //Fields for following a random position
    public GameObject sphere;



    //Fields for flocking
    //Bools
    protected bool flocking;

    //Vector
    protected Vector3 centroid;

    // Use this for initialization
    public void Start()
    {
        vehiclePosition = transform.position;
        radius = gameObject.GetComponent<Renderer>().bounds.extents.magnitude;
        sphere = GameObject.Find("SceneManager").GetComponent<SceneManager>().sphereReference;
    }

    // Update is called once per frame
    public void Update()
    {
        CalcSteeringForces();

        //Calculations Necessary for the movement of the vehicle
        velocity += acceleration * Time.deltaTime;
        vehiclePosition += velocity * Time.deltaTime;
        direction = velocity.normalized;
        acceleration = Vector3.zero;
        transform.position = vehiclePosition;

        RotateVehicle();
    }

    /// <summary>
    /// Rotate the vehicle towards its direction
    /// </summary>
    public void RotateVehicle()
    {
        //Rotating the vector in the direction of motion
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    public abstract void CalcSteeringForces();

    /// <summary>
    /// Method to find the cohesion force of the flock
    /// </summary>
    /// <returns></returns>
    public Vector3 Cohesion()
    {
        //Finding the average position of the flock
        centroid = GameObject.Find("SceneManager").GetComponent<SceneManager>().flockCenter;
        //Seeking it
        return Seek(centroid);
    }

    /// <summary>
    /// Method to find the seperation force
    /// </summary>
    /// <returns></returns>
    public Vector3 Seperation()
    {
        //Get a reference to all the flockers
        List<GameObject> boids = GameObject.Find("SceneManager").GetComponent<SceneManager>().boids;

        Vector3 seperationForce = Vector3.zero;

        //Loop through all of them
        for (int i = 0; i < boids.Count; i++)
        {
            float distance = Vector3.Distance(boids[i].transform.position, transform.position);
            //If flocker is too close to another flocker, flee it
            if (boids[i] != gameObject && distance < 4)
            {
                //Add all flee forces
                seperationForce += Flee(boids[i].transform.position);
            }
        }

        return seperationForce;
    }

    /// <summary>
    /// Method to find the alignment force for flockers
    /// </summary>
    /// <returns></returns>
    public Vector3 Alignment()
    {
        //Getting the average flock direction
        Vector3 flockDirection = GameObject.Find("SceneManager").GetComponent<SceneManager>().flockDirection;

        //Normalizing it
        flockDirection.Normalize();

        //Finding the desired velocity in that direction
        Vector3 desiredVelocity = flockDirection * maxSpeed;

        //Returning the steering force
        return (desiredVelocity - velocity);
    }

    /// <summary>
    /// Method for obstacle avoidance
    /// </summary>
    /// <returns></returns>
    public Vector3 ObstacleAvoidance()
    {
        //Getting a reference to all objects in the scene
        List<GameObject> obstacles = GameObject.Find("SceneManager").GetComponent<SceneManager>().obstacles;

        //All the relevant obstacles, i.e the one in the front
        List<GameObject> relevantObstacles = new List<GameObject>();

        for (int i = 0; i < obstacles.Count; i++)
        {
            Vector3 heading = obstacles[i].transform.position - transform.position;
            heading.y = 0;
            //Finding the dot product between forward of the vehicle and the vector from the vehicle and obstacle
            //If it is positive add it to the relevant obstacles list
            float dotValue = Vector3.Dot(transform.forward, (heading));
            if (dotValue >= 0)
            {
                relevantObstacles.Add(obstacles[i]);
            }

        }
        //looping through relevant obstacles
        for (int i = 0; i < relevantObstacles.Count; i++)
        {

            Vector3 heading = relevantObstacles[i].transform.position - transform.position;
            heading.y = 0;

            //Doing a safe distance check for the vehicle
            if (heading.magnitude < safeDistance)
            {

                //Getting the direction vector from vehicle to obstacle
                Vector3 direction = relevantObstacles[i].transform.position - transform.position;
                float obstacleRadius = relevantObstacles[i].GetComponent<Obstacle>().radius;

                //Using the dot product of the obstacle radius and the right vector of the vehicle
                //to find the distance between them
                float distance = (Vector3.Dot(transform.right, direction));

                //The non integer test
                //If the absolute value of the distance less than the sum of the radii of
                //the vehicle and obstacle
                if (Mathf.Abs(distance) < radius + obstacleRadius)
                {
                    flocking = false;

                    //If yes then check if the the obstacle is on the right or left
                    Vector3 desiredVelocity = Vector3.zero;
                    //If it is on the left, since dot product is <0
                    if (distance < 0)
                    {
                        //Steer right
                        desiredVelocity = transform.right * maxSpeed;
                    }

                    //If it is on the left
                    else
                    {
                        //Steer right
                        desiredVelocity = -transform.right * maxSpeed;
                    }

                    //Calculate the steering force and return;
                    return (desiredVelocity - velocity);

                }


            }
        }

        //If there is no obstacle within safe distance, no steering force is applied
        flocking = true;
        return Vector3.zero;

    }

    // ApplyForce
    // Receive an incoming force, divide by mass, and apply to the cumulative accel vector
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    // SEEK METHOD
    // All Vehicles have the knowledge of how to seek
    // They just may not be calling it all the time
    /// <summary>
    /// Seek
    /// </summary>
    /// <param name="targetPosition">Vector3 position of desired target</param>
    /// <returns>Steering force calculated to seek the desired target</returns>
    public Vector3 Seek(Vector3 targetPosition)
    {
        // Step 1: Find DV (desired velocity)
        // TargetPos - CurrentPos
        Vector3 desiredVelocity = targetPosition - vehiclePosition;
        //desiredVelocity.y = 0;

        // Step 2: Scale vel to max speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Step 3:  Calculate seeking steering force
        Vector3 seekingForce = desiredVelocity - velocity;

        // Step 4: Return force
        return seekingForce;
    }

    /// <summary>
    /// Method that enables vehicle to flee from a certain position
    /// </summary>
    /// <param name="targetPosition">Position of the target we are fleeing from</param>
    /// <returns>Returns the fleeing force</returns>
    public Vector3 Flee(Vector3 targetPosition)
    {

        // Step 1: Find DV (desired velocity)
        Vector3 desiredVelocity = (targetPosition - vehiclePosition) * -1;

        // Step 2: Scale vel to max speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Step 3:  Calculate fleeing steering force
        Vector3 fleeingForce = desiredVelocity - velocity;

        // Step 4: Return force
        return fleeingForce;

    }

    public bool StayInPark()
    {
        //Getting the bounds of the terrain
        Terrain terrain = Terrain.activeTerrain;
        float maxXPos = terrain.terrainData.bounds.max.x;
        float maxZPos = terrain.terrainData.bounds.max.z;
        float minXpos = terrain.terrainData.bounds.min.x;
        float minZPos = terrain.terrainData.bounds.min.z;

        //If it hits the left edge
        if (vehiclePosition.x <= minXpos)
        {
            return true;
        }

        //Hits the right edge
        if (vehiclePosition.x >= maxXPos)
        {
            return true;

        }

        //If it hits the top
        if (vehiclePosition.z >= maxZPos)
        {
            return true;

        }

        //If it hits the bottom
        if (vehiclePosition.z <= minZPos)
        {
            return true;

        }

        //Top limit
        if(vehiclePosition.y>25)
        {
            return true;
        }

        //Bottom limit
        if(vehiclePosition.y<8)
        {
            return true;
        }

        return false;


    }
}
