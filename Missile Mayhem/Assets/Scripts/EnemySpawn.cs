using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Author - Shubham Sachdeva
//Class that manages the enemies and also their collision with the bullets and the ship
//Restrictions or errors - NONE
public class EnemySpawn : MonoBehaviour
{
    //Fields
    List<GameObject> levelOne;
    List<GameObject> levelTwo;
    List<GameObject> levelThree;
    //object to hold the enemy
    public GameObject enemy;
    //List that holds different sprites
    public List<Sprite> enemySprites = new List<Sprite>();
    //Fields to get the collsion in this class
    public GameObject collisionManager;
    SATCollision collisionDetection;
    const int MaxCount=4; // Max number of large enemies

    // Use this for initialization
    void Start ()
    {
        levelOne = new List<GameObject>();
        levelTwo = new List<GameObject>();
        levelThree = new List<GameObject>();
        //Getting the Seperating axis collision class for the collision
        collisionDetection = collisionManager.GetComponent<SATCollision>();  	
	}
	
	// Update is called once per frame
	void Update ()
    {

        
		
	}

    /// <summary>
    /// Creates the large level one missile
    /// </summary>
    public void CreateMissile()
    {
        //Limiting the number of large enemies on the screen
        if (levelOne.Count < MaxCount)
        {
            Vector3 position;
            //Finding the camera
             Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            //Setting the initital position and direction randomly
            int randPos = Random.Range(1, 5);

            //Spawn at top edge
            if (randPos == 1)
            {
                position = new Vector3(Random.Range(-camera.orthographicSize * camera.aspect, camera.orthographicSize * camera.aspect),
                    camera.orthographicSize*2, 0);
            }
            
            //Spawn at bottom edge
            else if (randPos == 2)
            {
                position = new Vector3(Random.Range(-camera.orthographicSize * camera.aspect, camera.orthographicSize * camera.aspect),
                    -(camera.orthographicSize*2), 0);
            }

            //Spawn at right edge
            else if (randPos == 3)
            {
                position = new Vector3(camera.orthographicSize * camera.aspect * 2,
                     Random.Range(-camera.orthographicSize, camera.orthographicSize), 0);
            }

            //Spawn at left edge
            else
            {
                position = new Vector3(-camera.orthographicSize * camera.aspect*2,
                    Random.Range(-camera.orthographicSize, camera.orthographicSize), 0);
            }

            //Add the level one enemy to the list and instantiate it based on the random position
            levelOne.Add(Instantiate(enemy, position, Quaternion.identity));
            //Scaling it up to make it big
            levelOne[levelOne.Count-1].transform.localScale = new Vector3(2, 2, 0);
            //Selecting a random sprite for the enemy by going through and array
            levelOne[levelOne.Count - 1].GetComponentInChildren<SpriteRenderer>().sprite=enemySprites[Random.Range(0,3)];
            //Finding a random direction for the enemy to move in
            levelOne[levelOne.Count - 1].GetComponent<Enemy>().direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            levelOne[levelOne.Count - 1].GetComponent<Enemy>().direction.Normalize();

        }
    }

