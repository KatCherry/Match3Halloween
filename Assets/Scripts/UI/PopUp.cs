using Match3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public event Action OnReset;

    [SerializeField] private Button m_RestartButton;
    [SerializeField] private Button m_SelectLevelButton;

    void Start()
    {
        m_RestartButton.onClick.AddListener(Reset);
        m_SelectLevelButton.onClick.AddListener(SelectLevel);
    }

    private void SelectLevel()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScene");
    }

    private void Reset()
    {
        this.gameObject.SetActive(false);
        OnReset?.Invoke();
    }
}
