using UnityEngine;

public class BlankARHandler : MonoBehaviour
{
    private Transform workSpace;
    private Transform workLayer;

    private Vector3 startLocalPos;
    private Quaternion startLocalRot;

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

        if (workLayer != null)
        {
            startLocalPos = workLayer.localPosition;

            // force Z to negative
            startLocalPos.z = -Mathf.Abs(startLocalPos.z);

            startLocalRot = workLayer.localRotation;
        }
    }

    void Update()
    {
        SetTempChildrenToZero(transform);
        JUGAR();
    }

    void SetTempChildrenToZero(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains("[TEMP Layer]"))
            {
                child.localPosition = Vector3.zero;
            }

            SetTempChildrenToZero(child);
        }
    }

    void JUGAR()
    {
        if (workSpace != null)
        {
            workSpace.localPosition = Vector3.zero;
            workSpace.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        if (workLayer != null)
        {
            workLayer.localPosition = startLocalPos;
            workLayer.localRotation = startLocalRot;
        }
    }

    void OnGUI()
    {
        // #if UNITY_EDITOR
        // GUILayout.BeginVertical("box");
        //
        // GUILayout.Label("SELF");
        // PrintTransform(transform);
        //
        // if (workSpace != null)
        // {
        //     GUILayout.Space(10);
        //     GUILayout.Label("[Work Space]");
        //     PrintTransform(workSpace);
        // }
        //
        // if (workLayer != null)
        // {
        //     GUILayout.Space(10);
        //     GUILayout.Label("Work Layer");
        //     PrintTransform(workLayer);
        // }
        //
        // GUILayout.EndVertical();
        // #endif
    }

    void PrintTransform(Transform t)
    {
        Vector3 pos = t.localPosition;
        Vector3 rot = t.localEulerAngles;

        GUILayout.Label($"Pos: {pos.x:F3}, {pos.y:F3}, {pos.z:F3}");
        GUILayout.Label($"Rot: {rot.x:F3}, {rot.y:F3}, {rot.z:F3}");
    }
}