using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ARMarker
{
    public abstract class BaseChoiceButton<T>
        : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
        where T : class
    {

        [SerializeField]
        protected Button button;

        [SerializeField]
        protected RawImage rawImage;

        protected T cachedData;

        [SerializeField]
        protected ScrollRect cachedScrollRect;
        [SerializeField]
        protected RectTransform cachedDropArea;

        protected bool isPrefabSpawned;
        protected bool isDraggable;
        protected WorkLayer cachedWorkLayer;

        protected void SetImage(Texture2D texture)
        {
            rawImage.texture = texture;
        }

        public virtual void SetUp(T data, ScrollRect scrollRect,
            RectTransform dropArea, bool isDraggable)
        {
            if (data == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"Data is NULL!!!", gameObject);
                return;
            }

            cachedData = data;
            cachedScrollRect = scrollRect;
            cachedDropArea = dropArea;
            this.isDraggable = isDraggable;

            if (!isDraggable)
            {
                button.onClick.AddListener(() => AddLayer(data));
            }
        }

        protected abstract WorkLayer AddLayer(T data);

        #region EventSystems

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isDraggable)
            {
                cachedScrollRect?.OnBeginDrag(eventData);
                return;
            }

            if (!isPrefabSpawned)
            {
                cachedWorkLayer = AddLayer(cachedData);

                // ❌ Layer creation blocked (e.g. no marker selected)
                if (cachedWorkLayer == null)
                {
                    isPrefabSpawned = false;
                    return;
                }

                isPrefabSpawned = true;
                cachedWorkLayer.SetupInitialDrag(true);
            }


            cachedWorkLayer?.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDraggable)
            {
                cachedScrollRect?.OnDrag(eventData);
                return;
            }

            if (cachedWorkLayer == null)
            {
                return;
            }

            cachedWorkLayer.OnDrag(eventData);
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDraggable)
            {
                cachedScrollRect?.OnEndDrag(eventData);
                return;
            }

            if (cachedWorkLayer == null)
            {
                isPrefabSpawned = false;
                return;
            }

            isPrefabSpawned = false;
            cachedWorkLayer.OnEndDrag(eventData);
        }



        #endregion // EventSystems

        private bool IsCursorInDropAreaBounds(Vector2 screenPoint)
        {
            if (cachedScrollRect == null)
            {
                return false;
            }

            // Check if point is inside the ScrollRect

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                cachedDropArea,
                screenPoint,
                null, // For Canvas with Screen Space - Overlay
                out localPoint);

            // Check if the point is inside the dropArea and return it.
            return cachedDropArea.rect.Contains(localPoint);
        }

    }

}