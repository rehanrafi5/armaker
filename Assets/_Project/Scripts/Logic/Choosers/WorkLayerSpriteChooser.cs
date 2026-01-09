using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class WorkLayerSpriteChooser : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootChoicesButton;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private RectTransform dropAreaRect;

        [Header("Data")]

        [SerializeField]
        private WorkLayerSpriteChoices choices;

        [SerializeField]
        private SpriteChoiceButton prefabButton;

        private void Start()
        {
            SetUpButtons();
        }

        private void SetUpButtons()
        {
            if (choices == null
                || choices.Choices.Count == 0)
            {
                Debug.LogError($"{GetType().Name} " +
                    $"Missing Choices!", gameObject);
                return;
            }

            foreach (var choice in choices.Choices)
            { 
                if(choice == null)
                {
                    continue;
                }

                var button = Instantiate(prefabButton, rootChoicesButton);
                button.SetUp(choice, scrollRect, dropAreaRect, true);
                //button.RegisterOnClick(AddLayer);
                //button.RegisterOnOutsideDrag(AddLayer);
            }
        }

        //private void AddLayer(Sprite sprite)
        //{
        //    if (sprite == null)
        //    {
        //        return;
        //    }

        //    WorkSpaceSingleton.Instance.AddLayer(sprite);
        //}

    }

}