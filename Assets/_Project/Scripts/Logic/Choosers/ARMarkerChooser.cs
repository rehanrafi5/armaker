using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

namespace ARMarker
{ 

    public class ARMarkerChooser : MonoBehaviour
    {

        [Header("Data")]

        [SerializeField] private XRReferenceImageLibrary TheLibrary;
        
        [SerializeField]
        private ARMarkerChoices choices;

        [SerializeField]
        private MarkerChoiceButton prefabButton;

        [Header("UI Elements")]

        [SerializeField]
        private RawImage[] rawImagePreviewMarkers;

        [SerializeField]
        private Transform rootUI;

        [SerializeField]
        private Transform markerButtonsParent;

        [Space]

        [SerializeField]
        private Button buttonSelect;

        private MarkerChoiceButton cachedSelectedButton;
        private readonly List<MarkerChoiceButton> cachedSpawnedButtons = new();
        private Action<Sprite> onChooseMarker;

        private void Start()
        {
            GameManager.Instance.RegisterMarkerChooser(this);
            buttonSelect.onClick.AddListener(OnSelectMarker);

            SetUp();
            buttonSelect.interactable = false;
        }

        private void SetUp()
        {
            if (TheLibrary.count == 0)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): " +
                    $"Choices are missing!", gameObject);
                return;
            }

            for (int i = 0; i < TheLibrary.count; i++)
            {
                XRReferenceImage refImage = TheLibrary[i];

                for (int j = 0; j < choices.MarkerData.Length; j++)
                {
                    if (TheLibrary[i].name == choices.MarkerData[j].imageName)
                    {
                        var clone = Instantiate(prefabButton, markerButtonsParent);
                        clone.SetUp(choices.MarkerData[j].previewSprite, OnClickChoice);
                        cachedSpawnedButtons.Add(clone);
                    }
                }
            }
            // foreach (var choice in TheLibrary)
            // {
            //     if (choice == null)
            //     {
            //         continue;
            //     }
            //
            //     XRReferenceImage refImage = TheLibrary[i];
            //     
            //     var clone = Instantiate(prefabButton, markerButtonsParent);
            //     clone.SetUp(TextureToSprite(choice.texture), OnClickChoice);
            //     cachedSpawnedButtons.Add(clone);
            // }

            rootUI.gameObject.SetActive(false);
            SetCachedMarker();
        }

        private void OnSelectMarker()
        {
            rootUI.gameObject.SetActive(false);
            onChooseMarker?.Invoke(
                cachedSelectedButton.GetMarker());
            MainGameManager.instance.CurrentMarker = cachedSelectedButton.GetMarker().name;
            SetCachedMarker();
        }

        private void SetCachedMarker()
        {
            if (GameManager.Instance.GetMarker() == null)
            {
                return;
            }

            var marker = GameManager.Instance.GetMarker().texture;

            foreach (var rawImage in rawImagePreviewMarkers)
            {
                if (rawImage == null)
                {
                    continue;
                }

                rawImage.texture = marker;
            }
        }

        public void RegisterOnChooseMarker(Action<Sprite> listener)
        {
            if (listener == null)
            {
                return;
            }

            onChooseMarker += listener;
        }

        private void SetUpImageButtonsStatus(MarkerChoiceButton button)
        {
            foreach (var buttonSpawned in cachedSpawnedButtons)
            {
                buttonSpawned.SetIsSelected(false);
            }

            button.SetIsSelected(true);
            cachedSelectedButton = button;
        }

        private void OnClickChoice(MarkerChoiceButton button)
        {
            SetUpImageButtonsStatus(button);
            buttonSelect.interactable = true;
        }

        public void ShowChooserUI() => rootUI.gameObject.SetActive(true);

    }

}