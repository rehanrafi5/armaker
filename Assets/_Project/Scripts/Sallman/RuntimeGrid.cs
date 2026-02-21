using UnityEngine;

[ExecuteAlways]
public class RuntimeGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    public Material lineMaterial;
    public float lineWidth = 0.05f;
    public Color gridColor = Color.white;

    Transform gridRoot;

    void OnEnable()
    {
        CreateRoot();
        BuildGrid();
    }

    void LateUpdate()
    {
        // 🔴 Cancel out parent scale
        if (transform.lossyScale != Vector3.zero)
        {
            Vector3 s = transform.lossyScale;
            gridRoot.localScale = new Vector3(
                1f / s.x,
                1f / s.y,
                1f / s.z
            );
        }
    }

    void CreateRoot()
    {
        if (gridRoot != null) return;

        GameObject root = new GameObject("GridRoot");
        root.transform.SetParent(transform, false);
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        gridRoot = root.transform;
    }

    void BuildGrid()
    {
        ClearGrid();

        float halfW = width * cellSize * 0.5f;
        float halfH = height * cellSize * 0.5f;

        // Vertical lines
        for (int x = 0; x <= width; x++)
        {
            float xPos = -halfW + x * cellSize;
            CreateLine(
                new Vector3(xPos, -halfH, 0),
                new Vector3(xPos,  halfH, 0)
            );
        }

        // Horizontal lines
        for (int y = 0; y <= height; y++)
        {
            float yPos = -halfH + y * cellSize;
            CreateLine(
                new Vector3(-halfW, yPos, 0),
                new Vector3( halfW, yPos, 0)
            );
        }
    }

    void CreateLine(Vector3 localStart, Vector3 localEnd)
    {
        GameObject go = new GameObject("GridLine");
        go.transform.SetParent(gridRoot, false);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.startColor = gridColor;
        lr.endColor = gridColor;

        lr.SetPosition(0, localStart);
        lr.SetPosition(1, localEnd);
    }

    void ClearGrid()
    {
        if (gridRoot == null) return;

        for (int i = gridRoot.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridRoot.GetChild(i).gameObject);
        }
    }
}
