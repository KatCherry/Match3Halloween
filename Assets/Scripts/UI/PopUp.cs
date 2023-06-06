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
    public event Action OnUpdateBoard;
    [SerializeField] private Button m_RestartButton;
    [SerializeField] private Button m_NextLevelButton;
    [SerializeField] private int m_NextLevelNumber;

    void Start()
    {
        m_RestartButton.onClick.AddListener(Reset);
        m_NextLevelButton.onClick.AddListener(SelectLevel);
    }

    private void SelectLevel()
    {
        OnUpdateBoard?.Invoke();
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("Level"+ m_NextLevelNumber);
    }

    private void Reset()
    {
        this.gameObject.SetActive(false);
        OnReset?.Invoke();
    }
}
