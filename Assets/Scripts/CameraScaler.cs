using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [SerializeField]
    private TouchManager touchManager;

    private Camera worldCamera;
    [SerializeField]
    private float zoomSpeed = 8f;
    [SerializeField]
    private float dragSpeed = 0.5f;

    [SerializeField]
    private float targetAspectX = 9f;
    [SerializeField]
    private float targetAspectY = 16f;

    [SerializeField]
    private float baseOrthographicSize = 5f;
    private float maxOrthoSize = 5f;

    [SerializeField]
    private float padding = 200f;

    private float prevDistance = 0f;
    private float distance = 0f;

    public InputAction primaryTouchPosition;
    public InputAction secondaryTouchPosition;

    private bool zooming = false;
    private bool moving = false;
    private Vector2 prevPosition = Vector2.zero;
    private Vector2 deltaVector = Vector2.zero;

    private Vector2 minBounds = Vector2.zero;
    private Vector2 maxBounds = Vector2.zero;

    private float halfHeight = 5f;
    private float halfWidth = 5f;

    private void Awake()
    {
        worldCamera = GetComponent<Camera>();
        primaryTouchPosition = touchManager.primaryTouchPosition;
        secondaryTouchPosition = touchManager.secondaryTouchPosition;
    }

    private void OnEnable()
    {
        touchManager.OnZoomStart += ZoomStart;
        touchManager.OnZoomEnd += ZoomEnd;
        touchManager.OnCameraMove += Move;
    }

    private void OnDisable()
    {
        touchManager.OnZoomStart -= ZoomStart;
        touchManager.OnZoomEnd -= ZoomEnd;
        touchManager.OnCameraMove -= Move;
    }

    private void ZoomStart()
    {
        zooming = true;
    }

    private void ZoomEnd()
    {
        distance = prevDistance = 0f;
        zooming = false;
    }

    private void Zoom()
    {
        distance = Vector2.Distance(primaryTouchPosition.ReadValue<Vector2>(), secondaryTouchPosition.ReadValue<Vector2>());
        float targetOrthoSize = maxOrthoSize;
        float diff = Mathf.RoundToInt(distance - prevDistance);
        if (diff < 0)
        {
            targetOrthoSize = Mathf.Clamp(worldCamera.orthographicSize + 1, baseOrthographicSize, maxOrthoSize);
        }
        else if (diff > 0)
        {
            targetOrthoSize = Mathf.Clamp(worldCamera.orthographicSize - 1, baseOrthographicSize, maxOrthoSize);
            
        }

        if (diff != 0)
        {
            worldCamera.orthographicSize = Mathf.Lerp(worldCamera.orthographicSize, targetOrthoSize, Time.deltaTime * zoomSpeed);
            ClampCameraPosition(new Vector2(worldCamera.transform.position.x, worldCamera.transform.position.y));
        }
        prevDistance = distance;
    }

    private void Move(Vector2 delta)
    {
        float deltaX = -delta.x * Time.deltaTime + worldCamera.transform.position.x;
        float deltaY = -delta.y * Time.deltaTime + worldCamera.transform.position.y;

        //Debug.LogError("Delta: (" + delta.x + ", " + delta.y + ")");
        //Debug.LogError("Delta: (" + delta.x + ", " + delta.y + "), position: (" + worldCamera.transform.position.x + ", " + worldCamera.transform.position.y + "), after: (" + deltaX + ", " + deltaY + ")");

        ClampCameraPosition(new Vector2((float)deltaX, (float)deltaY));
        //worldCamera.transform.Translate(-delta.x * Time.deltaTime, -delta.y * Time.deltaTime, 0);
    }

    private void Update()
    {
        if (zooming)
        {
            Zoom();
        }
    }

    private void ClampCameraPosition(Vector2 position)
    {
        halfHeight = worldCamera.orthographicSize / 1.25f;
        halfWidth = worldCamera.aspect * worldCamera.orthographicSize;

        float x = ClampValue(position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float y = ClampValue(position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        //Debug.LogError("[" + worldCamera.orthographicSize + "] :: [" + position.x + " -> " + x + ", " + position.y + " -> " + y + "] --> X bounds: [" + (minBounds.x + halfWidth) + ", " + (maxBounds.x - halfWidth) + "], Y bounds: [" + (minBounds.y + halfHeight) + ", " + (maxBounds.y - halfHeight) + "]");
        worldCamera.transform.position = new Vector3(x, y, worldCamera.transform.position.z);
    }

    private float ClampValue(float value, float min, float max)
    {
        if (min > max)
        {
            Debug.LogError("Returning center due to inverted clamping: " + ((min + max) / 2f));
            return (min + max) / 2f;
        }

        return Mathf.Clamp(value, min, max);
    }

    public void AdjustCameraSize(Transform parentTransform)
    {
        float targetRatio = targetAspectX / targetAspectY;
        float screenAspectRatio = (float)Screen.width / Screen.height;

        Bounds bounds = new Bounds(parentTransform.GetChild(0).position, Vector3.zero);
        foreach(Transform t in parentTransform)
        {
            bounds.Encapsulate(t.position);
        }
        bounds.Expand(padding);

        transform.position = new Vector3(bounds.center.x, bounds.center.y, transform.position.z);
        float width = bounds.size.x / (2f * screenAspectRatio);
        float height = bounds.size.y / 2f;

        worldCamera.orthographicSize = Mathf.Max(width, height);
        worldCamera.orthographicSize += padding;
        maxOrthoSize = worldCamera.orthographicSize;

        minBounds = bounds.min;
        maxBounds = bounds.max;

        halfHeight = (maxOrthoSize - padding);
        halfWidth = worldCamera.aspect * (maxOrthoSize - padding);
        //halfWidth = worldCamera.aspect * halfHeight;

        Debug.LogError("Center: " + bounds.center + ", min: " + minBounds.ToString() + ", max: " + maxBounds + ", halfHeight: " + halfHeight + ", halfWidth: " + halfWidth);
    }
}