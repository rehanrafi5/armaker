using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{
    public class GIFChoiceButton : BaseChoiceButton<AnimatorLayerData>
    {
        public override void SetUp(AnimatorLayerData data, ScrollRect scrollRect,
            RectTransform dropArea, bool isDraggable)
        {
            base.SetUp(data, scrollRect, dropArea, isDraggable);
            SetImage(data.Sprite.texture);
        }

        protected override WorkLayer AddLayer(AnimatorLayerData data)
        {
            // 🚫 BLOCK GIF placement if no marker is selected
            if (!GameManager.Instance.HasValidMarker())
            {
                GameManager.Instance.RaiseNoMarkerError();
                return null;
            }

            return WorkSpaceSingleton.Instance.AddAnimatedLayer(data);
        }
    }
}