using System;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMarker
{
    public class ARSessionSingleton : BaseSingleton<ARSessionSingleton>
    {

        [Header("AR Settings")]

        [SerializeField]
        private GameObject prefabARBlankObject;

        [SerializeField]
        private int maxNumberOfMovingImages;

        [SerializeField]
        private float markerWidth = 0.5f;

        private ARTrackedImage cachedTrackedImage;
        private ARTrackedImageManager cachedARManager;

        private ARSession cachedSession;
        //private ARSessionOrigin cachedSessionOrigin;
        private XROrigin cachedSessionOrigin_;
        
        private GameObject cachedARObject;

        private Action<ARStatus> onStatusChange;
        private ARStatus cachedStatus;

        private int runCount = 0;

        protected override void Awake()
        {
            base.Awake();
            cachedStatus = ARStatus.UNSET;
            RegisterOnStatusChange(OnARStatusChange);
        }

        private void OnARStatusChange(ARStatus status)
        {
            cachedStatus = status;
        }

        public ARStatus GetStatus() => cachedStatus;

        public void RegisterOnStatusChange(Action<ARStatus> listener, 
            bool deRegisterInstead = false)
        {
            if (listener == null)
            {
                return;
            }


            if(deRegisterInstead)
            {
                onStatusChange -= listener;
            }
            else
            {
                onStatusChange += listener;
            }
        }

        public ARTrackedImage GetTrackedImage() => cachedTrackedImage;

        public void RegisterSessionOrigin(
            ARSession session, XROrigin sessionOrigin)
        {
            cachedSession = session;
            cachedSessionOrigin_ = sessionOrigin;
            onStatusChange?.Invoke(ARStatus.SessionOriginCreated);
        }

        public Transform InstantiateARObjectToSessionOrigin()
        {
            SafelyDeleteSpawnedARObject();

            cachedARObject = Instantiate(prefabARBlankObject,
                cachedSessionOrigin_.transform);
            return cachedARObject.transform;
        }

        public void DisableActiveTracking()
        {
            onStatusChange?.Invoke(ARStatus.UNSET);

            SafelyDeleteSpawnedARObject();

            if (cachedARManager != null)
            {
                cachedARManager.trackedImagesChanged -= OnChangedTrackedImage;
                cachedARManager.enabled = false; // ✅ DO NOT DESTROY
            }

            cachedTrackedImage = null;

            onStatusChange?.Invoke(ARStatus.UNSET);
        }


        private void SafelyDeleteSpawnedARObject()
        {
            if (cachedARObject != null)
            {
                Destroy(cachedARObject);
                cachedARObject = null;
            }
        }

        private void SetUpTracking()
        {
            runCount++;

            cachedARManager = cachedSessionOrigin_.gameObject
                .AddComponent<ARTrackedImageManager>();
            cachedARManager.trackedImagePrefab = prefabARBlankObject;
            cachedARManager.requestedMaxNumberOfMovingImages = maxNumberOfMovingImages;
            cachedARManager.trackedImagesChanged += OnChangedTrackedImage;
        }

        private IEnumerator C_StartTracking(Sprite marker)
        {
            DisableActiveTracking();
            SetUpTracking();

            onStatusChange?.Invoke(ARStatus.UNSET);

#if UNITY_EDITOR
            Debug.LogWarning($"{GetType().Name}" +
                $".StartTracking(): Skipping the creation " +
                $"of runtime XR marker library.", gameObject);

            onStatusChange?.Invoke(ARStatus.MarkerDetected);
            yield break;
#endif
            cachedARManager.enabled = true;
            Debug.LogWarning($"{GetType().Name}" +
                $".StartTracking(): ARSession.state is {ARSession.state}", gameObject);

            while (ARSession.state != ARSessionState.SessionTracking)
            {
                yield return null;
            }

            var library = cachedARManager.CreateRuntimeLibrary();
            cachedARManager.referenceLibrary = library;

            Debug.LogWarning($"{GetType().Name}" +
                $".StartTracking(): Created runtime library", gameObject);

            if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                var handle = mutableLibrary.ScheduleAddImageWithValidationJob(
                    marker.texture, marker.name, markerWidth);
                handle.jobHandle.Complete();

                yield return null;

                if (runCount > 1)
                {
                    cachedSession.Reset();
                }

                cachedARManager.enabled = true;

                if (handle.status != AddReferenceImageJobStatus.Success)
                {
                    Debug.LogError($"{GetType().Name}.StartTracking(): " +
                        $"Failed to add image! Status: {handle.status}");
                    yield break;
                }
                Debug.Log($"{GetType().Name}.StartTracking(): " +
                    $"Set Marker to: '{marker.name}'", gameObject);

                onStatusChange?.Invoke(ARStatus.ScanningMarker);
            }
            else
            {
                Debug.LogError($"{GetType().Name}.StartTracking(): " +
                    $"COULD NOT PROCESS NEW MARKER CHOICE " +
                    $"'{marker.name}'", gameObject);
            }
        }

        public void StartTracking(Sprite marker)
        {
            StopAllCoroutines();
            StartCoroutine(C_StartTracking(marker));
        }

        private void OnChangedTrackedImage(ARTrackedImagesChangedEventArgs eventArgs)
        {
            if (!isActiveAndEnabled)
                return;

            foreach (var trackedImage in eventArgs.added)
            {
                if (trackedImage == null)
                    continue;

                SafelyDeleteSpawnedARObject();
                cachedARObject = trackedImage.gameObject;

                cachedTrackedImage = trackedImage;
                onStatusChange?.Invoke(ARStatus.MarkerDetected);
            }

            foreach (var updatedImage in eventArgs.updated)
            {
                if (updatedImage == null)
                    continue;

                cachedTrackedImage = updatedImage;
                onStatusChange?.Invoke(ARStatus.ActivelyTrackingMarker);
            }

            foreach (var removedImage in eventArgs.removed)
            {
                onStatusChange?.Invoke(ARStatus.LostMarker);
            }
        }


    }

}