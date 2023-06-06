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

    public void StartGame()
    {
        SceneManager.LoadScene("Levels");
        OnNewData?.Invoke();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
