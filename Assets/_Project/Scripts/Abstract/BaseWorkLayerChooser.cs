using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class BaseWorkLayerChooser<T, U, V> 
        : MonoBehaviour
        where T : class
        where U : BaseChoiceButton<T>
        where V : BaseWorkLayerChoices<T>
    {

        [Header("UI Elements")]

        [SerializeField]
        protected Transform rootChoicesButton;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private RectTransform dropAreaRect;

        [Header("Data")]

        [SerializeField]
        protected V choices;

        [SerializeField]
        protected U prefabButton;

        protected virtual void Start()
        {
            SetUpButtons();
        }

        protected void SetUpButtons()
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
                if (choice == null)
                {
                    continue;
                }

                var button = Instantiate(prefabButton, rootChoicesButton);
                button.SetUp(choice, scrollRect, dropAreaRect, false);
                //button.RegisterOnClick(OnClickChoice);
            }
        }

        //protected abstract void OnClickChoice(T data);

        //protected abstract void RegisterButton<BaseChoiceButton<T>>(BaseChoiceButton<T> button);

    }

}