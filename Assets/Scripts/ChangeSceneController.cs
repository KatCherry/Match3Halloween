using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeSceneController : MonoBehaviour
{
    public event Action OnNewData;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void BackToLevelsGame()
    {
        SceneManager.LoadScene("Levels");
        Time.timeScale = 1;
        OnNewData?.Invoke();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Levels");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
