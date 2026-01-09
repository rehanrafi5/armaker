using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class SFXChoiceButton : BaseChoiceButton<SFXLayerData>
    {

        [Space]

        [SerializeField]
        private TextMeshProUGUI textName;

        public override void SetUp(SFXLayerData data, ScrollRect scrollRect,
            RectTransform dropArea, bool isDraggable)
        {
            base.SetUp(data, scrollRect, dropArea, isDraggable);
            SetImage(data.Sprite.texture);
            textName.text = data.Clip.name;
        }

        protected override WorkLayer AddLayer(SFXLayerData data)
        {
            return WorkSpaceSingleton.Instance.AddSFXLayer(data);
        }

        public void RegisterOnClick(Action<SFXLayerData> listener)
        {
            if (listener == null)
            {
                return;
            }

            button.onClick.AddListener(() => listener.Invoke(cachedData));
        }

    }

}