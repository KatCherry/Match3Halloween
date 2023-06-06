using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3
{
    [RequireComponent(typeof(Image))]
    public class UITile : MonoBehaviour, IPointerClickHandler
    {
        public event Action OnClick;
        public TileInfo TileInfo
        {
            get => m_TileInfo;
            set
            {
                m_TileInfo = value;
                m_TileImage.gameObject.SetActive(m_TileInfo.Sprite != null);
                m_TileImage.sprite = m_TileInfo.Sprite;
            }
        }

        [SerializeField] private Image m_TileImage;
        private TileInfo m_TileInfo;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public void MoveToPosition(Vector2 position, float duration, Action onAnimationComplete = null)
        {
            transform.DOLocalMove(position, duration).Play().OnComplete(() => onAnimationComplete?.Invoke());
        }

        private void OnValidate()
        {
            m_TileImage = GetComponent<Image>();
        }
    }
}