using UnityEngine;

public class DynamicZoomCamera : MonoBehaviour
{
    public Transform PlayerCharacterOne; // Reference to the first character
    public Transform PlayerCharacterTwo; // Reference to the second character
    public float zoomSpeed = 5f; // Speed at which the camera zooms in/out
    public float minZoom = 7f; // Adjusted minimum camera zoom (closer POV)
    public float maxZoom = 12f; // Adjusted maximum camera zoom (broader POV)
    public float zoomPadding = 3f; // Extra space around characters
    public Vector3 offset = new Vector3(0, 0, -10); // Offset to position the camera

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("DynamicZoomCamera requires a Camera component!");
        }
    }

    void LateUpdate()
    {
        if (PlayerCharacterOne == null || PlayerCharacterTwo == null || cam == null)
            return;

        // Calculate vertical midpoint (Y-axis only)
        float midpointY = (PlayerCharacterOne.position.y + PlayerCharacterTwo.position.y) / 2f;

        // Adjust camera position to follow the vertical midpoint with offset
        Vector3 newPosition = new Vector3(transform.position.x, midpointY, offset.z);
        transform.position = newPosition;

        // Calculate horizontal distance between characters
        float horizontalDistance = Mathf.Abs(PlayerCharacterOne.position.x - PlayerCharacterTwo.position.x);

        // Dynamically clamp zoom range based on desired distance thresholds
        float targetZoom = Mathf.Clamp(horizontalDistance + zoomPadding, minZoom, maxZoom);

        // Smoothly adjust the camera zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
