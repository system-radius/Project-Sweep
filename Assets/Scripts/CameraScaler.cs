using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    private Camera worldCamera;

    [SerializeField]
    private float targetAspectX = 9f;
    [SerializeField]
    private float targetAspectY = 16f;

    [SerializeField]
    private float baseOrthographicSize = 5f;

    [SerializeField]
    private float padding = 200f;

    private void Awake()
    {
        worldCamera = GetComponent<Camera>();
        /*
        float targetAspect = targetAspectX / targetAspectY;
        float windowAspect = (float) Screen.width / (float) Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Rect rect = worldCamera.rect;
        if (scaleHeight < 1)
        {
            rect.width = 1;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;
        } else
        {
            float scaleWidth = 1f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0;
        }
        worldCamera.rect = rect;
        */
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

        Debug.Log("Calculated bounds: " + width + ", " + height);

        worldCamera.orthographicSize = Mathf.Max(width, height);
        worldCamera.orthographicSize += padding;
    }
}