using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Managers.Level_Controller.Level_Creator
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Image image;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        [HideInInspector] public Transform parentDrag;
        public void OnBeginDrag(PointerEventData eventData)
        {
            parentDrag = transform.parent;
            transform.SetParent(parentDrag.parent);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = UnityEngine.Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(parentDrag);
            image.raycastTarget = true;
        }
    }
}