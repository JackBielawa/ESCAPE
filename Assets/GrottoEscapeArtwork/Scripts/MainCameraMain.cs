using UnityEngine;

public class DynamicZoomCamera : MonoBehaviour
{
    public float zoomSpeed = 5f;      // Speed at which the camera zooms in/out
    public float minZoom = 8f;        // Minimum camera zoom
    public float maxZoom = 14f;       // Maximum camera zoom
    public float zoomPadding = 2f;    // Extra space around characters
    public float verticalOffset = 2f; // Adjust this to remove the bottom space
    public Vector3 offset = new Vector3(0, 0, -10); // Offset for camera position

    private Transform playerOneTransform;
    private Transform playerTwoTransform;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("DynamicZoomCamera requires a Camera component!");
            return;
        }

        // Find players by their tags
        GameObject playerOneObj = GameObject.FindGameObjectWithTag("Player");
        GameObject playerTwoObj = GameObject.FindGameObjectWithTag("PlayerTwo");

        if (playerOneObj == null)
        {
            Debug.LogError("No GameObject found with the 'Player' tag.");
            return;
        }
        if (playerTwoObj == null)
        {
            Debug.LogError("No GameObject found with the 'PlayerTwo' tag.");
            return;
        }

        playerOneTransform = playerOneObj.transform;
        playerTwoTransform = playerTwoObj.transform;

        // At the start, focus the camera on the players' midpoint:
        float midpointX = (playerOneTransform.position.x + playerTwoTransform.position.x) / 2f;
        float midpointY = (playerOneTransform.position.y + playerTwoTransform.position.y) / 2f + verticalOffset;

        // Set initial camera position to center on both players
        transform.position = new Vector3(midpointX, midpointY, offset.z);

        // Start fully zoomed in (minZoom) to focus on players
        cam.orthographicSize = minZoom;
    }

    void LateUpdate()
    {
        if (playerOneTransform == null || playerTwoTransform == null || cam == null)
            return;

        // Calculate the midpoint between the players
        float midpointX = (playerOneTransform.position.x + playerTwoTransform.position.x) / 2f;
        float midpointY = (playerOneTransform.position.y + playerTwoTransform.position.y) / 2f + verticalOffset;

        // Update camera position to follow both players (centered)
        Vector3 newPosition = new Vector3(midpointX, midpointY, offset.z);
        transform.position = newPosition;

        // Calculate the horizontal distance between characters
        float horizontalDistance = Mathf.Abs(playerOneTransform.position.x - playerTwoTransform.position.x);

        // Determine the target zoom based on how far apart they are
        float targetZoom = Mathf.Clamp(horizontalDistance + zoomPadding, minZoom, maxZoom);

        // Smoothly transition to the target zoom over time
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
