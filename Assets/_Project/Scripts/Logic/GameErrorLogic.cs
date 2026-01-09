using TMPro;
using UnityEngine;

namespace ARMarker
{

    public class GameErrorLogic : MonoBehaviour
    {

        [SerializeField]
        private GameObject rootUI;

        [SerializeField]
        private TextMeshProUGUI textError;

        private void Start()
        {
            GameManager.Instance.RegisterOnError(OnError);
            rootUI.SetActive(false);
        }

        private void OnDestroy()
        {
            GameManager.Instance.RegisterOnError(OnError, true);
        }

        private void OnError(string error)
        {
            textError.text = error;
            rootUI.SetActive(true);  
        }

    }

}