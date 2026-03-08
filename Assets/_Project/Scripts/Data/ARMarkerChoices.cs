using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARMarker
{

    [CreateAssetMenu(
        menuName = ConstantStrings.MENU_ROOT + "Create AR Marker Choice List")]
    public class ARMarkerChoices : ScriptableObject
    {

        public ARImageUIData[] MarkerData;
        
        [SerializeField]
        private List<Sprite> choices = new();

        public List<Sprite> Choices => choices;

    }
    [Serializable]
    public class ARImageUIData
    {
        public string imageName;
        public Sprite previewSprite;
    }
}