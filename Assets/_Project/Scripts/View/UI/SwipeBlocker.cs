using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ARMarker
{

    public class SwipeBlocker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
    {
        [SerializeField] private ScrollRect parentScrollRect;

        private Vector2 dragStartPos;
        private bool blockScrollRect;

        public void SetScrollRect(ScrollRect scrollRect)
        { 
            parentScrollRect = scrollRect;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            // Prevent ScrollRect from hijacking drag at the start
            eventData.useDragThreshold = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragStartPos = eventData.position;
            blockScrollRect = false;

            if (parentScrollRect != null)
                parentScrollRect.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 dragDelta = eventData.position - dragStartPos;

            if (!blockScrollRect)
            {
                if ((parentScrollRect.vertical
                    && Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
                    || (parentScrollRect.horizontal
                    && Mathf.Abs(dragDelta.x) < Mathf.Abs(dragDelta.y)))
                {
                    blockScrollRect = true;
                }
                else
                {
                    if (parentScrollRect != null)
                        parentScrollRect.OnDrag(eventData);
                }
            }
            //else
            //{
            //    // Handle horizontal drag here, if needed
            //    // e.g., trigger swipe logic or animation
            //}
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!blockScrollRect && parentScrollRect != null)
            {
                parentScrollRect.OnEndDrag(eventData);
            }
        }

    }

}