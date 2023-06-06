using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class UICell : MonoBehaviour
    {

        [SerializeField] private GameObject m_DestroyMarker;
        [SerializeField] private UITile m_UITile;
        [SerializeField] private GameObject m_TileContainer;

        private List<TileInfo> m_TileInfos;

        public event Action OnClick
        {
            add
            {
                m_UITile.OnClick += value;
            }
            remove
            {
                m_UITile.OnClick -= value;
            }
        }

        public UITile UITile => m_UITile;

        public void Init(List<TileInfo> tileInfos)
        {
            m_TileInfos = tileInfos;
        }

        public void SetNewTile(UITile uITile, float duration, Action onComplete = null)
        {
            m_UITile = uITile;
            m_UITile.gameObject.transform.SetParent(m_TileContainer.transform, true);
            m_UITile.MoveToPosition(m_TileContainer.transform.localPosition, GetDuration(uITile, duration), onComplete);
        }

        public void SetTile(TileType tileType)
        {
            m_UITile.TileInfo = GetTileInfo(tileType);
        }

        public void MarkAsDestroy(bool isDestroy)
        {
            m_DestroyMarker.SetActive(isDestroy);
        }

        public void SetTileToEmpty()
        {
            m_UITile.TileInfo = GetTileInfo(TileType.Empty);
            m_DestroyMarker.SetActive(false);
        }

        public void DestroyTile()
        {
            Destroy(m_UITile);
        }

        private TileInfo GetTileInfo(TileType tileType)
        {
            var tileInfo = new TileInfo();
            foreach (var i in m_TileInfos)
            {
                if(i.Type == tileType)
                {
                    tileInfo = i;
                }
                
            }
            return tileInfo;
        }

        private static float GetDuration(UITile uITile, float duration)
        {
            return uITile.TileInfo.Type == TileType.Empty ? 0 : duration;
        }
    }
}