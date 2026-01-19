using System.Collections;
using ARMarker.ARMarker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class WorkLayerListItemUI : MonoBehaviour
    {

        [Header("UI Elements")]

        [SerializeField]
        private RawImage rawImagePreview;

        [SerializeField]
        private TextMeshProUGUI textName;

        [SerializeField]
        private SwipeBlocker swipeBlocker;

        [Header("UI Indicators")]

        [SerializeField]
        private GameObject indicatorVideo;

        [SerializeField]
        private GameObject indicatorGIF;

        [SerializeField]
        private GameObject indicatorSFX;

        [SerializeField]
        private string prefixLocked = "[LOCKED] ";

        [Header("Colors")]

        [SerializeField]
        private Color colorBgNormal;

        [SerializeField]
        private Color colorBgHighlighted;

        [Header("Buttons")]

        [SerializeField]
        private Button buttonBackground;

        [SerializeField]
        private Button buttonLock;

        [SerializeField]
        private Button buttonDuplicate;

        [SerializeField]
        private Button buttonDelete;

        private WorkLayer cachedLayer;

        private bool hasBeenSetUp = false;

        private void Awake()
        {
            buttonDelete.onClick.AddListener(DeleteSelf);
            buttonDuplicate.onClick.AddListener(DuplicateLayer);

            buttonLock.onClick.AddListener(LockLayer);
        }

        private void OnEnable()
        {
            if (hasBeenSetUp)
            {
                return;
            }

            ExecuteSetUp();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            if (cachedLayer != null)
            {
                cachedLayer.RegisterOnSetUpData(
                    ExecuteSetUp, OnSelectedLayer, true);
            }

            buttonDelete?.onClick.RemoveAllListeners();
            buttonDuplicate?.onClick.RemoveAllListeners();
            buttonLock?.onClick.RemoveAllListeners();
            buttonBackground?.onClick.RemoveAllListeners();
        }

        

        private void LockLayer()
        {
            if (cachedLayer == null)
                return;

            cachedLayer.ToggleLockState();

            var spriteName = cachedLayer.Data.sprite.name;
            textName.text = (cachedLayer.IsLocked)
                ? (prefixLocked + spriteName) : spriteName;

            if (cachedLayer.IsLocked)
            {
                // Reset undo/redo stacks for a "fresh start"
                UndoManager.Instance.ResetUndoRedo();

                // Optionally deselect in workspace
                WorkSpaceSingleton.Instance.ChangeActiveLayer(null);
            }
            else
            {
                SafelyForceSelectLayerOnWorkspace();
            }
        }


        private void DuplicateLayer()
        {
            WorkSpaceSingleton.Instance.DuplicateLayer(cachedLayer);
        }

        private bool isBeingDestroyed = false;

        private void DeleteSelf()
        {
            if (isBeingDestroyed)
                return;

            isBeingDestroyed = true;

            WorkSpaceSingleton.Instance.DeleteLayer(cachedLayer);
            Destroy(gameObject);
        }

        
        private void OnSelectedLayer(bool isSelected)
        {
            if (this == null || !isActiveAndEnabled)
                return;

            if (buttonBackground == null)
                return;

            if (buttonBackground.image == null)
                return;

            buttonBackground.image.color =
                isSelected ? colorBgHighlighted : colorBgNormal;
        }


        private void SafelyForceSelectLayerOnWorkspace()
        {
            if (cachedLayer.IsLocked)
            {
                return;
            }

            cachedLayer.Select();
            WorkSpaceSingleton.Instance.RetriggerCurrentEditMode();
        }

        public void SetLayer(WorkLayer layer)
        {
            cachedLayer = layer;
            cachedLayer.RegisterOnSetUpData(ExecuteSetUp, OnSelectedLayer);
            buttonBackground.onClick.AddListener(SafelyForceSelectLayerOnWorkspace);

            ExecuteSetUp();
        }

        public void SetScrollRect(ScrollRect scrollRect)
        { 
            swipeBlocker.SetScrollRect(scrollRect);
        }

        private void ExecuteSetUp()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            StartCoroutine(C_SetUpCache());
        }

        private IEnumerator C_SetUpCache()
        {
            yield return null;

            if (this == null || !isActiveAndEnabled)
                yield break;

            if (cachedLayer == null || cachedLayer.Data == null)
                yield break;

            if (rawImagePreview == null || textName == null)
                yield break;

            textName.text = cachedLayer.Data.sprite.name;
            rawImagePreview.texture = cachedLayer.Data.sprite.texture;

            indicatorGIF?.SetActive(cachedLayer.Data.animController != null);
            indicatorVideo?.SetActive(cachedLayer.Data.videoClip != null);

            bool hasNoAudio = (cachedLayer.Data.audioClip == null);
            buttonLock?.gameObject.SetActive(hasNoAudio);
            indicatorSFX?.SetActive(!hasNoAudio);

            if (!hasNoAudio)
            {
                textName.text = cachedLayer.Data.audioClip.name;
            }

            hasBeenSetUp = true;
        }


    }

}