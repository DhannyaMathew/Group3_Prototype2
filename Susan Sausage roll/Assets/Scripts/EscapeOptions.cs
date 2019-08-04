using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeOptions : MonoBehaviour
{
    public GameObject PauseScreen;
    public GameObject Lose;
    private bool GamePaused = false;
    private bool hadLost = false;

    void Pause()
    {
        if (Lose.activeInHierarchy)
        {
            Lose.SetActive(false);
            hadLost = true;
        }
        PauseScreen.SetActive(true);
        Time.timeScale = 0f;                //freezes the game
        GamePaused = true;
    }

    public void Resume()
    {
        if (hadLost)
        {
            Lose.SetActive(true);
        }
        PauseScreen.SetActive(false);
        Time.timeScale = 1f;                //resumes the game
        GamePaused = false;
    }
    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

    }
    public void Quit()
    {
        Application.Quit();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused == true)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
}
