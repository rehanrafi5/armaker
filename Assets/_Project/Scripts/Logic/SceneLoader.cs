using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARMarker
{

    public class SceneLoader : MonoBehaviour
    {

        [SerializeField]
        private Scene scene;

        [SerializeField]
        private float delay;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(delay);

            SceneManager.LoadScene((int)scene);
        }

    }

}