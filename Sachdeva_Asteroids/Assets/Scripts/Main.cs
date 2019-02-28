using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Author Shubham Sachdeva
//Class that handles the overall game
//Restrictions or errors - NONE
public class Main : MonoBehaviour
{
    //Fields
    EnemySpawn enemyManager;
    BulletSpawn bulletManager;
    Vehicle ship;
    private AudioSource audioSource;
    [SerializeField] AudioClip explosionSound;

	// Use this for initialization
	void Start ()
    {
        ship = GameObject.Find("Ship").GetComponent<Vehicle>();
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemySpawn>();
        bulletManager = GameObject.Find("BulletManager").GetComponent<BulletSpawn>();
        audioSource = gameObject.GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update()
    {
        //Calling the relevant methods for the ship
        ship.Drive();
        ship.RotateVehicle();
        ship.SetTransform();
        ship.WrapAround();

        //Creating a missile
        enemyManager.CreateMissile();

        //collision of the ship with asteroids
        if (!ship.damaged) //Checking to see if the ship was just damages
        { 
            if (enemyManager.MissileCollision(ship.gameObject))//Collision between ship and asteroid
            {
                AudioManager.PlayExplosionSound(); //play collision sound
                ship.damaged = true;
                //Changing the color to red
                ship.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                //Make the ship invincible for 3 seconds
                Invoke("Invincible", 3);
            }
        }

        //Checking to see if ship has destroyed
        //If health is 0 then switch to  game over scene
        if(Vehicle.shipHealth<=0)
        {
            SceneManager.LoadScene("GameOver");
        }

        //Checking the collision with all bullets on the screen
        for(int i = 0; i<bulletManager.bullets.Count;i++)
        {  
            if (enemyManager.MissileCollision(bulletManager.bullets[i])) //Collison with bullets and asteroids
            {
                AudioManager.PlayExplosionSound();
                Destroy(bulletManager.bullets[i]);
                bulletManager.bullets.Remove(bulletManager.bullets[i]);   
            }
        }
        
        //If user pressses space, shoot bullet
        if(Input.GetKeyDown(KeyCode.Space))
        {
            bulletManager.ShootBullet();
        }
        //Check every frame to see if the bullet should be destroyed
        bulletManager.DestroyBullet(); 
	}

    //method to make the ship invincible for a few seconds after it has been hit
    private void Invincible()
    {
        ship.damaged = false;
        ship.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
}
