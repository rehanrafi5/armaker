using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugTxt;
    [SerializeField] private GameObject ScanObj;
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    private GameObject arObject;
    private ARAnchor anchor;
    private bool placed;

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
            HandleImage(trackedImage);

        foreach (var trackedImage in args.updated)
            HandleImage(trackedImage);
    }

    private void Update()
    {
        if (MainGameManager.instance.currentAR == null)
        {
            var obj = FindFirstObjectByType<ABC>(FindObjectsInactive.Include);

            if (obj != null)
                MainGameManager.instance.currentAR = obj.gameObject;
        }
    }

    private void HandleImage(ARTrackedImage trackedImage)
    {
        var manager = MainGameManager.instance;

        if (manager.currentAR == null)
        {
            debugTxt.text = "currentAR NULL";
            
            return;
        }

        if (trackedImage.referenceImage.name != manager.CurrentMarker)
        {
            debugTxt.text = "Marker mismatch: " + trackedImage.referenceImage.name;
            return;
        }

        if (!placed && trackedImage.trackingState == TrackingState.Tracking)
        {
            arObject = manager.currentAR;

            // Create anchor on tracked image
            anchor = trackedImage.gameObject.AddComponent<ARAnchor>();

            arObject.transform.SetParent(anchor.transform);
            arObject.transform.localPosition = Vector3.zero;
            arObject.transform.localRotation = Quaternion.identity;

            Vector2 size = trackedImage.size;
            float scale = Mathf.Min(size.x, size.y);
            arObject.transform.localScale = (Vector3.one * scale) / 2;

            arObject.SetActive(true);
            ScanObj.SetActive(false);

            placed = true;

            debugTxt.text = "Anchor placed";
        }

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            ScanObj.SetActive(false);
            debugTxt.text = trackedImage.referenceImage.name + " Tracking";
        }
        else
        {
            debugTxt.text = trackedImage.referenceImage.name + " Limited";
        }
    }
}