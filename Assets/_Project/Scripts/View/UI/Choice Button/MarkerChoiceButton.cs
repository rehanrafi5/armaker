using System;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class MarkerChoiceButton : MonoBehaviour
    {

        [SerializeField]
        private Button button;

        [SerializeField]
        private RawImage rawImage;

        [SerializeField]
        private GameObject selectedIndicator;

        [Space]

        private Sprite cachedSprite;

        private void Awake()
        {
            selectedIndicator.SetActive(false);
        }

        public void SetUp(Sprite marker, 
            Action<MarkerChoiceButton> listener)
        {
            if (listener != null)
            {
                button.onClick.AddListener(
                    () => listener.Invoke(this));
            }
            
            rawImage.texture = marker.texture;
            cachedSprite = marker;
            SetIsSelected(false);
        }

        public void SetIsSelected(bool isSelected) 
        {
            selectedIndicator.SetActive(isSelected);
        }

        public Sprite GetMarker() => cachedSprite;

    }

}