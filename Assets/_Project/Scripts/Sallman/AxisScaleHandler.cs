using ARMarker;
using UnityEngine;

public class AxisScaleHandler : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis scaleAxis;

    [SerializeField] private Transform target;

    private Camera cam;
    private bool dragging;
    private Vector2 lastScreenPos;
    private bool firstDragFrame;
    
    LayerResizeXYHandler layerResizeHandler;

    void Awake()
    {
        cam = Camera.main;
        if (target == null)
            target = transform.parent;
        
        layerResizeHandler =  target.GetComponent<LayerResizeXYHandler>();
    }
    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            TryBeginDrag(Input.mousePosition);

        if (dragging && Input.GetMouseButton(0))
            ContinueDrag(Input.mousePosition);

        if (dragging && Input.GetMouseButtonUp(0))
            EndDrag();
    }

    void HandleTouch()
    {
        foreach (var t in Input.touches)
        {
            if (t.phase == TouchPhase.Began)
                TryBeginDrag(t.position);

            if (dragging && (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary))
                ContinueDrag(t.position);

            if (dragging && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
                EndDrag();
        }
    }

    void TryBeginDrag(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        int mask = LayerMask.GetMask("MovingTools");

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask)) return;
        if (hit.transform != transform) return;

        dragging = true;
        lastScreenPos = screenPos;
        firstDragFrame = true;
        
        MainGameManager.instance.FreeMovement = false;
        
        if (scaleAxis == Axis.X || scaleAxis == Axis.Y)
            layerResizeHandler.OnDragStart();
    }

    void ContinueDrag(Vector2 screenPos)
    {
        if (firstDragFrame)
        {
            lastScreenPos = screenPos;
            firstDragFrame = false;
            return;
        }

        Vector2 delta = screenPos - lastScreenPos;

        Vector3 axisDir = GetAxisDirection();
        Vector3 axisScreenDir =
            cam.WorldToScreenPoint(target.position + axisDir) -
            cam.WorldToScreenPoint(target.position);

        axisScreenDir.z = 0f;
        axisScreenDir.Normalize();

        float scaleDelta = Vector2.Dot(delta, (Vector2)axisScreenDir) * 0.01f;

        Vector3 scale = target.localScale;
        scale += axisDir * scaleDelta;
        scale = Vector3.Max(scale, Vector3.one * 0.01f);

        target.localScale = scale;

        lastScreenPos = screenPos;
        
        if (scaleAxis == Axis.X || scaleAxis == Axis.Y)
            layerResizeHandler.OnDragging();
    }

    void EndDrag()
    {
        dragging = false;
        
        MainGameManager.instance.FreeMovement = true;
        
        if (scaleAxis == Axis.X || scaleAxis == Axis.Y)
            layerResizeHandler.OnDragging();
    }

    Vector3 GetAxisDirection()
    {
        return scaleAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };
    }
}
