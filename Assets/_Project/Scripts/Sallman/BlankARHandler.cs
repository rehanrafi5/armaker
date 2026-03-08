using UnityEngine;

public class BlankARHandler : MonoBehaviour
{
    void Update()
    {
        SetTempChildrenToZero(transform);
    }

    void SetTempChildrenToZero(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains("[TEMP Layer]"))
            {
                child.localPosition = Vector3.zero;
            }

            // Continue searching deeper
            SetTempChildrenToZero(child);
        }
    }
    
    private Transform workSpace;
    private Transform workLayer;
    
    void Start()
    {
        Transform[] all = GetComponentsInChildren<Transform>(true);

        foreach (var t in all)
        {
            if (t.name.Contains("[Work Space]"))
                workSpace = t;

            if (t.name.Contains("Work Layer"))
                workLayer = t;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box");

        GUILayout.Label("SELF");
        PrintTransform(transform);

        if (workSpace != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("[Work Space]");
            PrintTransform(workSpace);
        }

        if (workLayer != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Work Layer");
            PrintTransform(workLayer);
        }

        GUILayout.EndVertical();
    }

    void PrintTransform(Transform t)
    {
        Vector3 pos = t.position;
        Vector3 rot = t.eulerAngles;

        GUILayout.Label($"Pos: {pos.x:F3}, {pos.y:F3}, {pos.z:F3}");
        GUILayout.Label($"Rot: {rot.x:F3}, {rot.y:F3}, {rot.z:F3}");
    }
}
