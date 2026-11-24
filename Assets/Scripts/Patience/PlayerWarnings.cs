using UnityEngine;

public class PlayerWarnings : MonoBehaviour
{
    public Camera roomCamera;
    public RectTransform roomWindow;
    public Transform target;
    // Positions the UI slightly above the player
    public Vector3 offset = new Vector3(0, 1f, 0);

    private RectTransform rectTransform;
    private RectTransform canvasRect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the UI and its parent canvas
        rectTransform = GetComponent<RectTransform>();
        // Get the canvas rect transform for positioning the symbol
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    // LateUpdate runs after Update, allowing the UI to follow the players head after they move.
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 viewportPosition = roomCamera.WorldToViewportPoint(target.position + offset);

        // Hide the warning symbols when the player is not in the camera view
        bool isVisible = viewportPosition.z > 0f &&
                         viewportPosition.x >= 0f && viewportPosition.x <= 1f &&
                         viewportPosition.y >= 0f && viewportPosition.y <= 1f;

        rectTransform.GetComponent<CanvasRenderer>().SetAlpha(isVisible ? 1f : 0f);

        if (!isVisible) return;


        if (viewportPosition.z <= 0f) return;

        // Convert the camera position to a position insidethe waiting room
        Vector2 windowPosition = new Vector2(
            (viewportPosition.x - 0.5f) * roomWindow.rect.width,
            (viewportPosition.y - 0.5f) * roomWindow.rect.height
            );

        // Covert that position into the world coordinates
        Vector3 worldPosition = roomWindow.TransformPoint(windowPosition);

        // Convert it into the canvas coordinates system
        Vector2 screenposition = RectTransformUtility.WorldToScreenPoint(null, worldPosition);

        // Convert the screen position into coordinates that the screen space canvas can use
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenposition, null, out Vector2 localPosition);

        // Move the Ui element to the players screen position
        rectTransform.anchoredPosition = localPosition;



        /* OUT OF USE CODE
         * 
         * Convert the players 3D world position into a position on on the screen
        Vector3 screenPosition = roomCamera.WorldToScreenPoint(target.position + offset);
        Debug.Log(
                   "Target: " + target.name +
                   " | World: " + target.position +
                   " | Camera: " + Camera.main.transform.position +
                   " | Screen: " + screenPosition
                 );

        // Ignore players positioned behind the camera
        if (screenPosition.z <= 0f)
        {
            return;
        }

        // Convert the screen position into coordinates that the screen space canvas can use
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out Vector2 localPosition);

        // Move the Ui element to the players screen position
        rectTransform.anchoredPosition = localPosition;
        Debug.Log("Warning position: " + localPosition);
        */
    }
}