using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMarker
{
    [RequireComponent(typeof(BoxCollider))]
    public class LayerRotationHandler : BaseWorkLayerHandler,
        IPointerDownHandler,
        IDragHandler,
        IPointerUpHandler
    {
        [Header("Rotation UI")]
        [SerializeField] private GameObject rotationIndicators;
        [SerializeField] private GameObject xDial;
        [SerializeField] private GameObject yDial;
        [SerializeField] private GameObject zDial;

        [Header("Collider")]
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private Vector3 colliderSizeOnActivation;

        [Header("Settings")]
        [SerializeField] private float rotationSensitivity = 0.4f;

        private Camera mainCam;
        private bool isDragging;
        private Vector3 rotationAxis;
        private Vector3 originalColliderSize;

        private void Awake()
        {
            mainCam = Camera.main;
            originalColliderSize = boxCollider.size;
        }

        private void OnDisable()
        {
            boxCollider.size = originalColliderSize;
            ResetDragging();
        }

        // =======================
        // Selection
        // =======================

        public override void Select()
        {
            base.Select();
            rotationIndicators.SetActive(true);
            boxCollider.size = colliderSizeOnActivation;
        }

        public override void Deselect()
        {
            base.Deselect();
            rotationIndicators.SetActive(false);
            boxCollider.size = originalColliderSize;
            ResetDragging();
        }

        // =======================
        // Pointer Events
        // =======================

        public void OnDragStart()
        {
            mainCam = Camera.main;
            //rotationAxis = DetectAxis(eventData);

            Select();
            
            if (rotationAxis == Vector3.zero)
            {
                
                return;
            }

            isDragging = true;
        }
        public void OnDragging()
        {
            // if (!isDragging)
            //     return;
            //
            // ApplyRotation(eventData.delta);
        }
        public void OnDragEnd()
        {
            ResetDragging();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }

        // =======================
        // Rotation Logic
        // =======================

        private void ApplyRotation(Vector2 delta)
        {
            float deltaRotation = delta.magnitude * rotationSensitivity;

            float direction = Vector2.Dot(
                delta,
                GetAxisScreenDirection(rotationAxis)
            ) > 0 ? 1f : -1f;

            transform.Rotate(rotationAxis, direction * deltaRotation, Space.Self);
        }

        private Vector3 DetectAxis(PointerEventData eventData)
        {
            Ray ray = mainCam.ScreenPointToRay(eventData.position);

            if (!Physics.Raycast(ray, out RaycastHit hit))
                return Vector3.zero;

            GameObject hitObj = hit.collider.gameObject;

            if (hitObj == xDial)
                return Vector3.right;

            if (hitObj == yDial)
                return Vector3.up;

            if (hitObj == zDial)
                return Vector3.forward;

            return Vector3.zero;
        }

        private void ResetDragging()
        {
            isDragging = false;
            rotationAxis = Vector3.zero;
        }

        private Vector2 GetAxisScreenDirection(Vector3 worldAxis)
        {
            Vector3 screenStart = mainCam.WorldToScreenPoint(transform.position);
            Vector3 screenEnd = mainCam.WorldToScreenPoint(
                transform.position + transform.TransformDirection(worldAxis)
            );

            return (screenEnd - screenStart).normalized;
        }
    }
}
