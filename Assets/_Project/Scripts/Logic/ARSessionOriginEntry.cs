using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMarker
{

    public class ARSessionOriginEntry : MonoBehaviour
    {

        [SerializeField]
        private ARSession aRSession;

        [SerializeField] private XROrigin xrOrigin;
        
        [SerializeField]
        private ARSessionOrigin aRSessionOrigin;

        private void Awake()
        {
            ARSessionSingleton.Instance.RegisterSessionOrigin(
                aRSession, xrOrigin);
        }

    }

}