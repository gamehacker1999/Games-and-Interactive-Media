using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

//Author - Shubham Sachdeva
//Class that manages the button click, it inherits from the interface IPointerClickHandler, IPointerEnterHandler
//and IPointerExitHandler for mouse click event handling
//redtrictions - Not reusable since it only handles the scenes within this game
public class ButtonClick : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /// <summary>
    /// Method that checks if the button was clicked
    /// </summary>
    /// <param name="eventData">the event that occured with the button</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        //If start button was clicked, go to instruction
        if (gameObject.name == "Start")
        {
            SceneManager.LoadScene("Instructions");
        }
         
        //If continue button was clicked, start game
        if (gameObject.name == "Continue")
        {
            SceneManager.LoadScene("Game");
        }

        //If back was clicked, go to Instructions
        if (gameObject.name == "Back")
        {
            SceneManager.LoadScene("Introductions");
        }

        //If quit is clicked, end game
        if (gameObject.name == "Quit")
        {
            Application.Quit();
        }

        //If retry is clicked, start game again
        if (gameObject.name == "Retry")
        {
            SceneManager.LoadScene("Game");
        }

        //If exit is clicked, back to introduction
        if (gameObject.name == "Exit")
        {
            SceneManager.LoadScene("Introductions");
        }
   
    }

    /// <summary>
    /// Turn the button red when mouse hovers over
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = Color.red;
    }

    /// <summary>
    /// Turn the button back to normal after the mouse leaves the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = Color.white;
    }
}
