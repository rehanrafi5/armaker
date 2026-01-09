using System.Collections.Generic;
using UnityEngine;

namespace ARMarker
{

    public class ARStatusTracker : MonoBehaviour
    {

        [SerializeField]
        private bool shouldEnable;

        [SerializeField]
        private List<ARStatus> trackedStatuses = new();

        private void Start()
        {
            ARSessionSingleton.Instance
                .RegisterOnStatusChange(OnARStatusChange);
            OnARStatusChange(ARSessionSingleton.Instance.GetStatus());
        }

        private void OnDestroy()
        {
            ARSessionSingleton.Instance
                .RegisterOnStatusChange(OnARStatusChange, true);
        }

        private void OnARStatusChange(ARStatus status)
        {
            if (trackedStatuses.Contains(status))
            {
                gameObject.SetActive(shouldEnable);
            }
            else
            {
                gameObject.SetActive(!shouldEnable);
            }
        }

    }

}