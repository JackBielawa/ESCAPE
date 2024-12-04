using UnityEngine;

public class DynamicZoomCamera : MonoBehaviour
{
    public Transform PlayerCharacterOne; // Reference to the first character
    public Transform PlayerCharacterTwo; // Reference to the second character
    public float zoomSpeed = 5f; // Speed at which the camera zooms in/out
    public float minZoom = 8f; // Minimum camera zoom
    public float maxZoom = 14f; // Maximum camera zoom
    public float zoomPadding = 2f; // Extra space around characters
    public float verticalOffset = 2f; // Adjust this to remove the bottom space
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

        // Calculate the midpoint between the players on the Y-axis and adjust vertically
        float midpointY = (PlayerCharacterOne.position.y + PlayerCharacterTwo.position.y) / 2f;

        // Shift the camera up by the vertical offset
        float adjustedMidpointY = midpointY + verticalOffset;

        // Set the camera position, ensuring it follows the Y midpoint with an offset
        Vector3 newPosition = new Vector3(transform.position.x, adjustedMidpointY, offset.z);
        transform.position = newPosition;

        // Calculate the horizontal distance between characters
        float horizontalDistance = Mathf.Abs(PlayerCharacterOne.position.x - PlayerCharacterTwo.position.x);

        // Adjust zoom level based on horizontal distance
        float targetZoom = Mathf.Clamp(horizontalDistance + zoomPadding, minZoom, maxZoom);

        // Smoothly transition to the target zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
