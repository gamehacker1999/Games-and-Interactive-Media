using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to manage all the agents in the scene
//Author - Shubham Sachdeva
//Restrictions - NONE
public class AgentManager : MonoBehaviour {

    //Fields
    public GameObject zombie;
    public GameObject human;
    public GameObject purpleSphere;

    public List<GameObject> zombies;
    public List<GameObject> humans;
    public List<GameObject> obstacles;

	// Use this for initialization
	void Start ()
    {

        zombies = new List<GameObject>();
        humans = new List<GameObject>();

        //Initializing the humans, zombies and the sphere
        for(int i =0;i<3; i++)
        {
            zombies.Add(InstantiateObject(zombie));
            humans.Add(InstantiateObject(human));
        }

        //Activating the smooth follow script after creating all the agents
        GameObject.Find("Character Camera").GetComponent<SmoothFollow>().enabled = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Spawn a zombie if user presses z
        if(Input.GetKeyDown(KeyCode.Z)&&zombies.Count<10)
        {
            zombies.Add(InstantiateObject(zombie));
        }

        //Spawn a human if user presses h
        if (Input.GetKeyDown(KeyCode.H) && humans.Count < 5)
        {
            humans.Add(InstantiateObject(human));
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

        //Getting the x and z position
        float xPos = Random.Range(minXpos, maxXPos);
        float zPos = Random.Range(minZPos, maxZPos);

        //finding the y position
        float yPos = terrain.SampleHeight(new Vector3(xPos, 0, zPos));
        //yPos += agent.transform.localScale.y/2;

        //Instantiating
        return Instantiate(agent, new Vector3(xPos, yPos, zPos), Quaternion.identity);

    }

}
