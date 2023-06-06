using System;
using UnityEngine;

namespace Match3
{
    public class Cell
    {
        public event Action<Coordinates> OnClick;
        public UITile UITile => m_UICell.UITile;

        private UICell m_UICell;
        private bool m_IsMarkedToDestroy;

        public bool IsMarkedToDestroy
        {
            get => m_IsMarkedToDestroy;
            set
            {
                m_IsMarkedToDestroy = value;
                m_UICell.MarkAsDestroy(value);
            }
        }

        public Coordinates Coordinates { get; private set; }
        public TileType TileType => m_UICell.UITile.TileInfo.Type;

        public Cell(Coordinates coordinates, UICell uICell)
        {
            m_UICell = uICell;
            Coordinates = coordinates;
            AddUICellListener();
        }

        public void SetTileType(TileType tileType)
        {
            m_UICell.SetTile(tileType);
        }

        public void SetTileToEmpty()
        {
            m_UICell.SetTileToEmpty();
        }

        public void DestroyTile()
        {
            m_UICell.DestroyTile();
        }

        public void SwipeTile(UITile uITile, float duration, Action onComplete = null)
        {
            RemoveCellListener();
            m_UICell.SetNewTile(uITile, duration, onComplete);
            AddUICellListener();
        }

        private void InvokeOnClick()
        {
            OnClick?.Invoke(Coordinates);
        }

        ~Cell()
        {
            RemoveCellListener();
        }

        private void AddUICellListener()
        {
            m_UICell.OnClick += InvokeOnClick;
        }

        private void RemoveCellListener()
        {
            m_UICell.OnClick -= InvokeOnClick;
        }
    }
}