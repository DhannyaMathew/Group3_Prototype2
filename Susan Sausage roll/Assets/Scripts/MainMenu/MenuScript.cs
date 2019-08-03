using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
   
    void Awake()
    {
       
        if (FadeEffect.instance != null)
        {
            FadeEffect.instance.FadeOut(0.5f);
        }
    }

    public void DelayedPlayGame()
    {
        if (FadeEffect.instance != null)
        {
            FadeEffect.instance.FadeIn(0.5f);
        }
        Time.timeScale = 1f;
        Invoke("PlayGame", 0.5f);
    }

    public void DelayedQuitGame()
    {
        if (FadeEffect.instance != null)
        {
            FadeEffect.instance.FadeIn(0.5f);
        }
        Invoke("Quit", 0.5f);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
