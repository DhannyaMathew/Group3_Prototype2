using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardButtonSelect : MonoBehaviour {
	public EventSystem MenuSystem; 
	public List<GameObject> Menu = new List<GameObject>();
	int currSelected;
    public AudioClip buttonClick;

    //this allows users to select buttons on the main menu using the keyboard only
    public void Start(){
		currSelected = 0;	
	}



    public void Submitted()
    {
        Menu[currSelected].GetComponent<Button>().onClick.Invoke();
        //var pointer = new PointerEventData(EventSystem.current);
        //ExecuteEvents.Execute(Menu[currSelected], pointer, ExecuteEvents.pointerDownHandler);
        if (buttonClick != null)
        {
            SoundController.instance.RandomPitchandsfx(1, buttonClick);
        }
    }

	public void Update ()
	{

		if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S)) {
			if (currSelected < Menu.Count) {
				currSelected++;
			}
			if (currSelected > Menu.Count-1) {
				currSelected = 0;
			}
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)|| Input.GetKeyDown (KeyCode.W)) {
			if (currSelected > -1) {
				currSelected--;
			}
			if (currSelected < 0) {
				currSelected = Menu.Count - 1;
			}
		}

        MenuSystem.SetSelectedGameObject (Menu [currSelected]);	//sets the current selected item to the one selected in the eventsystem
	}
}
