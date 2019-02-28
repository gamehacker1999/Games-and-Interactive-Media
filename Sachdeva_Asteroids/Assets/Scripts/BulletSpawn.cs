using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author - Shubham Sachdeva
//Class that manages and spawns the bullets
//Restrictions or errors - NONE
public class BulletSpawn : MonoBehaviour
{
    //Fields
    public List<GameObject> bullets;
    public const int MaxNumber=3;
    [SerializeField]private GameObject bullet;
    //field to check if the bullet is still on the screen
    new Camera camera;

	// Use this for initialization
	void Start ()
    {
        //Getting the main camera from the unity scene
        camera = GameObject.FindObjectOfType<Camera>();
        bullets = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    /// <summary>
    /// Method shoots the bullet
    /// </summary>
    public void ShootBullet()
    {
        //Limiting the number of bullets on the screen
        if (bullets.Count < MaxNumber)
        {
            bullets.Add(Instantiate(bullet));
            
        }
    }

    /// <summary>
    /// Method to destroy the bullet
    /// </summary>
    public void DestroyBullet()
    {
        //Checking the bounds of the screen
        float totalCameraHeight = camera.orthographicSize ;
        float totalCameraWidth = totalCameraHeight * camera.aspect;

        //Checking if any bullet is out of the bounds of the screen
        if (bullets.Count > 0)  //If the list has some bullets
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                //Condition if bullet is outside of the screen
                if (bullets[i].transform.position.x>totalCameraWidth||
                    bullets[i].transform.position.x<-totalCameraWidth||
                    bullets[i].transform.position.y>totalCameraHeight||
                    bullets[i].transform.position.y<-totalCameraHeight)
                {
                    //Destroy the bullet
                    Destroy(bullets[i]);
                    bullets.RemoveAt(i);
                }
            }
        } 
    }
}
