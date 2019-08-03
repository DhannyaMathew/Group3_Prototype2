using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource[] Audio;
	public AudioSource MusicSource;
	public static SoundController instance = null;
	private float higherPitch = 1.00f;
	private float lowerPitch = 0.99f;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != null) {
			Destroy (gameObject);
		}
        if (AudioListener.volume == 0)
        {
            AudioListener.volume = 1;
        }
    }


	public void Playone (int AudioSourceNum, AudioClip clip)
	{

        Audio[AudioSourceNum].clip = clip;
        Audio[AudioSourceNum].Play ();
	}

	public void PlayBG (AudioClip clip)
	{

		MusicSource.clip = clip;
		MusicSource.Play ();
	}

	public void RandomPitchandsfx(int AudioSourceNum, params AudioClip[] clips)
	{ 		
		int randomIndex = Random.Range (0, clips.Length);			
		float randomPitch = Random.Range (lowerPitch, higherPitch);	
		Audio[AudioSourceNum].pitch = randomPitch;
        Audio[AudioSourceNum].clip = clips [randomIndex];                           

        if (!Audio[AudioSourceNum].isPlaying)
        {
            Audio[AudioSourceNum].Play();
        }


    }


}
