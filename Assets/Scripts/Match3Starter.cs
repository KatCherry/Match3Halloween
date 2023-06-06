using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Match3
{
    public class Match3Starter : MonoBehaviour
    {
        [SerializeField] private int m_ColumnCount;
        [SerializeField] private int m_RowCount;
        [SerializeField] private int m_CountInARow;
        [SerializeField] private int m_MaxCountOfBlocks;
        [SerializeField] private int m_StepsCountForLevel;
        [SerializeField] private int m_ScoreCountForLevel;
        [SerializeField] private UIBoard m_UIBoard;
        [SerializeField] private List<TileInfo> m_TileInfos;
        [SerializeField] private TMP_Text m_Score;
        [SerializeField] private TMP_Text m_Steps;
        [SerializeField] private PopUp m_PopUpWin;
        [SerializeField] private PopUp m_PopUpLose;
        [SerializeField] private ChangeSceneController m_SceneController;

        private Board m_Board;
        private bool m_AreSettingsUpdated;
        private int Score;
        private int Steps;

        private void Start()
        {
            Play();
        }

        private void SetScore(int score, int steps)
        {
            Score = score;
            Steps = steps;
            m_Score.text = Score.ToString();
            m_Steps.text = Steps.ToString();
            if (Steps == 0 && Score != m_ScoreCountForLevel)
            {
                LoosePopUpOpen();
            }
            if (Steps != 0 && Score >= m_ScoreCountForLevel)
            {
                WinPopUpOpen();
            }

        }

        private void LoosePopUpOpen()
        {
            Time.timeScale = 0;
            m_PopUpLose.gameObject.SetActive(true);
        }

        private void WinPopUpOpen()
        {
            Time.timeScale = 0;
            m_PopUpWin.gameObject.SetActive(true);
        }

        public void Play()
        {
            if (m_Board == null)
            {
                InstantiateBoard();
            }

            if (m_AreSettingsUpdated)
            {
                UpdateSettings();
            }
            m_Board.OnGetScore += SetScore;
            m_Board.Start();
            m_Board.SetStepsCount(m_StepsCountForLevel);
            m_PopUpLose.OnReset += Reset;
            m_SceneController.OnNewData += ResetForNewScene;
        }

        private void ResetForNewScene()
        {
            m_Board.Reset();
            Time.timeScale = 1;
        }

        private void Reset()
        {
            m_Board.Reset();
            Play();
            Time.timeScale = 1;
        }

        private void UpdateSettings()
        {
            if (m_Board != null)
            {
                m_Board.UpdateModel(GetBoardModel());
            }
        }

        private void InstantiateBoard()
        {
            m_Board = new Board(GetBoardModel());
            m_AreSettingsUpdated = false;
        }

        private BoardModel GetBoardModel()
        {
            return new BoardModel(m_UIBoard, m_TileInfos, m_CountInARow, m_ColumnCount, m_RowCount, m_MaxCountOfBlocks, m_StepsCountForLevel, m_ScoreCountForLevel);
        }

        private void OnValidate()
        {
            m_AreSettingsUpdated = true;
        }
    }
}