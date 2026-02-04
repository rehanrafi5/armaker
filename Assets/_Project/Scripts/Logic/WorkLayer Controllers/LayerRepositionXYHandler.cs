using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMarker
{

    public class LayerRepositionXYHandler : BaseWorkLayerHandler, 
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {

        [SerializeField]
        private GameObject repositionArrows;

        private Vector3 offset;
        private bool dragging;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        public override void Select()
        {
            base.Select();
            repositionArrows.SetActive(true);
        }

        public override void Deselect()
        {
            base.Deselect();
            repositionArrows.SetActive(false);
        }

        public void OnDragStart()
        {
            mainCamera = Camera.main;
            Select();
        }
        public void OnDragging()
        {
            Select();
        }
        public void OnDragEnd()
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //mainCamera = Camera.main;
            //Select();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //Vector3 inputPos = eventData.position;
            //offset = transform.position - ScreenToWorld(inputPos, transform.position.z);
            //dragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // if (dragging)
            // {
            //     Select();
            //     Vector3 newPos = ScreenToWorld(
            //         eventData.position, transform.position.z) + offset;
            //     newPos.z = transform.position.z;
            //     transform.position = newPos;
            // }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // dragging = false;
        }

        private Vector3 ScreenToWorld(Vector3 screenPos, float z)
        {
            screenPos.z = z - mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(screenPos);
        }

    }

}