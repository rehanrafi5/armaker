using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{
    public class SpriteChoiceButton : BaseChoiceButton<Sprite>
    {
        public override void SetUp(
            Sprite sprite,
            ScrollRect scrollRect,
            RectTransform dropArea,
            bool isDraggable
        )
        {
            base.SetUp(sprite, scrollRect, dropArea, isDraggable);
            SetImage(sprite.texture);
        }

        protected override WorkLayer AddLayer(Sprite data)
        {
            // 🚫 BLOCK asset placement if no marker is selected
            if (!GameManager.Instance.HasValidMarker())
            {
                GameManager.Instance.RaiseNoMarkerError();
                return null;
            }

            return WorkSpaceSingleton.Instance.AddLayer(data);
        }
    }
}