using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    [RequireComponent(typeof(Button))]
    public class TerminalButton : MonoBehaviour
    {
        [SerializeField]
        private Scene sceneTarget;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(
                () => GameManager.Instance.LoadScene(sceneTarget));
        }
    }
}