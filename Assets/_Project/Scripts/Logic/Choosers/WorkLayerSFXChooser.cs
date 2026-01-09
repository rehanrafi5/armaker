using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class WorkLayerSFXChooser : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private Transform rootChoicesButton;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private RectTransform dropAreaRect;

        [Header("Audio Settings")]

        [SerializeField]
        private AudioSource audioSourcePreview;

        [SerializeField]
        private float previewLengthInSeconds = 5f;

        [Header("Data")]

        [SerializeField]
        private WorkLayerSFXChoices choices;

        [SerializeField]
        private SFXChoiceButton prefabButton;

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
                if (choice == null)
                {
                    continue;
                }

                if (choice.Sprite == null)
                {
                    choice.SetSprite(choices.DefaultSprite);
                }

                var button = Instantiate(prefabButton, rootChoicesButton);
                button.SetUp(choice, scrollRect, dropAreaRect, true);
                button.RegisterOnClick(OnClickChoice);
            }
        }

        private void OnClickChoice(SFXLayerData data)
        {
            if (data == null)
            {
                return;
            }

            StopAllCoroutines();
            StartCoroutine(C_PreviewSound(data));
        }

        private IEnumerator C_PreviewSound(SFXLayerData data)
        {
            audioSourcePreview.Stop();
            audioSourcePreview.clip = data.Clip;
            audioSourcePreview.volume = 1f;
            audioSourcePreview.Play();

            yield return new WaitForSeconds(previewLengthInSeconds - 1f);

            var timeElapsed = 0f;
            while (timeElapsed < 1f)
            {
                audioSourcePreview.volume = Mathf.Lerp(1f, 0f, timeElapsed / 1f);
                yield return null;
                timeElapsed += Time.deltaTime;
            }

            audioSourcePreview.Stop();
        }

    }

}