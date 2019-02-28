using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents the zombie class and the behaviours for zombies
//Inherits from the vehicle class
//Author - Shubham Sachdeva
//Restrictions - NONE
public class Zombie : Vehicle
{

    //Fields
    List<GameObject> humans;
    List<GameObject> zombies;

    //Fields for debug lines and closest humans
    Vector3 closestHumanPosition;

    // Use this for initialization
    void Start()
    {
        //Calling the base class's start
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //Calling the base class's update
        base.Update();
        CheckCollision();

        //Toggle debug lines with D
        if(Input.GetKeyDown(KeyCode.D))
        {
            drawDebug = !drawDebug;
        }

    }

    /// <summary>
    /// Calculates the steering force
    /// </summary>
    public override void CalcSteeringForces()
    {
        humans = GameObject.Find("AgentManager").GetComponent<AgentManager>().humans;
        //Vector for the ultimate steering force

        Vector3 ultimateForce = Vector3.zero;

        Vector3 min = Vector3.zero;
        GameObject closestHuman = null;
        //Setting min distance to infinity so that we can use it 
        //to run an algorithm to find the closest zombie
        float minDistance = Mathf.Infinity;

        //Zombie can seek the humans
        for (int i = 0; i < humans.Count; i++)
        {
            //Calling the overloaded seek method for the zombie to seek humans
            //If min distance is greater than the distance between zombie and human
            if ((transform.position - humans[i].transform.position).magnitude < minDistance)
            {
                //we have a new min distance
                minDistance = (transform.position - humans[i].transform.position).magnitude;
                min = humans[i].transform.position;
                //store the reference to the closest human
                closestHuman = humans[i];
                closestHumanPosition = min;
            }

        }

        //Seek the closest human
        if (humans.Count > 0&&pursue)
        {
            //pursue until you get close and then seek
            if (minDistance > 7)
                ultimateForce += Pursue(closestHuman);

            else
                ultimateForce += Seek(closestHuman);

            //obstacleAvoidanceWeight = 2;
        }

        //Else slow down
        else if(humans.Count<=0)
        {
            //obstacleAvoidanceWeight = 0.5f;
            ApplyFriction(0.9f);

        }

        //Staying in the park
        bool stayInPark = StayInPark();

        //If the zombie is leaving the bounds
        //Seek the center of the terrain
        if (stayInPark)
        {
            Terrain terrain = Terrain.activeTerrain;
            Vector3 center = terrain.terrainData.bounds.center;
            ultimateForce += Seek(center);

        }

    


        //code for seperation
        zombies = GameObject.Find("AgentManager").GetComponent<AgentManager>().zombies;

        for(int i=0;i<zombies.Count;i++)
        {
            //Get all nearby zombies
            float distance = Vector3.Distance(zombies[i].transform.position, transform.position);
            if(zombies[i]!=gameObject&&distance<8)
            {
                //Evade them at a distance greater than 3
                //Applying weighting on the seperation force to make natural movement
               if (distance > 3)
                   ultimateForce += Evade(zombies[i])*3/distance;

               //else flee them
                else
                    ultimateForce += Flee(zombies[i])*3/distance;
            }
        }


        //Avoiding obstacles
        ultimateForce += ObstacleAvoidance();

        if (humans.Count <= 0)
        {
               ultimateForce += Wander();            
        }

        //Scaling the ultimate force by max speed
        ultimateForce *= maxSpeed;

        ultimateForce.y = 0;
        //Applying ultimate force
        ApplyForce(ultimateForce);

    }

    /// <summary>
    /// Check collision between this zombie and a human and turn that human into a zombie
    /// </summary>
    private void CheckCollision()
    {
        //Getting the reference to all humans
        humans = GameObject.Find("AgentManager").GetComponent<AgentManager>().humans;

        //Checking collision of zombie with human
        for(int i=0;i<humans.Count;i++)
        {
            //Using SAT to check collision and return whether they are colliding
            bool colliding = GameObject.Find("AgentManager").GetComponent<SATCollision>().IsColliding(gameObject, humans[i]);

            //If they are colling destroy the human and 
            //Instantiate a zombie there
            if(colliding)
            {
                Vector3 humanPosition = humans[i].transform.position;
                //Making sure that the human isnt clipping through the terrain
                Vector3 newZombiePosition = new Vector3(humanPosition.x, transform.position.y, humanPosition.z);
                //Destroying the human
                Destroy(humans[i]);
                humans.Remove(humans[i]);
                //Creating a new zombie and adding it to the class
                GameObject newZombie = Instantiate(gameObject, newZombiePosition, Quaternion.identity);
                //Adding debug lines and future position gameobject
                newZombie.GetComponent<Zombie>().drawDebug = drawDebug;
                GameObject.Find("AgentManager").GetComponent<AgentManager>().zombies.Add(newZombie);
            }

        }

    }


    /// <summary>
    /// Makes the zombie stay in the park
    /// </summary>
    /// <returns></returns>
    bool StayInPark()
    {
        //Getting the maximum and minimum x and z bounds of terrain
        Terrain terrain = Terrain.activeTerrain;
        float maxXPos = terrain.terrainData.bounds.max.x;
        float maxZPos = terrain.terrainData.bounds.max.z;
        float minXpos = terrain.terrainData.bounds.min.x;
        float minZPos = terrain.terrainData.bounds.min.z;

        //if it Hits the left edge
        if (vehiclePosition.x<= minXpos)
        {
            if(Mathf.Abs(vehiclePosition.x-minXpos)<25)
            return true;
        }

        //if it hits the right edge
        if (vehiclePosition.x>= maxXPos)
        {
            if (Mathf.Abs(vehiclePosition.x - maxXPos) < 25)

                return true;

        }

        //If it hits the top
        if (vehiclePosition.z >= maxZPos)
        {
            if (Mathf.Abs(vehiclePosition.z - maxZPos) < 25)

                return true;

        }

        //If it hits the bottom
        if (vehiclePosition.z<= minZPos)
        {
            if (Mathf.Abs(vehiclePosition.z - minZPos) < 25)

                return true;

        }


          return false;


    }

    //Method to show the debug lines
    private void OnRenderObject()
    {
        //If the user wants to see the debug lines
        if (drawDebug)
        {
            //Forward Line
            debugGreen.SetPass(0);

            GL.Begin(GL.LINES);
            GL.Vertex(transform.position); //Starting end point
            GL.Vertex(transform.position + transform.forward * 10); //Ending end point
            GL.End();

            //Right Line
            debugBlue.SetPass(0);

            GL.Begin(GL.LINES);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + transform.right * 10);
            GL.End();

            //Closest Human line
            if (GameObject.Find("AgentManager").GetComponent<AgentManager>().humans.Count > 0)
            {
                debugBlack.SetPass(0);
                GL.Begin(GL.LINES);
                GL.Vertex(transform.position);
                GL.Vertex(closestHumanPosition);
                GL.End();
            }

            //future position gameobject is set to true
            futurePositionReference.SetActive(true);

        }

        else
        {
            //Else set it's visibility to false
            futurePositionReference.SetActive(false);
        }

    }
}
