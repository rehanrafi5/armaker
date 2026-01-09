using System.Collections;
using UnityEngine;

namespace ARMarker
{

    public class CameraPerspectiveSwitcher : MonoBehaviour
    {

        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private float switchDuration = 1f;

        [Header("Settings for 2D")]

        [SerializeField]
        private Vector3 position2D;

        [SerializeField]
        private Quaternion rotation2D;

        [SerializeField]
        private GameObject[] activeObjects2D;

        [Header("Settings for 3D")]

        [SerializeField]
        private Vector3 position3D;

        [SerializeField]
        private Quaternion rotation3D;

        [SerializeField]
        private GameObject[] activeObjects3D;

        public void SwitchPerspective(bool is2D)
        {
            //camera.orthographic = is2D;

            foreach (var obj in activeObjects2D)
            {
                if (obj == null)
                {
                    continue;
                }

                obj.SetActive(is2D);
            }

            foreach (var obj in activeObjects3D)
            {
                if (obj == null)
                {
                    continue;
                }

                obj.SetActive(!is2D);
            }

            StopAllCoroutines();
            StartCoroutine(LerpTransform(is2D));
        }

        private IEnumerator LerpTransform(bool is2D)
        {
            var finalPos = is2D ? position2D : position3D;
            var finalRot = is2D ? rotation2D : rotation3D;

            var startPos = transform.position;
            var startRot = transform.rotation;
            var elapsed = 0f;

            while (elapsed < switchDuration)
            {
                elapsed += Time.deltaTime;
                var percentage = Mathf.Clamp01(
                    elapsed / switchDuration);

                transform.position = Vector3.Lerp(
                    startPos, finalPos, percentage);
                transform.rotation = Quaternion.Slerp(
                    startRot, finalRot, percentage);

                yield return null;
            }

            transform.position = finalPos;
            transform.rotation = finalRot;
        }

    }

}