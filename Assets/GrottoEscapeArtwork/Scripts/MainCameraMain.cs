using UnityEngine;

public class DynamicZoomCamera : MonoBehaviour
{
    public Transform PlayerCharacterOne; // Reference to the first character
    public Transform PlayerCharacterTwo; // Reference to the second character
    public float zoomSpeed = 5f; // Speed at which the camera zooms in/out
    public float minZoom = 5f; // Minimum camera zoom
    public float maxZoom = 15f; // Maximum camera zoom
    public float zoomPadding = 2f; // Extra space around characters
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

        // Update camera position
        Vector3 midpoint = (PlayerCharacterOne.position + PlayerCharacterTwo.position) / 2f;
        transform.position = midpoint + offset;

        // Calculate distance between characters
        float distance = Vector2.Distance(PlayerCharacterOne.position, PlayerCharacterTwo.position);

        // Adjust camera zoom
        float targetZoom = Mathf.Clamp(distance + zoomPadding, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
