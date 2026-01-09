using UnityEngine;

namespace ARMarker
{

    [System.Serializable]
    public class AnimatorLayerData
    {

        [SerializeField]
        private Sprite sprite;

        [SerializeField]
        private RuntimeAnimatorController controller;

        public Sprite Sprite => sprite;
        public RuntimeAnimatorController Controller => controller;

    }

}