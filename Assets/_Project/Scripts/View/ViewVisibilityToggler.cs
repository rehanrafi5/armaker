using UnityEngine;

namespace ARMarker
{

    public class ViewVisibilityToggler : MonoBehaviour
    {

        public void ToggleVisibility()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);        
        }

    }

}