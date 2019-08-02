using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;//required when dealing with event data
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{  
    //behaviour inherited for on select and on press events and functions

    BaseEventData buttonEvent;
    public AudioClip buttonHover;
    public AudioClip buttonClick;
    //do this whene UI is hovered
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundController.instance.RandomPitchandsfx(0,buttonHover);
    }
    //do this whene UI is selected
    public void OnSelect(BaseEventData eventData)
    {
        if (buttonClick != null)
        {
            SoundController.instance.RandomPitchandsfx(1,buttonClick);
        }
    }
}
