using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMarker
{

    [RequireComponent(typeof(BoxCollider))]
    public class LayerRepositionZHandler : MonoBehaviour, 
        IBeginDragHandler, IDragHandler
    {

        [SerializeField] 
        private Transform influencedObject;

        [SerializeField] 
        [Tooltip("How much Z changes per pixel dragged on X")]
        private float sensitivity = 0.01f;
        
        [SerializeField] 
        private float minZ = -10f;
        
        [SerializeField] 
        private float maxZ = 10f;

        private Vector2 previousPointerPos;

        public void OnBeginDrag(PointerEventData eventData)
        {
            previousPointerPos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (influencedObject == null)
                return;

            Vector2 currentPointerPos = eventData.position;
            float deltaX = currentPointerPos.x - previousPointerPos.x;
            previousPointerPos = currentPointerPos;

            float deltaZ = deltaX * sensitivity;
            float newZ = Mathf.Clamp(influencedObject.position.z + deltaZ, minZ, maxZ);

            influencedObject.position = new Vector3(
                influencedObject.position.x,
                influencedObject.position.y,
                newZ
            );
        }

    }

}