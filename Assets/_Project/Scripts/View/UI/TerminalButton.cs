using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    [RequireComponent(typeof(Button))]
    public class TerminalButton : MonoBehaviour
    {
        [SerializeField]
        private Scene sceneTarget;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnChangeScene);
        }
        
        Transform copyParent;
        void OnChangeScene()
        {
            if (MainGameManager.instance.currentAR != null)
            {
                Destroy(MainGameManager.instance.currentAR.gameObject);
            }
            
            if (copyParent == null)
            {
                GameObject parentGO = new GameObject("WorkLayerCopies");
                copyParent = parentGO.transform;
            }
            WorkLayer[] workLayers = FindObjectsOfType<WorkLayer>();

            foreach (WorkLayer wl in workLayers)
            {
                GameObject go = wl.gameObject;
                BoxCollider box = go.GetComponent<BoxCollider>();

                if (box != null && box.enabled)
                {
                    GameObject copy = Instantiate(go, go.transform.position, go.transform.rotation, copyParent);
                    Vector3 sppp = copy.transform.position;
                    sppp.z = 0;
                    copy.transform.position = sppp;
                    Vector3 rot = copy.transform.eulerAngles;
                    rot.x += 90;
                    copy.transform.eulerAngles = rot;
                    
                    copy.name = go.name + "_Copy";
                }
            }
            foreach (Transform duplicated in copyParent.transform)
            {
                int childCount = duplicated.childCount;

                if (childCount == 0) continue;

                // Loop through all children except the last one
                for (int i = 0; i < childCount - 1; i++)
                {
                    duplicated.GetChild(i).gameObject.SetActive(false);
                }

                // Optional: ensure the last child is active
                duplicated.GetChild(childCount - 1).gameObject.SetActive(true);
            }
            copyParent.gameObject.SetActive(false);
            
            DontDestroyOnLoad(copyParent.gameObject);
            MainGameManager.instance.currentAR = copyParent.gameObject;
            
            Debug.Log("WorkLayer copies created under: " + copyParent.name);

            var g = FindObjectsOfType<AxisFollowHandler>();
            for (int i = 0; i < g.Length; i++)
            {
                g[i].gameObject.SetActive(false);
            }
            GameManager.Instance.LoadScene(sceneTarget);
        }
    }
}