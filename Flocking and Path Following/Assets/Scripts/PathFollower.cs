using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for the path follower
//Inherits from vehicle
//Author - Shubham Sachdeva
//Restrictions - None
public class PathFollower : Vehicle
{
    //Fields
    List<GameObject> waypoints;
    int currentWaypoint;

	// Use this for initialization
	void Start ()
    {
        base.Start();
        //Getting all the waypoints
        waypoints = GameObject.Find("SceneManager").GetComponent<SceneManager>().waypoints;
        currentWaypoint = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        base.Update();
		
	}

    /// <summary>
    /// Calculating the steering forces
    /// </summary>
    public override void CalcSteeringForces()
    {
        Vector3 ultimateForce = Vector3.zero;

        //Adding the path following force
        ultimateForce += PathFollowing();

        ultimateForce.y = 0;

        //Scaling the ultimate force
        ultimateForce *= maxSpeed;

        //Applying the force
        ApplyForce(ultimateForce);
    }

    /// <summary>
    /// Following the predefined path
    /// </summary>
    /// <returns></returns>
    public Vector3 PathFollowing()
    {
        //Loop through the waypoint
        for(int i = 0;i<waypoints.Count;i++)
        {
            //If you are close to a waypoint, changeyour objective
            if(Vector3.Distance(waypoints[i].transform.position,transform.position)<=5)
            {
                //If i+1 is less than count select i+1
                if(i+1>=waypoints.Count)
                {
                    currentWaypoint = 0;
                }

                //else go back to zero
                else
                {
                    currentWaypoint = i + 1;
                }

            }

        }

        //Return the seeking force
        return (Seek(waypoints[currentWaypoint].transform.position));

    }
}
