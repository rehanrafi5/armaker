using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{ 
    public class LayerEditOptionButton : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private Button button;

        [Header("Settings")]

        [SerializeField]
        private LayerEditMode mode;

        [SerializeField]
        private Color colorActive;

        [SerializeField]
        private Color colorNormal;

        [SerializeField]
        private bool isDefaultMode;

        private void Start()
        {
            button.onClick.AddListener(() => 
                WorkSpaceSingleton.Instance.SetLayerEditMode(mode));

            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayerEditMode(OnChangeLayerEditMode);

            if (isDefaultMode)
            {
                Debug.Log($"{GetType().Name} is default mode called", gameObject);
                WorkSpaceSingleton.Instance.SetLayerEditMode(mode);
            }
        }

        private void OnChangeLayerCount(int count)
        { 
            button.interactable = count > 0;
        }

        private void OnDestroy()
        {
            if (WorkSpaceSingleton.Instance == null)
            {
                return;
            }

            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayerEditMode(OnChangeLayerEditMode, true);
        }

        private void OnChangeLayerEditMode(LayerEditMode mode)
        { 
            button.image.color = this.mode == mode ? colorActive : colorNormal;
        }

    }

}