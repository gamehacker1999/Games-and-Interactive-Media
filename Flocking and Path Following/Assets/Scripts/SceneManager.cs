using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that manages the overall scene
//Author - Shubham Sachdeva
//Restrictions - None
public class SceneManager : MonoBehaviour
{
    //Fields
    public Vector3 flockCenter;
    public Vector3 flockDirection;
    public GameObject boid;
    public List<GameObject> boids;
    public GameObject pathFollower;
    public List<GameObject> obstacles;
    public GameObject sphere;
    public GameObject sphereReference;
    public List<GameObject> waypoints;

    //Fields for debugging
    public GameObject centerOfFlock;
    bool drawDebug;
   [SerializeField]Material debugRed;
   [SerializeField]Material debugOrange;



    // Use this for initialization
    void Start()
    {
        //Making a list for the boids and instantiating them
        boids = new List<GameObject>();

        for (int i = 0; i < 15; i++)
        {
            boids.Add(InstantiateObject(boid));
        }

        //Debud object for the average position of the flock
        centerOfFlock = Instantiate(centerOfFlock, flockCenter, Quaternion.identity);

        //Sending the reference to the center of the flock to to back and front cameras so
        //that they can follow it
        GameObject.Find("FlockCenterCamera").GetComponent<SmoothFollow>().target = centerOfFlock.transform;

        GameObject.Find("FlockFrontCamera").GetComponent<SmoothFollowFront>().target = centerOfFlock.transform;

        //No debug lines show initially
        drawDebug = false;




    }

    // Update is called once per frame
    void Update()
    {
        //Calculate the avarage direction of flock
        flockDirection = Vector3.zero;
        for (int i = 0; i < boids.Count; i++)
        {
            //Sum all the boids' forwards
            flockDirection += boids[i].transform.forward;
        }

        //Calculating the average direction of the flockers
        Vector3 positionSum = Vector3.zero;
        for (int i = 0; i < boids.Count; i++)
        {
            //Adding all the positions
            positionSum += boids[i].transform.position;
        }

        //Dividing by the number of boids
        flockCenter = positionSum / boids.Count;

        //Center of the flock object is places at the average position of the flock
        centerOfFlock.transform.position = flockCenter;
        centerOfFlock.transform.rotation = Quaternion.LookRotation(flockDirection.normalized);

        //Toggle debug lines with D
        if(Input.GetKeyDown(KeyCode.D))
        {
            drawDebug = !drawDebug;
        }


    }

    /// <summary>
    /// Method to intantiate an object on the terrain
    /// </summary>
    /// <param name="agent">The agent being instantiated</param>
    /// <returns></returns>
    public GameObject InstantiateObject(GameObject agent)
    {

        //Getting the terrain bounds
        Terrain terrain = Terrain.activeTerrain;
        float maxXPos = terrain.terrainData.bounds.max.x;
        float maxZPos = terrain.terrainData.bounds.max.z;
        float minXpos = terrain.terrainData.bounds.min.x;
        float minZPos = terrain.terrainData.bounds.min.z;

        //Getting the x,y and z position
        float xPos = Random.Range(minXpos, maxXPos);
        float zPos = Random.Range(minZPos, maxZPos);
        float yPos = Random.Range(10, 20);

        //Instantiating
        return Instantiate(agent, new Vector3(xPos, yPos, zPos), Quaternion.identity);

    }


    //Method to show the debug lines
    private void OnRenderObject()
    {

        //If the user wants to see the debug lines
        if (drawDebug)
        {
            //Flock direction line
            debugOrange.SetPass(0);

            GL.Begin(GL.LINES);
            GL.Vertex(flockCenter); //Starting end point
            GL.Vertex(flockCenter + flockDirection); //Finishing end point
            GL.End();

            //Lines from waypoint to waypoint
            debugRed.SetPass(0);

            //Loop through waypoint and draw lines between successive waypoints
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (i + 1 < waypoints.Count)
                {
                    GL.Begin(GL.LINES);
                    GL.Vertex(waypoints[i].transform.position);
                    GL.Vertex(waypoints[i+1].transform.position);
                    GL.End();
                }

                //If its the last waypoint, connect it to the first one
                else
                {
                    GL.Begin(GL.LINES);
                    GL.Vertex(waypoints[i].transform.position);
                    GL.Vertex(waypoints[0].transform.position);
                    GL.End();
                }
            }

            //Make the center of flock object visible
            centerOfFlock.GetComponent<Renderer>().enabled = true;

        }

        else
        {
            //Else make it visible
            centerOfFlock.GetComponent<Renderer>().enabled = false;
        }

    }
}
