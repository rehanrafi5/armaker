using UnityEngine;

namespace ARMarker
{

    public class TransformRotationPreserver : MonoBehaviour
    {

        private Quaternion initialWorldRotation;

        private void Awake()
        {
            initialWorldRotation = transform.rotation;
        }

        private void LateUpdate()
        {
            transform.rotation = initialWorldRotation;   
        }

    }

}