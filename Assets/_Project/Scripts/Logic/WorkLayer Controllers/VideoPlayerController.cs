using UnityEngine;
using UnityEngine.Video;

namespace ARMarker
{

    public class VideoPlayerController : MonoBehaviour
    {

        [SerializeField]
        private VideoPlayer videoPlayer;

        [SerializeField]
        private float maxWidthHeight = 1f;

        [SerializeField]
        private Transform selectionBox;

        public void SetUp(VideoClip clip)
        {
            if (clip == null)
            {
                return;
            }

            videoPlayer.clip = clip;
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();            
        }

        private void OnVideoPrepared(VideoPlayer vp)
        {
            // Get video resolution
            float videoWidth = vp.width;
            float videoHeight = vp.height;

            float aspectRatio = videoWidth / videoHeight;

            if (videoWidth < videoHeight)
            {
                transform.localScale = new Vector3(
                    maxWidthHeight * aspectRatio, maxWidthHeight, 1f);
            }
            else
            {
                transform.localScale = new Vector3(
                    maxWidthHeight, maxWidthHeight / aspectRatio, 1f);
            }

            var zScale = selectionBox.localScale.z;
            var newScale = transform.localScale + (transform.localScale * 0.1f);
            selectionBox.localScale = new Vector3(newScale.x, newScale.y, zScale);

            Play();
        }

        [ContextMenu("Play")]
        public void Play()
        {
            videoPlayer.Play();
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            videoPlayer.Stop();
        }

    }

}