    /// <summary>
    /// Method to check a collision with the enemy
    /// </summary>
    /// <param name="gObject">The game object with which the missile might collide</param>
    /// <returns></returns>
    public bool MissileCollision(GameObject gObject)
    {
        //Making sure that there are enemies on the scene
        if (levelOne.Count > 0 || levelTwo.Count > 0 || levelThree.Count > 0)
        {
            //checking the collision of the level one asteroids with the ship
            for (int i = levelOne.Count - 1; i >= 0; i--)
            {
                if(collisionDetection.IsColliding(levelOne[i],gObject))
                {
                    //Deleting the game object and making two new enemies at the same spot
                    //Missile 1 spawn
                    //Spawning it at the same location as the bigger missile
                    levelTwo.Add(Instantiate(enemy,
                        new Vector3(levelOne[i].transform.position.x, levelOne[i].transform.position.y,0),
                        Quaternion.identity));
                    //Choosing a random sprite
                    levelTwo[levelTwo.Count - 1].GetComponentInChildren<SpriteRenderer>().sprite = enemySprites[Random.Range(0, 3)];
                    //Setting and appropriate scale to make it smaller
                    levelTwo[levelTwo.Count - 1].transform.localScale = new Vector3(1, 1, 0);
                    //Adding variation to the direction based on the current direction

                    //If the direction tends to the right hand side
                    if (Mathf.Abs(levelOne[i].GetComponent<Enemy>().direction.x) 
                        >= Mathf.Abs(levelOne[i].GetComponent<Enemy>().direction.y))
                    {
                        //Randomize the y direction
                        levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.y = 
                            levelOne[i].GetComponent<Enemy>().direction.y+Random.Range(-0.5f,0.5f);

                        levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.x =
                            levelOne[i].GetComponent<Enemy>().direction.x; 

                    }

                    //If the direction tends to the y direction
                    else
                    {
                       //Randomize the direction in x direction
                        levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.x =
                           levelOne[i].GetComponent<Enemy>().direction.x + Random.Range(-0.5f, -0.5f);

                        levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.y =
                            levelOne[i].GetComponent<Enemy>().direction.y;
                    }
                    //Normalizing the direction
                    levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.Normalize();

                    //Missile 2 spawn
                    //Spawning based on where the bigger missile was
                    levelTwo.Add(Instantiate(enemy,
                        new Vector3(levelOne[i].transform.position.x, levelOne[i].transform.position.y, 0),
                        Quaternion.identity));
                    //Selecting a random sprite
                    levelTwo[levelTwo.Count-1].GetComponentInChildren<SpriteRenderer>().sprite = enemySprites[Random.Range(0, 3)];
                    //Setting the appropriate scale
                    levelTwo[levelTwo.Count - 1].transform.localScale = new Vector3(1, 1, 0);

                    //Setting the direction based on the direction of the destroyed missile 

                    //If the destroyed missile tended to the x direction more
                    if (Mathf.Abs(levelOne[i].GetComponent<Enemy>().direction.x)
                          >= Mathf.Abs(levelOne[i].GetComponent<Enemy>().direction.y))
                    {
                        //Randomize y direction
                        levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.y =
                            levelOne[i].GetComponent<Enemy>().direction.y + Random.Range(-0.5f, 0.5f);

                        levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.x =
                            levelOne[i].GetComponent<Enemy>().direction.x;

                    }

                    //If it tended to the y direction
                    else
                    {
                        //randomize x direction
                        levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.x =
                           levelOne[i].GetComponent<Enemy>().direction.x + Random.Range(-0.5f, 0.5f);

                        levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.y =
                            levelOne[i].GetComponent<Enemy>().direction.y;
                    }
                    //Normalizing the direction
                    levelTwo[levelTwo.Count - 1].GetComponent<Enemy>().direction.Normalize();

                    //Adding 20 points to the score
                    Vehicle.score += 20;
                    //Destroying the missile and removing it from the list
                    Destroy(levelOne[i]);
                    levelOne.Remove(levelOne[i]);

                    //Checking to see if the gameobject is the ship
                    if (gObject.GetComponent<Vehicle>()!=null)
                    {
                        //Decrease ship health
                        Vehicle.shipHealth--;
                    }
                    return true;
                }
            }

            //checking the collision of the level two asteroids with the ship
            for (int i = levelTwo.Count - 1; i >= 0; i--)
            {
                if(collisionDetection.IsColliding(levelTwo[i],gObject))
                {
                    //Deleting the game object and making two new enemies at the same spot

                    //Missile 1 spawn
                    //Setting posision based on the parent missile
                    levelThree.Add(Instantiate(enemy,
                       new Vector3(levelTwo[i].transform.position.x , levelTwo[i].transform.position.y, 0),
                       Quaternion.identity));
                    //Randomizing sprite
                    levelThree[levelThree.Count - 1].GetComponentInChildren<SpriteRenderer>().sprite = enemySprites[Random.Range(0, 3)];
               
                    //Setting the direction based on where the bigger missile tended towards
                    if (Mathf.Abs(levelTwo[i].GetComponent<Enemy>().direction.x)
                          >= Mathf.Abs(levelTwo[i].GetComponent<Enemy>().direction.y))
                    {
                        //Randomize y direction if parent tended to x
                        levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.y =
                            levelTwo[i].GetComponent<Enemy>().direction.y + Random.Range(-0.5f, 0.5f);

                        levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.x =
                            levelTwo[i].GetComponent<Enemy>().direction.x;

                    }

                    else
                    {
                        //Randomize x direction if parent tended towards y
                        levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.x =
                           levelTwo[i].GetComponent<Enemy>().direction.x + Random.Range(-0.5f, 0.5f);

                        levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.y =
                            levelTwo[i].GetComponent<Enemy>().direction.y;

                    }
                    //Normalizing the direction
                    levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.Normalize();
                    //Setting a smaller scale
                    levelThree[levelThree.Count - 1].transform.localScale = new Vector3(0.5f, 0.5f, 0);

                    //Missile 2 spawn

                    //Setting the position based on the parennt
                    levelThree.Add(Instantiate(enemy,
                       new Vector3(levelTwo[i].transform.position.x , levelTwo[i].transform.position.y , 0),
                       Quaternion.identity));
                    //Getting a random sprite
                    levelThree[levelThree.Count - 1].GetComponentInChildren<SpriteRenderer>().sprite = enemySprites[Random.Range(0, 3)];

                    //If the parent was tendend towards x randomize y
                    if (Mathf.Abs(levelTwo[i].GetComponent<Enemy>().direction.x)
                          >= Mathf.Abs(levelTwo[i].GetComponent<Enemy>().direction.y))
                    {
                        levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.y =
                            levelTwo[i].GetComponent<Enemy>().direction.y + Random.Range(-0.5f, 0.5f);

                        levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.x =
                            levelTwo[i].GetComponent<Enemy>().direction.x;

                    }

                    //If the parent was tending towards y randomize x
                    else
                    {

                        levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.x =
                           levelTwo[i].GetComponent<Enemy>().direction.x + Random.Range(-0.5f, 0.5f);

                        levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.y =
                            levelTwo[i].GetComponent<Enemy>().direction.y;

                    }
                    //Normalizing the direction
                    levelThree[levelThree.Count - 1].GetComponent<Enemy>().direction.Normalize();
                    //Scaling it appropriately so it is smaller than parent
                    levelThree[levelThree.Count - 1].transform.localScale = new Vector3(0.5f, 0.5f, 0);

                    //Increasing the score of the player by 50
                    Vehicle.score += 50;

                    //Destroy the missile and remove it from list
                    Destroy(levelTwo[i]);
                    levelTwo.Remove(levelTwo[i]);

                    //Checking to see if the gameobject is the ship
                    if (gObject.GetComponent<Vehicle>() != null)
                    {
                        Vehicle.shipHealth--; // Decrease health
                    }
                    return true;
                }
            }

            //collision with level three asteroids
            for (int i = levelThree.Count - 1; i >= 0; i--)
            {
                //if (collisionDetection.AABBCollision(levelThree[i], gObject))
                if(collisionDetection.IsColliding(levelThree[i],gObject))
                {
                    //Increasing player score by 100
                    Vehicle.score += 100;
                    //Destroying the object and removing it from list
                    Destroy(levelThree[i]);
                    levelThree.Remove(levelThree[i]);

                    //Checking to see if the gameobject is the ship
                    if (gObject.GetComponent<Vehicle>() != null)
                    {
                        Vehicle.shipHealth--;// Decrease health
                    }
                    return true;
                }
            }
        }
        return false;
    }
}
