using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class UIBoard : MonoBehaviour
    {
        [SerializeField] private UICell m_UICellPrefab;
        [SerializeField] private GameObject m_CellContainer;
        [SerializeField] private Camera m_Camera;

        private float m_Weight;
        private float m_Height;
        private int m_XCoordinate;
        private int m_YCoordinate;
        private UICell[,] m_UICells;
        private List<TileInfo> m_TileInfos;
        private float m_Xi;
        private float m_Yi;

        public void Init(List<TileInfo> tileInfos)
        {
            m_TileInfos = tileInfos;
        }

        public void CreateCells(int columntCount, int rowCount)
        {
            m_XCoordinate = columntCount;
            m_YCoordinate = rowCount;

            m_Height = m_Camera.orthographicSize * 2;
            m_Weight = m_Height * m_Camera.aspect;

            var blockSize = 1;
            var delta = 0.025f;
            var scaleX = (m_Weight * 0.9f - delta * (m_XCoordinate - 1)) / (m_XCoordinate * blockSize);
            var scaleY = (m_Height * 0.9f - delta * (m_YCoordinate - 1)) / (m_YCoordinate * blockSize);
            var scale = Mathf.Min(scaleX, scaleY);

            var boardH = m_YCoordinate * blockSize * scale + delta * (m_YCoordinate - 1);
            var boardW = m_XCoordinate * blockSize * scale + delta * (m_XCoordinate - 1);

            var x0 = -boardW / 2;
            var y0 = boardH / 2;

            m_UICells = new UICell[m_XCoordinate, m_YCoordinate];

            for (int i = 0; i < m_XCoordinate; i++)
            {
                m_Xi = x0 + blockSize * (i + 0.5f) * scale + delta * i;
                for (int j = 0; j < m_YCoordinate; j++)
                {
                    m_Yi = y0 - blockSize * (j + 0.5f) * scale - delta * j;
                    UICell uICell = Instantiate(m_UICellPrefab, new Vector2(m_Xi, m_Yi), m_CellContainer.transform.rotation, m_CellContainer.transform);
                    uICell.transform.localScale = new Vector3(scale * 0.95f, scale * 0.95f, 1);
                    uICell.transform.localPosition = new Vector3(uICell.transform.localPosition.x, uICell.transform.localPosition.y, 0f);
                    uICell.Init(m_TileInfos);
                    m_UICells[i, j] = uICell;
                }
            }
        }

        public void DestroyCells()
        {
            foreach (var cell in m_UICells)
            {
                Destroy(cell.gameObject);
            }

            Array.Clear(m_UICells, 0, m_UICells.Length);
        }

        public UICell GetUICell(Coordinates coordinates)
        {
            return m_UICells[coordinates.X, coordinates.Y];
        }
    }
}