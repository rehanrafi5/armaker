using ARMarker;
using UnityEngine;

public class AxisDragHandler : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis moveAxis;

    [SerializeField] private Transform target;

    private Camera cam;

    private int activeFingerId = -1;
    private bool dragging;
    
    private Vector2 lastScreenPos;
    private bool firstDragFrame;
    
    LayerRepositionXYHandler layerRepositionXY;
    WorkLayer workLayer;
    
    void Awake()
    {
        cam = Camera.main;

        if (target == null)
            target = transform.parent;

        layerRepositionXY = target.GetComponent<LayerRepositionXYHandler>();
        workLayer = target.GetComponent<WorkLayer>();
    }
    void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;   
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    // ---------------- MOUSE ----------------
    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            TryBeginDrag(Input.mousePosition, -1);

        if (dragging && Input.GetMouseButton(0))
            ContinueDrag(Input.mousePosition);

        if (dragging && Input.GetMouseButtonUp(0))
            EndDrag();
    }

    // ---------------- TOUCH ----------------
    void HandleTouch()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
                TryBeginDrag(touch.position, touch.fingerId);

            if (dragging && touch.fingerId == activeFingerId &&
                (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
                ContinueDrag(touch.position);

            if (dragging && touch.fingerId == activeFingerId &&
                (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
                EndDrag();
        }
    }

    // ---------------- CORE ----------------
    void TryBeginDrag(Vector2 screenPos, int fingerId)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        int mask = LayerMask.GetMask("MovingTools");
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
            return;

        if (hit.transform != transform) return;

        dragging = true;
        activeFingerId = fingerId;

        lastScreenPos = screenPos;
        firstDragFrame = true;   // 🔑 IMPORTANT

        MainGameManager.instance.FreeMovement = false;

        if (moveAxis == Axis.X || moveAxis == Axis.Y)
            layerRepositionXY.OnDragStart();

        workLayer.OnDragStart();
    }

    void ContinueDrag(Vector2 screenPos)
    {
        if (firstDragFrame)
        {
            // Swallow the first frame to prevent jump
            lastScreenPos = screenPos;
            firstDragFrame = false;
            return;
        }

        Vector2 screenDelta = screenPos - lastScreenPos;

        Vector3 axisDir = GetAxisDirection();

        Vector3 axisScreenDir =
            cam.WorldToScreenPoint(target.position + axisDir) -
            cam.WorldToScreenPoint(target.position);

        axisScreenDir.z = 0f;

        if (axisScreenDir.sqrMagnitude < 0.0001f)
            return;

        axisScreenDir.Normalize();

        float moveAmount = Vector2.Dot(screenDelta, (Vector2)axisScreenDir);

        float worldPerPixel = 0.01f;
        target.position += axisDir * moveAmount * worldPerPixel;

        lastScreenPos = screenPos;

        if (moveAxis == Axis.X || moveAxis == Axis.Y)
            layerRepositionXY.OnDragging();
        
        workLayer.OnDragging();
    }

    void EndDrag()
    {
        dragging = false;
        activeFingerId = -1;

        MainGameManager.instance.FreeMovement = true;

        if (moveAxis == Axis.X || moveAxis == Axis.Y)
            layerRepositionXY.OnDragEnd();
        
        workLayer.OnDragEnd();
    }

    Vector3 GetAxisDirection()
    {
        return moveAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };
    }
}
