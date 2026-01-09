using UnityEngine;

namespace ARMarker
{

    public class SafeAreaAdjuster : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private ScreenOrientation orientation;

        private Rect lastSafeArea = new Rect(0, 0, 0, 0);
        
        private void OnEnable()
        {
            ApplySafeArea();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (Screen.safeArea != lastSafeArea 
                || Screen.orientation != orientation)
            {
                ApplySafeArea();
            }
        }

        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;
            lastSafeArea = safeArea;
            orientation = Screen.orientation;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

    }

}