using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugTxt;
    [SerializeField] private GameObject ScanObj;

    private bool isInSite;
    
    [Header("AR References")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    private GameObject spawnedObject;

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void Update()
    {
        // if (isInSite)
        // {
        //     MainGameManager.instance.currentAR.transform.localPosition = Vector3.zero;
        //     MainGameManager.instance.currentAR.transform.localRotation = Quaternion.identity;
        // }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
            HandleTrackedImage(trackedImage);

        foreach (var trackedImage in args.updated)
            HandleTrackedImage(trackedImage);
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        var marker = MainGameManager.instance.CurrentMarker;
        var obj = MainGameManager.instance.currentAR;

        // Debug default
        debugTxt.text = trackedImage.referenceImage.name + " - Not Tracked";

        ScanObj.SetActive(true);
        // Only respond to the marker we care about
        if (trackedImage.referenceImage.name != marker || obj == null)
        {
            debugTxt.text = trackedImage.referenceImage.name + " - Not Available";
            ScanObj.SetActive(true);
            return;
        }

        // Attach object to tracked image **once**
        if (obj.transform.parent != trackedImage.transform)
        {
            obj.transform.SetParent(trackedImage.transform, false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }

        Vector2 imageSize = trackedImage.size;
        float scale = Mathf.Min(imageSize.x, imageSize.y);
        obj.transform.localScale = (Vector3.one * scale) / 2;
        
        // Show or hide based on tracking state
        bool isTracking = trackedImage.trackingState == TrackingState.Tracking;
        isInSite = isTracking;
        obj.SetActive(isTracking);

        if (isTracking)
        {
            ScanObj.SetActive(false);
        }
        else
        {
            ScanObj.SetActive(true);
        }

        debugTxt.text = trackedImage.referenceImage.name + (isTracking ? " - Tracked" : " - Lost");
    }
}
