using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents the human class and the behaviours for humans
//Inherits from the vehicle class
//Author - Shubham Sachdeva
//Restrictions - NONE
public class Human : Vehicle
{

    //Fields
    List<GameObject> zombies;
    List<GameObject> humans;

	// Use this for initialization
	void Start ()
    {
        base.Start();
    }
	
	// Update is called once per frame
	void Update ()
    {
        base.Update();

        //Toggle debug lines with D
        if (Input.GetKeyDown(KeyCode.D))
        {
            drawDebug = !drawDebug;
        }
    }

    /// <summary>
    /// Calculates the steering forces
    /// </summary>
    public override void  CalcSteeringForces()
    {
        //Getting a reference to all the zombies in the scene
        zombies = GameObject.Find("AgentManager").GetComponent<AgentManager>().zombies;
        

        //Vector that represents the total steering force
        Vector3 ultimateForce =Vector3.zero;

        //Flee the zombie
        for(int i = 0;i<zombies.Count;i++)
        {
            float sqDistance =  (transform.position - zombies[i].transform.position).sqrMagnitude;

            //Evade or flee the zombie when it is less than a few units away
            if ((sqDistance<81)&&evade)
            {
                //Applying the weights 
                if (sqDistance > 45)
                    ultimateForce += Evade(zombies[i])*9/Mathf.Sqrt(sqDistance) ;

                else
                    ultimateForce += Flee(zombies[i]) * 9 / Mathf.Sqrt(sqDistance);
            }

            //Else slow down
            else if(sqDistance>=81)
            {
                ApplyFriction(0.9f);
            }

        }

        //code for seperation
        //Get a reference to all the humans in the scene
        humans = GameObject.Find("AgentManager").GetComponent<AgentManager>().humans;

        for (int i = 0; i < humans.Count; i++)
        {
            //Find which human is too close to it
            float distance = Vector3.Distance(humans[i].transform.position, transform.position);
            if (humans[i] != gameObject && distance < 3)
            {
                //If human is too close then flee him
                ultimateForce += Flee(humans[i]);
            }
        }

        //Staying within the terrain
        bool stayInPark = StayInPark();

        //If the human hits the boundry, seek the center
        if(stayInPark)
        {
            //Turn evasion of to create natural movement
            evade = false;

            //Getting the center of the terrain and seeking it
            Terrain terrain = Terrain.activeTerrain;
            Vector3 center = terrain.terrainData.bounds.center;
            ultimateForce += Seek(center)*Vector3.Distance(transform.position,center)/25;

        }

        //Else turn evasion on
        else
        {
            evade = true;
        }

       

        //Avoiding obstacles
        ultimateForce += ObstacleAvoidance()*2f; //Multiplying by a factor that makes movement more realistic

        //Wandering
        ultimateForce += Wander();

        //Scaling the ultimate force by max speed
        ultimateForce *= maxSpeed;

        ultimateForce.y = 0;

        //Applying this force
        ApplyForce(ultimateForce);

    }

    /// <summary>
    /// Human stays in the terrain
    /// </summary>
    /// <returns></returns>
    bool StayInPark()
    {
        //Getting the bounds of the terrain
        Terrain terrain = Terrain.activeTerrain;
        float maxXPos = terrain.terrainData.bounds.max.x;
        float maxZPos = terrain.terrainData.bounds.max.z;
        float minXpos = terrain.terrainData.bounds.min.x;
        float minZPos = terrain.terrainData.bounds.min.z;

        //If it hits the left edge
        if (vehiclePosition.x<= minXpos)
        {
            return true;
        }

        //Hits the right edge
        if (vehiclePosition.x>= maxXPos)
        {
            return true;

        }

        //If it hits the top
        if (vehiclePosition.z >= maxZPos)
        {
            return true;

        }
    
        //If it hits the bottom
        if (vehiclePosition.z<= minZPos)
        {
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
            GL.Vertex(transform.position + transform.forward * 10); //Finishing end point
            GL.End();

            //Right Line
            debugBlue.SetPass(0);

            GL.Begin(GL.LINES);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + transform.right * 10);
            GL.End();

            //Gameobject for the future position is visible
            futurePositionReference.SetActive(true);

        }

        else
        {
            //Else turn visibility off
            futurePositionReference.SetActive(false);
        }

    }

    private void OnDestroy()
    {
        //Destroy the future position gObject
        Destroy(futurePositionReference);
    }
}
