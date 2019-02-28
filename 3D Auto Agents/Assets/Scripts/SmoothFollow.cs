using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to make a camera follow a character
//Author - Shubham Sachdeva
//Restrictions - NONE
public class SmoothFollow : MonoBehaviour
{
    //Fields
    public GameObject targetObject;
    public Transform target;
    public float distance = 3.0f;
    public float height = 1.50f;
    public float heightDamping = 2.0f;
    public float positionDamping =2.0f;
    public float rotationDamping = 2.0f;

	// Use this for initialization
	void Start ()
    {
        //Getting the reference to the target this camera will follow
        List<GameObject> zombies = GameObject.Find("AgentManager").GetComponent<AgentManager>().zombies;
        targetObject = zombies[0];
        target = targetObject.transform;

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void LateUpdate()
    {

        // Early out if we don't have a target
        if (!target)return;
        float wantedHeight = target.position.y + height;
        float currentHeight = transform.position.y;
        // Damp the height
        currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
        // Set the position of the camera
        Vector3 wantedPosition = target.position -target.forward * distance;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * positionDamping);
        // Adjust the height of the camera
        transform.position = new Vector3 (transform.position.x, currentHeight+0.1f, transform.position.z);
        // Set the forward to rotate with time
        transform.forward = Vector3.Lerp (transform.forward, target.forward , Time.deltaTime * rotationDamping);
    }
}
