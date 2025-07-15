using UnityEngine;

public class FollowCamera : MonoBehaviour
{
public GameObject cameraObject; // The camera object to follow
    public float yOffset; // Offset from the camera's position
    public float backOffset = 0.5f; // How far behind the camera to stay
    public float positionDamp = 8f; // Higher = snappier, lower = smoother
    public float rotationDamp = 8f; // Higher = snappier, lower = smoother

    void Update()
    {
        // Calculate the target position just behind the camera
        Vector3 backDirection = -cameraObject.transform.forward;
        Vector3 targetPosition = cameraObject.transform.position + backDirection * backOffset;
        // Find the middle point between the ground (assumed y=0) and the camera
        float cameraY = cameraObject.transform.position.y;
        float midY = cameraY * 0.5f;
        targetPosition.y = midY - yOffset;
        // Smoothly move towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionDamp);

        // Match only the Y rotation, but dampened
        float targetY = cameraObject.transform.eulerAngles.y;
        float currentY = transform.eulerAngles.y;
        float newY = Mathf.LerpAngle(currentY, targetY, Time.deltaTime * rotationDamp);
        transform.rotation = Quaternion.Euler(0f, newY, 0f);
    }
}
