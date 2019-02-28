using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{

    //camera array that holds all cameras
    public Camera[] cameras;

    private int currentCameraIndex;

	// Use this for initialization
	void Start ()
    {
        currentCameraIndex = 0;

        //turn off all cameras except the current one
        for (int i= 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        //if any cameras were added then enable the first one
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }

    }
	
	// Update is called once per frame
	void Update ()
    {

        //change camera if user presses the c button
        if(Input.GetKeyDown(KeyCode.C))
        {

            currentCameraIndex++;
            //if the index is in bounds then switch camera
            if (currentCameraIndex < cameras.Length)
            {

                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                cameras[currentCameraIndex].gameObject.SetActive(true);


            }

            //else switch to the first camera

            else
            {

                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                currentCameraIndex = 0;
                cameras[currentCameraIndex].gameObject.SetActive(true);


            }

        }

        
		
	}


}
