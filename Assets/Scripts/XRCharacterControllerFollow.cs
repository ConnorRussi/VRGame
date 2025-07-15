using UnityEngine;

public class XRCharacterControllerFollow : MonoBehaviour
{
    public GameObject xrCameraObject; // Reference to the XR Camera GameObject
    private CharacterController characterController;

    [Tooltip("How far the camera can move from the center before the body follows (meters).")]
    public float deadZoneRadius = 0.15f;

    [Tooltip("How quickly the body follows the camera (0 = instant, 1 = never).")]
    [Range(0f, 1f)]
    public float followSmooth = 0.2f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (xrCameraObject == null) return;

        // Get the camera's local position relative to the XR Origin
        Vector3 cameraLocalPos = xrCameraObject.transform.localPosition;
        Vector3 horizontalOffset = new Vector3(cameraLocalPos.x, 0f, cameraLocalPos.z);

        // Only move if outside the dead zone
        if (horizontalOffset.magnitude > deadZoneRadius)
        {
            // Calculate the move vector (smoothed)
            Vector3 move = horizontalOffset * (1f - followSmooth);

            // Move the CharacterController (use Move for proper collision)
            characterController.Move(move);

            // Reset the camera's local XZ position to zero, keep Y (height) unchanged
            xrCameraObject.transform.localPosition = new Vector3(0f, cameraLocalPos.y, 0f);
        }
    }
}