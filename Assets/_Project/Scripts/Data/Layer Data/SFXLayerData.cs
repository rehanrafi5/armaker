using UnityEngine;

namespace ARMarker
{

    [System.Serializable]
    public class SFXLayerData
    {

        [SerializeField]
        private Sprite sprite;

        [SerializeField]
        private AudioClip clip;

        public Sprite Sprite => sprite;
        public AudioClip Clip => clip;

        public void SetSprite(Sprite sprite)
        { 
            this.sprite = sprite;
        }

    }

}