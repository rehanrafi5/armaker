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
            if (MainGameManager.instance.currentAR != null)
            {
                Destroy(MainGameManager.instance.currentAR.gameObject);
            }
            yield return new WaitForSeconds(delay);

            SceneManager.LoadScene((int)scene);
        }

    }

}