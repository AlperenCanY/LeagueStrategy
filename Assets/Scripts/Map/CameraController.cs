using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;
    public SpriteRenderer mapRenderer;

    [Header("Keyboard Movement")]
    public float moveSpeed = 8f;
    public float movementSmoothTime = 0.12f;

    [Header("Mouse Drag")]
    public float dragSensitivity = 0.85f;
    public float dragSmoothTime = 0.06f;

    [Header("Zoom")]
    public float zoomSpeed = 4f;
    public float zoomSmoothTime = 0.10f;
    public float minZoom = 2.5f;
    public float maxZoom = 8f;

    private Vector3 targetPosition;
    private Vector3 movementVelocity;

    private float targetZoom;
    private float zoomVelocity;

    private Vector3 lastMousePosition;
    private bool isDragging;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        targetPosition = transform.position;
        targetZoom = targetCamera.orthographicSize;
    }

    private void Update()
    {
        HandleKeyboardMovement();
        HandleMouseDrag();
        HandleZoom();
    }

    private void LateUpdate()
    {
        ClampTargetPositionToMap();

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref movementVelocity,
            movementSmoothTime
        );

        targetCamera.orthographicSize = Mathf.SmoothDamp(
            targetCamera.orthographicSize,
            targetZoom,
            ref zoomVelocity,
            zoomSmoothTime
        );
    }

    private void HandleKeyboardMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, vertical, 0f).normalized;

        if (inputDirection.sqrMagnitude <= 0.01f)
            return;

        float zoomMultiplier = targetZoom / maxZoom;
        float adjustedSpeed = moveSpeed * Mathf.Lerp(0.65f, 1.35f, zoomMultiplier);

        targetPosition += inputDirection * adjustedSpeed * Time.deltaTime;
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }

        if (!isDragging)
            return;

        Vector3 currentMousePosition = Input.mousePosition;

        Vector3 lastWorldPosition = targetCamera.ScreenToWorldPoint(lastMousePosition);
        Vector3 currentWorldPosition = targetCamera.ScreenToWorldPoint(currentMousePosition);

        Vector3 dragDifference = lastWorldPosition - currentWorldPosition;

        targetPosition += dragDifference * dragSensitivity;

        lastMousePosition = currentMousePosition;
    }

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) <= 0.01f)
            return;

        targetZoom -= scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    private void ClampTargetPositionToMap()
    {
        if (mapRenderer == null || targetCamera == null)
            return;

        Bounds mapBounds = mapRenderer.bounds;

        float cameraHeight = targetZoom;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        float minX = mapBounds.min.x + cameraWidth;
        float maxX = mapBounds.max.x - cameraWidth;
        float minY = mapBounds.min.y + cameraHeight;
        float maxY = mapBounds.max.y - cameraHeight;

        Vector3 clampedPosition = targetPosition;

        if (minX > maxX)
            clampedPosition.x = mapBounds.center.x;
        else
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);

        if (minY > maxY)
            clampedPosition.y = mapBounds.center.y;
        else
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

        clampedPosition.z = transform.position.z;
        targetPosition = clampedPosition;
    }
}