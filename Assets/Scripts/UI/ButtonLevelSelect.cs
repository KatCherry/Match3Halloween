using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonLevelSelect : MonoBehaviour
{
    [SerializeField] private int m_LevelNumber;
    private Button m_Level;

    void Start()
    {
        m_Level = GetComponent<Button>();
        m_Level.onClick.AddListener(SelectLevel);
    }

    private void SelectLevel()
    {
        SceneManager.LoadScene("Level"+ m_LevelNumber);
    }

    public int GetLevelNumber()
    {
        return m_LevelNumber;
    }
}
