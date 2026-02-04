using ARMarker;
using UnityEngine;

public class AxisRotateHandler : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis rotateAxis;

    [SerializeField] private Transform target;

    private Camera cam;
    private bool dragging;
    private Vector2 lastScreenPos;
    private bool firstDragFrame;
    
    LayerRotationHandler layerRotationHandler;

    void Awake()
    {
        cam = Camera.main;
        if (target == null)
            target = transform.parent;
        
        layerRotationHandler = target.GetComponent<LayerRotationHandler>();
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
            TryBeginDrag(Input.mousePosition, -1);

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
                TryBeginDrag(t.position, t.fingerId);

            if (dragging && t.fingerId == -1 || dragging && t.fingerId == t.fingerId)
                ContinueDrag(t.position);

            if (dragging && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
                EndDrag();
        }
    }

    void TryBeginDrag(Vector2 screenPos, int fingerId)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        int mask = LayerMask.GetMask("MovingTools");

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask)) return;
        if (hit.transform != transform) return;

        dragging = true;
        lastScreenPos = screenPos;
        firstDragFrame = true;
        
        MainGameManager.instance.FreeMovement = false;
        
        if (rotateAxis == Axis.X || rotateAxis == Axis.Y)
            layerRotationHandler.OnDragStart();
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

        float angle = delta.x + delta.y;

        float distance = Vector3.Distance(cam.transform.position, target.position);
        float rotationSpeed = distance * 0.2f;

        target.Rotate(GetAxisDirection(), angle * rotationSpeed, Space.World);

        lastScreenPos = screenPos;
        
        if (rotateAxis == Axis.X || rotateAxis == Axis.Y)
            layerRotationHandler.OnDragging();
    }

    void EndDrag()
    {
        dragging = false;
        
        MainGameManager.instance.FreeMovement = true;
        
        if (rotateAxis == Axis.X || rotateAxis == Axis.Y)
            layerRotationHandler.OnDragging();
    }

    Vector3 GetAxisDirection()
    {
        return rotateAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };
    }
}
