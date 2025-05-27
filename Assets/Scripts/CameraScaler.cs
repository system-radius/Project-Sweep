using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [SerializeField]
    private TouchManager touchManager;

    private Camera worldCamera;
    [SerializeField]
    private float cameraSpeed = 8f;

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
    }

    private void OnDisable()
    {
        touchManager.OnZoomStart -= ZoomStart;
        touchManager.OnZoomEnd -= ZoomEnd;
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
            worldCamera.orthographicSize = Mathf.Lerp(worldCamera.orthographicSize, targetOrthoSize, Time.deltaTime * cameraSpeed);
        }
        prevDistance = distance;
    }

    private void Update()
    {
        if (zooming)
        {
            Zoom();
        }
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
    }
}