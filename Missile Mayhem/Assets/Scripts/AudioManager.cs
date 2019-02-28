using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author - Shubham Sachdeva
//Class to manage the audio in the game
//Restrictions - The fields used to play the song are static so they cannot be serialized
public class AudioManager : MonoBehaviour
{
    //Fields
    private static AudioSource audioSource;
    static AudioClip explosionSound;
    static AudioClip bulletSound;
    //fields to set the static variables using the inspector
    [SerializeField] AudioClip explosionSoundInspector;
    [SerializeField] AudioClip bulletSoundInspector;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this.gameObject); //So that the audio can play when changing the scenes
        audioSource = gameObject.GetComponent<AudioSource>();
        //Taking the fields from the inspector and putting their values in the static fields
        bulletSound = bulletSoundInspector;
        explosionSound = explosionSoundInspector;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /// <summary>
    /// Plays the sound when bullets are shot
    /// </summary>
    public static void PlayBulletSound()
    {
        audioSource.PlayOneShot(bulletSound);
    }

    /// <summary>
    /// Plays the sound when a collision occurs
    /// </summary>
    public static void PlayExplosionSound()
    {
        audioSource.PlayOneShot(explosionSound, 0.1f);
    }
}
