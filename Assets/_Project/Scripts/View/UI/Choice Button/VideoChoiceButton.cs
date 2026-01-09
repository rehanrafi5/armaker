using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{
    public class VideoChoiceButton : BaseChoiceButton<VideoLayerData>
    {

        public override void SetUp(VideoLayerData data, ScrollRect scrollRect,
            RectTransform dropArea, bool isDraggable)
        {
            base.SetUp(data, scrollRect, dropArea, isDraggable);
            SetImage(data.Sprite.texture);
        }

        protected override WorkLayer AddLayer(VideoLayerData data)
        {
            return WorkSpaceSingleton.Instance.AddVideoLayer(data);
        }
    }

}