using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMarker
{

    /// <summary>
    /// NOTE: Please refine and revamp this code soon. 
    /// Right now, it's at this dirty/prototype state to save time for prototyping.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class LayerResizeXYHandler : BaseWorkLayerHandler, IPointerDownHandler, IDragHandler
    {
        [SerializeField]
        [Tooltip("Radius normalized to scale (i.e., 0.1 at scale (1,1,1))")]
        private float cornerRadius = 0.1f;

        private Camera mainCamera;
        private Vector3 initialPointerWorldPos;
        private Vector3 initialObjectScale;
        private Vector3 localCenter;
        private bool isCornerDrag = false;
        private Vector2 cornerDirection;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            mainCamera = Camera.main;
            Select();

            Vector3 worldPoint;
            if (!ScreenPointToWorldOnPlane(eventData.position, out worldPoint))
                return;

            initialPointerWorldPos = worldPoint;
            initialObjectScale = transform.localScale;

            BoxCollider box = GetComponent<BoxCollider>();
            localCenter = transform.TransformPoint(box.center);

            Vector3 localClickPos = transform.InverseTransformPoint(worldPoint);
            Vector3 extents = box.size * 0.5f;

            isCornerDrag = false;

            // Adjust effective radius based on current object scale (average of X and Y for uniformity)
            float averageScaleXY = (initialObjectScale.x + initialObjectScale.y) * 0.5f;
            float scaledCornerRadius = cornerRadius / averageScaleXY;

            // Check corners
            Vector3[] corners = new Vector3[]
            {
            new Vector3(-extents.x,  extents.y), // Top-left
            new Vector3( extents.x,  extents.y), // Top-right
            new Vector3(-extents.x, -extents.y), // Bottom-left
            new Vector3( extents.x, -extents.y), // Bottom-right
            };

            foreach (Vector3 corner in corners)
            {
                if (Vector3.Distance(localClickPos, corner) <= scaledCornerRadius)
                {
                    isCornerDrag = true;
                    cornerDirection = corner.normalized;
                    break;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Select();
            Vector3 currentWorldPos;
            if (!ScreenPointToWorldOnPlane(eventData.position, out currentWorldPos))
                return;

            Vector3 dragVector = currentWorldPos - initialPointerWorldPos;

            if (isCornerDrag)
            {
                // Corner resizing (independent X and Y)
                Vector3 localDelta = transform.InverseTransformVector(dragVector);
                float deltaX = cornerDirection.x * localDelta.x;
                float deltaY = cornerDirection.y * localDelta.y;

                Vector3 newScale = initialObjectScale;
                newScale.x = Mathf.Max(0.1f, initialObjectScale.x + deltaX);
                newScale.y = Mathf.Max(0.1f, initialObjectScale.y + deltaY);
                transform.localScale = new Vector3(newScale.x, newScale.y, initialObjectScale.z);
            }
            else
            {
                // Uniform resizing based on distance to center
                float initialDist = Vector3.Distance(initialPointerWorldPos, localCenter);
                float currentDist = Vector3.Distance(currentWorldPos, localCenter);
                float scaleFactor = currentDist / initialDist;

                scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 10f); // Reasonable limits

                Vector3 newScale = initialObjectScale * scaleFactor;
                transform.localScale = new Vector3(newScale.x, newScale.y, initialObjectScale.z);
            }
        }

        private bool ScreenPointToWorldOnPlane(Vector2 screenPoint, out Vector3 worldPoint)
        {
            worldPoint = Vector3.zero;
            Ray ray = mainCamera.ScreenPointToRay(screenPoint);
            Plane plane = new Plane(transform.forward, transform.position);

            float enter;
            if (plane.Raycast(ray, out enter))
            {
                worldPoint = ray.GetPoint(enter);
                return true;
            }
            return false;
        }

    }

}