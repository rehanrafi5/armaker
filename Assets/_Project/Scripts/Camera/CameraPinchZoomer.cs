using UnityEngine;

namespace ARMarker
{

    public class CameraPinchZoomer : MonoBehaviour
    {
        [Tooltip("How sensitive the zoom is to pinch gestures.")]
        [SerializeField]
        private float zoomSensitivity = 0.1f;

        [Tooltip("Minimum orthographic size allowed.")]
        [SerializeField]
        private float minZoom = 1f;

        [Tooltip("Maximum orthographic size allowed.")]
        [SerializeField]
        private float maxZoom = 10f;

        [SerializeField]
        private new Camera camera;

        private void Update()
        {
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                // Find the position difference between the touches in this frame and the last frame.
                Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
                Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

                float prevMagnitude = (prevTouch0 - prevTouch1).magnitude;
                float currentMagnitude = (touch0.position - touch1.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                // Apply the zoom based on the difference and sensitivity
                camera.orthographicSize -= difference * zoomSensitivity;
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
            }
        }
    }

}