using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Managers.Level_Controller.Level_Creator
{
    public class WaveformDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public MusicController musicController;
        private Image image;
        
        private bool isLmb;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            musicController.StartDrag();
        }

        public void OnDrag(PointerEventData eventData)
        {
            image.rectTransform.anchoredPosition += eventData.delta;
            
            musicController.DragWaveform(image.rectTransform.anchoredPosition);
            isLmb = UnityEngine.Input.GetMouseButton(0);
        }

        public void OnEndDrag(PointerEventData eventData)
        {

            musicController.StopDrag(isLmb);
        }
    }
}
