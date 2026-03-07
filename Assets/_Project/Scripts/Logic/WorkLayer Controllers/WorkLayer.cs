using System;
using System.Collections;
using ARMarker.ARMarker;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace ARMarker
{

    [RequireComponent(typeof(BoxCollider))]
    public class WorkLayer : MonoBehaviour,
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {

        [Header("Elements")]

        [SerializeField]
        private GameObject statusLocked;

        [SerializeField]
        private GameObject statusSelected;

        [Header("Settings")]

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private VideoPlayerController videoController;

        [SerializeField]
        private AudioSource audioSource;

        [Header("Settings Values")]

        [SerializeField]
        private float blinkDuration;

        [Header("Layer Transform Handlers")]

        [SerializeField]
        private LayerRepositionXYHandler repositionXYHandler;

        [SerializeField]
        private LayerRotationHandler rotationHandler;

        [SerializeField]
        private LayerResizeXYHandler resizeHandler;

        public WorkLayerData Data => cachedData;
        public bool IsLocked => isLocked;

        private WorkLayerData cachedData;
        private BoxCollider boxCollider;
        private Camera mainCamera;
        private Action onSetUpData;
        private Action<bool> onSelected;

        private bool isLocked;
        private bool isInitialDrag;

        public SpriteRenderer ImageOnBack;
        
        public bool hasBeenPlaced = false;

        [SerializeField] private GameObject[] Axis;

        private void Awake()
        {
            mainCamera = Camera.main;

            boxCollider = GetComponent<BoxCollider>();
            statusLocked.SetActive(false);
        }

        private void Start()
        {
            repositionXYHandler.RegisterListener(OnSelectLayer);
            rotationHandler.RegisterListener(OnSelectLayer);
            resizeHandler.RegisterListener(OnSelectLayer);

            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayerEditMode(OnChangeLayerEditMode);
            OnChangeLayerEditMode(WorkSpaceSingleton.Instance.GetLayerEditMode());

            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayer(OnChangeActiveLayer);

            for (int i = 0; i < Axis.Length; i++)
            {
                Axis[i].transform.parent = null;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(BlinkSpriteRenderer());   
        }

        private void OnDestroy()
        {
            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayerEditMode(OnChangeLayerEditMode, true);
            WorkSpaceSingleton.Instance
                .RegisterOnChangeLayer(OnChangeActiveLayer, true);
        }

        private void OnChangeActiveLayer(WorkLayer activeLayer)
        {
            if (this == activeLayer)
            {
                return;
            }

            Deselect();
        }

        private void OnChangeLayerEditMode(LayerEditMode mode)
        {
            var isActive = WorkSpaceSingleton.Instance.IsLayerActive(this);
            
            switch (mode)
            {
                case LayerEditMode.Reposition:
                    {
                        repositionXYHandler.enabled = true;
                        if (isActive)
                        { 
                            repositionXYHandler.Select();
                        }

                        rotationHandler.Deselect();
                        resizeHandler.Deselect();

                        rotationHandler.enabled = false;
                        resizeHandler.enabled = false;
                        break;
                    }
                case LayerEditMode.Rotate:
                    {
                        rotationHandler.enabled = true;
                        if (isActive)
                        {
                            rotationHandler.Select();
                        }

                        repositionXYHandler.Deselect();
                        resizeHandler.Deselect();

                        repositionXYHandler.enabled = false;
                        resizeHandler.enabled = false;
                        break;
                    }
                case LayerEditMode.Resize:
                    {
                        resizeHandler.enabled = true;
                        if (isActive)
                        {
                            resizeHandler.Select();
                        }

                        repositionXYHandler.Deselect();
                        rotationHandler.Deselect();

                        repositionXYHandler.enabled = false;
                        rotationHandler.enabled = false;
                        break;
                    }
            }
        }

        private void OnMouseDown()
        {
            if (statusSelected.activeInHierarchy)
            {
                return;
            }
            OnSelectLayer();
            
            WorkSpaceSingleton.Instance.SetLayerEditMode(LayerEditMode.Reposition);
        }

        private void OnSelectLayer()
        {
            statusSelected.SetActive(true);
            onSelected?.Invoke(true);
            WorkSpaceSingleton.Instance.ChangeActiveLayer(this);
        }

        public void SetUp(WorkLayerData data)
        {
            if (data == null)
            {
                Debug.LogWarning($"{GetType().Name}.SetUp(): " +
                    $"data is null!", gameObject);
                return;
            }

            cachedData = data;

            gameObject.transform.localPosition = data.position;
            gameObject.transform.rotation = data.rotation;
            gameObject.transform.localScale = data.scale;

            boxCollider.enabled = !data.isTemporary;

            SetUpSprite();
            onSetUpData?.Invoke();
        }

        public void SetUpAnimator(RuntimeAnimatorController controller)
        {
            if (controller == null)
            {
                Debug.LogWarning($"{GetType().Name}.SetUp(): " +
                    $"controller is null!", gameObject);
                return;
            }

            cachedData.animController = controller;
            animator.runtimeAnimatorController = controller;
            onSetUpData?.Invoke();
        }

        public void SetUpSFX(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning($"{GetType().Name}.SetUp(): " +
                    $"clip is null!", gameObject);
                return;
            }

            cachedData.audioClip = clip;

            animator.enabled = false;
            spriteRenderer.enabled = true;
            SetToHalfTransparency();
            transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            videoController.gameObject.SetActive(false);
            boxCollider.enabled = false;

            audioSource.enabled = true;
            audioSource.clip = clip;
            audioSource.Play();
            onSetUpData?.Invoke();

            StartCoroutine(BlinkSpriteRenderer());
        }

        private IEnumerator BlinkSpriteRenderer()
        {
            if (cachedData == null || cachedData.audioClip == null)
            {
                yield break;
            }

            if (GameManager.Instance.IsInARWorld())
            {
                spriteRenderer.enabled = false;
                yield break;
            }

            spriteRenderer.enabled = true;

            while (true)
            {
                yield return StartCoroutine(FadeAlpha(0f, 1f, blinkDuration));
                yield return StartCoroutine(FadeAlpha(1f, 0f, blinkDuration));
            }
        }

        private IEnumerator FadeAlpha(float from, float to, float time)
        {
            float elapsed = 0f;
            Color color = spriteRenderer.color;

            while (elapsed < time)
            {
                float t = elapsed / time;
                color.a = Mathf.Lerp(from, to, t);
                spriteRenderer.color = color;
                elapsed += Time.deltaTime;
                yield return null;
            }

            color.a = to;
            spriteRenderer.color = color;
        }

        public void SetToHalfTransparency()
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }

        public void SetUpVideoController(VideoClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning($"{GetType().Name}.SetUp(): " 
                    + $"clip is null!", gameObject);
                return;
            }

            cachedData.videoClip = clip;
            animator.enabled = false;
            spriteRenderer.enabled = false;

            videoController.SetUp(clip);
            videoController.gameObject.SetActive(true);
            onSetUpData?.Invoke();
        }

        public void RegisterOnSetUpData(
            Action listener, Action<bool> listenerOnSelected,
            bool deregisterInstead = false)
        {
            if (listener == null || listenerOnSelected == null)
            {
                return;
            }

            if (deregisterInstead)
            {
                onSetUpData -= listener;
                onSelected -= listenerOnSelected;
            }
            else
            {
                onSetUpData += listener;
                onSelected += listenerOnSelected;
            }
        }

        public void ToggleLockState()
        {
            if (cachedData.audioClip != null)
            {
                return;
            }

            isLocked = !isLocked;
            boxCollider.enabled = !isLocked;
            statusLocked.SetActive(isLocked);
        }

        public void SetEnabledIfTemporary(bool isEnabled)
        {
            if (cachedData == null || !cachedData.isTemporary)
            {
                return;
            }

            gameObject.SetActive(isEnabled);
        }

        public void Select()
        {
            OnSelectLayer();   
        }

        public void Deselect()
        {
            statusSelected.SetActive(false);

            repositionXYHandler.Deselect();
            rotationHandler.Deselect();
            resizeHandler.Deselect();

            onSelected?.Invoke(false);
        }

        private void SetUpSprite()
        {
            if (cachedData == null || cachedData.sprite == null)
            {
                Destroy(spriteRenderer);
                return;
            }

            Vector2 targetSize = new Vector2(
                ConstantInts.AR_OBJECT_SIZE_DIVISOR,
                ConstantInts.AR_OBJECT_SIZE_DIVISOR
            );

            // FRONT
            spriteRenderer.sprite = cachedData.sprite;
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            spriteRenderer.size = targetSize;

            // BACK (THIS IS WHAT YOU WERE MISSING)
            ImageOnBack.sprite = cachedData.sprite;
            ImageOnBack.drawMode = SpriteDrawMode.Sliced;
            ImageOnBack.size = targetSize;
            ImageOnBack.transform.localScale = Vector3.one;
        }
        public void OnDragStart()
        {
            //Ignore undo for locked, temporary, or unplaced layers
            if (isLocked || cachedData.isTemporary || !cachedData.hasBeenPlaced) 
                return;
            
            MainGameManager.instance.FreeMovement = false;
            
            UndoManager.Instance.CaptureBeforeModify(this);
        }
        public void OnDragging()
        {
            
        }
        public void OnDragEnd()
        {
            if (isLocked || cachedData.isTemporary) return;
            
            if (!cachedData.hasBeenPlaced)
            {
                // First time drop → mark as placed, do NOT capture undo
                cachedData.hasBeenPlaced = true;
                return;
            }
            MainGameManager.instance.FreeMovement = true;
            // Only capture undo if the layer has been placed before
            UndoManager.Instance.CaptureAfterModify(this);
            
            isInitialDrag = false;
        }
        public void SetupInitialDrag(bool allowInitialDrag)
        {
            isInitialDrag = allowInitialDrag;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isLocked || cachedData.isTemporary) return;
            
            if (!cachedData.hasBeenPlaced)
            {
                // First time drop → mark as placed, do NOT capture undo
                cachedData.hasBeenPlaced = true;
                return;
            }
            MainGameManager.instance.FreeMovement = true;
            // Only capture undo if the layer has been placed before
            UndoManager.Instance.CaptureAfterModify(this);
            
            isInitialDrag = false;
        }



        public void OnDrag(PointerEventData eventData)
        {
            if (cachedData.hasBeenPlaced)
                return;
            
            if (isInitialDrag)
            {
                var newPos = ScreenToWorld(eventData.position, cachedData.position.z);
                newPos.z = cachedData.position.z;
                transform.localPosition = newPos;
                
                MainGameManager.instance.FreeMovement = false;
            }
        }


        private Vector3 ScreenToWorld(Vector3 screenPos, float z)
        {
            screenPos.z = z - mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(screenPos);
        }
    }

}