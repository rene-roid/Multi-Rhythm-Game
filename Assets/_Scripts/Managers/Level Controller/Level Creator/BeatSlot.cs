using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Managers.Level_Controller.Level_Creator
{
    public class BeatSlot : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            GameObject droppedObject = eventData.pointerDrag;
            DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                draggableItem.parentDrag = transform;
            }
        }
    }
}
