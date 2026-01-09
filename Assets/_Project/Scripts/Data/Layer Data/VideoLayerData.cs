using UnityEngine;
using UnityEngine.Video;

namespace ARMarker
{

    [System.Serializable]
    public class VideoLayerData
    {

        [SerializeField]
        private Sprite sprite;

        [SerializeField]
        private VideoClip clip;

        public Sprite Sprite => sprite;
        public VideoClip Clip => clip;

    }

}