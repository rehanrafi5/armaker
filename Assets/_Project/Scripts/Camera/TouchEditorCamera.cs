using System.Collections;
using ARMarker;
using UnityEngine;

public class TouchEditorCamera : MonoBehaviour
{
    CameraPerspectiveSwitcher thisCameraPerspectiveSwitcher;
    
    [Header("Pivot Transform")]
    public Transform pivot; // assign empty GameObject as pivot
    private Vector3 pivotDefaultPos;

    [Header("Rotation")]
    public float rotationSpeed = 0.2f;
    public float minPitch = -80f;
    public float maxPitch = 80f;
    public float minYaw = -360f;
    public float maxYaw = 360f;

    [Header("Pan")]
    public float panSpeed = 0.01f;
    public Vector3 pivotMin;
    public Vector3 pivotMax;

    [Header("Zoom")]
    public float zoomSpeed = 0.02f;
    public float minDistance = 1f;
    public float maxDistance = 50f;

    private float yaw;
    private float pitch;
    private float distance;

    void Start()
    {
        thisCameraPerspectiveSwitcher = GetComponent<CameraPerspectiveSwitcher>();
        
        pivotDefaultPos = pivot.position;
    }
    public void SyncFromCurrentTransform()
    {
        Vector3 dir = transform.position - pivot.position;
        distance = dir.magnitude;

        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        if (!MainGameManager.instance.FreeMovement)
            return;
        if (!thisCameraPerspectiveSwitcher.is3DPerspective)
        {
            return;
        }
#if UNITY_EDITOR
        HandleEditorInput();
#else
        HandleTouchInput();
#endif
        UpdateCameraPosition();
    }
    void RotateCamera(Vector2 delta)
    {
        yaw += delta.x * rotationSpeed;
        pitch -= delta.y * rotationSpeed;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        yaw = ClampAngle(yaw, minYaw, maxYaw);
    }

    // ---------------- PAN ----------------
    void PanPivot(Vector2 delta)
    {
        Vector3 right = transform.right;
        Vector3 up = transform.up; // camera local up

        Vector3 move = (-right * delta.x + -up * delta.y) * panSpeed;
        pivot.position += move;

        // clamp pivot
        pivot.position = new Vector3(
            Mathf.Clamp(pivot.position.x, pivotMin.x, pivotMax.x),
            Mathf.Clamp(pivot.position.y, pivotMin.y, pivotMax.y),
            Mathf.Clamp(pivot.position.z, pivotMin.z, pivotMax.z)
        );
    }

    // ---------------- ZOOM ----------------
    void Zoom(float delta)
    {
        distance -= delta * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    // ---------------- UPDATE CAMERA ----------------
    void UpdateCameraPosition()
    {
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = pivot.position + rot * new Vector3(0, 0, -distance);
        transform.LookAt(pivot.position);
    }

    // ---------------- INPUT ----------------
    void HandleEditorInput()
    {
        // Rotate
        if (Input.GetMouseButton(0))
        {
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            RotateCamera(delta * 10f);
        }

        // Pan
        if (Input.GetMouseButton(1))
        {
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            PanPivot(delta * 0.5f);
        }

        // Zoom
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
            Zoom(scroll * 2f);
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
                RotateCamera(t.deltaPosition);
        }
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // Pan
            Vector2 delta = (t0.deltaPosition + t1.deltaPosition) * 0.5f;
            PanPivot(delta * 0.01f);

            // Zoom
            Vector2 p0Prev = t0.position - t0.deltaPosition;
            Vector2 p1Prev = t1.position - t1.deltaPosition;
            float prevDist = Vector2.Distance(p0Prev, p1Prev);
            float currDist = Vector2.Distance(t0.position, t1.position);
            float pinchDelta = currDist - prevDist;
            Zoom(pinchDelta);
        }
    }

    // ---------------- HELPERS ----------------
    float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);
        min = NormalizeAngle(min);
        max = NormalizeAngle(max);

        if (min < max)
            return Mathf.Clamp(angle, min, max);

        if (angle > min || angle < max)
            return angle;

        float distToMin = Mathf.Abs(Mathf.DeltaAngle(angle, min));
        float distToMax = Mathf.Abs(Mathf.DeltaAngle(angle, max));
        return distToMin < distToMax ? min : max;
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    public void OnResetBtnClick()
    {
        ResetToDefault3D_Smooth();
    }
    public void ResetToDefault3D_Smooth(float duration = 0.5f)
    {
        if (!thisCameraPerspectiveSwitcher.is3DPerspective)
            return;

        StopAllCoroutines();
        StartCoroutine(ResetRoutine(duration));
    }

    private IEnumerator ResetRoutine(float duration)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = thisCameraPerspectiveSwitcher.position3D;
        Quaternion endRot = thisCameraPerspectiveSwitcher.rotation3D;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);

            transform.position = Vector3.Lerp(startPos, endPos, p);
            transform.rotation = Quaternion.Slerp(startRot, endRot, p);
            pivot.position = Vector3.Lerp(pivot.position, pivotDefaultPos, p);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;
        pivot.position = pivotDefaultPos;

        SyncFromCurrentTransform();
    }
    public void ShiftTo2D()
    {
        pivot.position = pivotDefaultPos;
    }
}
