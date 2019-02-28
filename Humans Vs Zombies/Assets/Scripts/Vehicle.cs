using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class that defines a general functionality for an autonomous agent
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

    //Debug Lines Materials
    [SerializeField] protected Material debugGreen;
    [SerializeField] protected Material debugBlue;
    [SerializeField] protected Material debugBlack;
    [SerializeField] protected GameObject futurePosition;
    protected GameObject futurePositionReference;

    protected bool drawDebug = false;

    //Field for obstacle avoidance
    private float radius;
    [SerializeField] float safeDistance;
    //protected float obstacleAvoidanceWeight;
    protected List<GameObject> obstacles;
    protected bool pursue;
    protected bool evade;

    //Fields for wandering
    [SerializeField] float circleDistance;
    [SerializeField] float circleRadius;
    protected bool wander;
    protected float wanderAngle;
    protected float elapsed;

    //Field to stay in park
    protected bool stayInPark;

    // Use this for initialization
    public void Start()
    {
        vehiclePosition = transform.position;

        //Setting wander to true
        wander = true;

        //Getting radius of vehicle
        radius = gameObject.GetComponent<Renderer>().bounds.extents.magnitude;

        //Getting an initial wander angle
        wanderAngle = 0;

        obstacles = GameObject.Find("AgentManager").GetComponent<AgentManager>().obstacles;
        futurePositionReference= Instantiate(futurePosition, transform.position + velocity, Quaternion.identity);
        futurePositionReference.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
    {

        CalcSteeringForces();

        // ------------------------------
        // Moving algorithm
        //Add accel to velocity
        //update the vehicle position
        //Change the direction
        //Draw the vehicle at new position
        velocity += acceleration * Time.deltaTime;
        vehiclePosition += velocity * Time.deltaTime;
        direction = velocity.normalized;
        acceleration = Vector3.zero;
        transform.position = vehiclePosition;
        // ------------------------------

        RotateVehicle();

        futurePositionReference.transform.position = transform.position + velocity;

    }

    // ApplyForce
    // Receive an incoming force, divide by mass, and apply to the cumulative accel vector
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    // ApplyForce
    // Receive an incoming force, divide by mass, and apply to the cumulative accel vector
    public void ApplyGravityForce(Vector3 force)
    {
        acceleration += force;
    }

    /// <summary>
    /// Method to apply the friction force on the agent
    /// </summary>
    /// <param name="coeff">the coefficient of friction of the surface</param>
    public void ApplyFriction(float coeff)
    {
        //Friction is applied in the opposite direction to velocity
        Vector3 friction = velocity * -1;
        //Normalizing it
        friction.Normalize();
        //Multiplying it by the coeff of friction
        friction = friction * coeff;
        //Adding it to the acceleration
        acceleration += friction;
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
    /// Overloaded Seek
    /// </summary>
    /// <param name="target">GameObject of the target</param>
    /// <returns>Steering force calculated to seek the desired target</returns>
    public Vector3 Seek(GameObject target)
    {
        return Seek(target.transform.position);
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

    /// <summary>
    /// Overloaded seek, moves vehicle to a certain target
    /// </summary>
    /// <param name="target">Gameobject we are fleeing from</param>
    /// <returns>The position of the target</returns>
    public Vector3 Flee(GameObject target)
    {

        //Calling the overloaded flee method
        return Flee(target.transform.position);

    }

    /// <summary>
    /// Rotate the vehicle towards its direction
    /// </summary>
    public void RotateVehicle()
    {
        if(direction!=Vector3.zero)
        transform.rotation = Quaternion.LookRotation(direction);
    }

    //Abstract method that calculates the steering forces;
    public abstract void CalcSteeringForces();

    /// <summary>
    /// Method for obstacle avoidance
    /// </summary>
    /// <returns></returns>
    public Vector3 ObstacleAvoidance()
    {
        //Getting a reference to all objects in the scene
        //List<GameObject> obstacles = GameObject.Find("AgentManager").GetComponent<AgentManager>().obstacles;

        //All the relevant obstacles, i.e the one in the front
        List<GameObject> relevantObstacles = new List<GameObject>();

        for(int i=0;i<obstacles.Count;i++)
        {
            //Finding the dot product between forward of the vehicle and the vector from the vehicle and obstacle
            //If it is positive add it to the relevant obstacles list
            float dotValue = Vector3.Dot(transform.forward, (obstacles[i].transform.position - transform.position));
            if(dotValue>=0)
            {
                relevantObstacles.Add(obstacles[i]);
            }

        }

        //looping through relevant obstacles
        for(int i = 0;i<relevantObstacles.Count;i++)
        {
            //Doing a safe distance check for the vehicle
            if(Vector3.Distance(transform.position,relevantObstacles[i].transform.position)<safeDistance)
            {

                float obstacleDistance = Vector3.Distance(transform.position, relevantObstacles[i].transform.position);

                //Getting the direction vector from vehicle to obstacle
                Vector3 direction = relevantObstacles[i].transform.position - transform.position;
                float obstacleRadius = relevantObstacles[i].GetComponent<Obstacle>().radius;

                //Using the dot product of the obstacle radius and the right vector of the vehicle
                //to find the distance between them
                float distance = (Vector3.Dot(transform.right, direction));

                //The non integer test
                //If the absolute value of the distance less than the sum of the radii of
                //the vehicle and obstacle
                if(Mathf.Abs(distance)<radius+obstacleRadius)
                {
                    //Turning evade off so as to create more natural movement
                    evade = false;
                    //If yes then check if the the obstacle is on the right or left
                    Vector3 desiredVelocity = Vector3.zero;
                    //If it is on the left, since dot product is <0
                    if(distance<0)
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
                    return ((desiredVelocity-velocity)*120/((obstacleDistance*obstacleDistance)+1));
                    
                }

                

                
            }
        }

        //If there is no obstacle within safe distance, no steering force is applied
        //Turn on wander, evade and pursue
        wander = true;
        evade = true;
        pursue = true;
        return Vector3.zero;
        

    }

    /// <summary>
    /// Method that makes the vehicle wander
    /// </summary>
    /// <returns></returns>
    public Vector3 Wander()
    {
        //If wander is turned on
        if (wander)
        {
            //Calculate the direction of the center of the circle
            Vector3 circleCenter = Vector3.zero;

            //Make the center the same direction as velocity and normalize it
            circleCenter = transform.forward;
            circleCenter.Normalize();

            //Place the center of the circle where you want it to be in front of the agent
            circleCenter *= circleDistance;

            //Find the global position of the center
            circleCenter += transform.position;

            //Find a small angle to wander on
            wanderAngle = Random.Range(wanderAngle - 0.3f, wanderAngle + 0.3f);

            //Find a point on that circle which is that many radians away from the initial direction 
            //Use a circle radius to determine that
            float xPos = Mathf.Cos(wanderAngle) * circleRadius;
            float zPos = Mathf.Sin(wanderAngle) * circleRadius;

            //Use the x and z positions to find the spot on the circle and then
            //Find the global position of that spot
            Vector3 wanderingSpot = circleCenter + new Vector3(xPos, 0, zPos);

            //Seek that spot
            return Seek(wanderingSpot)/1.5f; //Wandering weight is a constant factor to get a more natural movements
        }

        return Vector3.zero;


    }

    /// <summary>
    /// Method to evade an agent
    /// </summary>
    /// <param name="agent">The agent we evade</param>
    /// <returns></returns>
    public Vector3 Evade(GameObject agent)
    {
        //Evasion is just fleeing the future position of the agent
        return Flee(agent.transform.position + agent.GetComponent<Vehicle>().velocity);
    }

    /// <summary>
    /// Method to pursue an agent
    /// </summary>
    /// <param name="agent">The agent we want to pursue</param>
    /// <returns></returns>
    public Vector3 Pursue(GameObject agent)
    {
        //Pursue is just seeking the future position of the target
        return Seek(agent.transform.position + agent.GetComponent<Vehicle>().velocity);
    }

  

}

