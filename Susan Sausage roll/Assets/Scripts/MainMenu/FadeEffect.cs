using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
	//fade screen attached to the camera on the menu screen.
	public Image BlackScreen;
	public static FadeEffect instance = null;
	// Use this for initialization
	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != null) {
			Destroy (gameObject);
		}
	}

	void Start ()
	{
		
		BlackScreen.canvasRenderer.SetAlpha (1f);
	}

	//fade into black
	public void FadeIn (float speed)
	{
		BlackScreen.canvasRenderer.SetAlpha (0f);
		BlackScreen.CrossFadeAlpha (1f, speed, false);
	}

	//fade out of black
	public void FadeOut (float speed)
	{
		BlackScreen.canvasRenderer.SetAlpha (1f);
		BlackScreen.CrossFadeAlpha (0f, speed, false);
	}
}
