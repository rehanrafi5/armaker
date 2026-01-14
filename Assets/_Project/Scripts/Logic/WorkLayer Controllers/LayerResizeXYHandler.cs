using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMarker
{
    public class LayerResizeXYHandler : BaseWorkLayerHandler, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private float scaleSensitivity = 0.003f;
        [SerializeField] private float minScale = 0.05f;
        [SerializeField] private float maxScale = 5f;

        private Vector2 startPointerPos;
        private Vector3 startScale;
        private Camera mainCam;

        [Header("UI")]
        public GameObject ScaleMarker;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        // =======================
        // Selection
        // =======================

        public override void Select()
        {
            base.Select();
            mainCam = Camera.main;
            if (ScaleMarker != null)
                ScaleMarker.SetActive(true);
        }

        public override void Deselect()
        {
            base.Deselect();
            if (ScaleMarker != null)
                ScaleMarker.SetActive(false);
        }

        // =======================
        // Pointer Events
        // =======================

        public void OnPointerDown(PointerEventData eventData)
        {
            Select();
            startPointerPos = eventData.position;
            startScale = transform.localScale;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Keep selection active while dragging
            Select();

            Vector2 delta = eventData.position - startPointerPos;

            // Project delta onto local X and Y directions
            Vector2 screenLocalX = GetAxisScreenDirection(transform.right);
            Vector2 screenLocalY = GetAxisScreenDirection(transform.up);

            float scaleDeltaX = Vector2.Dot(delta, screenLocalX) * scaleSensitivity;
            float scaleDeltaY = Vector2.Dot(delta, screenLocalY) * scaleSensitivity;

            float newScaleX = Mathf.Clamp(startScale.x * (1f + scaleDeltaX), minScale, maxScale);
            float newScaleY = Mathf.Clamp(startScale.y * (1f + scaleDeltaY), minScale, maxScale);

            transform.localScale = new Vector3(newScaleX, newScaleY, startScale.z);
        }

        // =======================
        // Helper
        // =======================

        private Vector2 GetAxisScreenDirection(Vector3 worldAxis)
        {
            Vector3 screenStart = mainCam.WorldToScreenPoint(transform.position);
            Vector3 screenEnd = mainCam.WorldToScreenPoint(transform.position + worldAxis);
            return (screenEnd - screenStart).normalized;
        }
    }
}
