using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grill : MonoBehaviour
{
    public Material offMat;
    public Material onMat;
    public AudioClip on, off;
    public bool IsOn { get; private set; }

    public void TurnOn()
    {
        IsOn = true;
        GetComponentInChildren<Renderer>().material = onMat;
    }

    public void TurnOff()
    {
        IsOn = false;
        GetComponentInChildren<Renderer>().material = offMat;
    }

    public AudioClip Sound()
    {
        return IsOn ? on : off;
    }
}