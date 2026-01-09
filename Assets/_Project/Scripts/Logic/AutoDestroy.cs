using System.Collections;
using UnityEngine;

namespace ARMarker
{
    public class AutoDestroy : MonoBehaviour
    {

        [SerializeField]
        private float countdown = 1f;

        [SerializeField]
        private bool autoDestroy = true;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(countdown);

            if (autoDestroy)
            {
                DestroyImmediate(gameObject);
            }
            else
            { 
                gameObject.SetActive(false);
            }
        }

    }

}