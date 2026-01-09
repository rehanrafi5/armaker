using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectSnapOnEnable : MonoBehaviour
    {
        public enum SnapPosition
        {
            TOP,
            BOTTOM,
            LEFT,
            RIGHT
        }

        [SerializeField] 
        private SnapPosition snapPosition = SnapPosition.TOP;
        
        private ScrollRect targetScrollRect;

        private void Awake()
        {
            targetScrollRect = GetComponent<ScrollRect>();
        }

        private void OnEnable()
        {
            if (targetScrollRect == null) return;

            switch (snapPosition)
            {
                case SnapPosition.TOP:
                    targetScrollRect.verticalNormalizedPosition = 1f;
                    break;
                case SnapPosition.BOTTOM:
                    targetScrollRect.verticalNormalizedPosition = 0f;
                    break;
                case SnapPosition.LEFT:
                    targetScrollRect.horizontalNormalizedPosition = 0f;
                    break;
                case SnapPosition.RIGHT:
                    targetScrollRect.horizontalNormalizedPosition = 1f;
                    break;
            }
        }
    }

}