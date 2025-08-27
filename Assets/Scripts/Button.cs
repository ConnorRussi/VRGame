using UnityEngine;

public class Button : MonoBehaviour
{
    public float maxDepth;
    public float maxHeight;
    public float activationDepth;

    public Transform visualTarget;
    public Vector3 localAxis;
    public float resetSpeed = 5;
    public float followAngleThreshold = 45;
    private Vector3 initialLocalPos;

    private Vector3 offset;
    private Transform pokeAttachTransform;

    private bool isFollowing = false;
    public GameObject pressingObject;

    private bool hasActivated = false; // Prevent multiple activations per press
    public GameObject objectToActivate;
    private IButtonInteractor interactorScript;

    void Start()
    {
        initialLocalPos = visualTarget.localPosition;
        interactorScript = objectToActivate?.GetComponent<IButtonInteractor>();
        if (objectToActivate != null && interactorScript == null)
        {
            Debug.LogWarning("The object to activate does not have an IButtonInteractor component!" + gameObject.name);
        }
    }

    public void Follow()
    {
        isFollowing = true;
        pokeAttachTransform = pressingObject.transform;
        offset = visualTarget.position - pokeAttachTransform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Left Hand") || other.CompareTag("Right Hand")) && pressingObject == null)
        {
            pressingObject = other.gameObject;
            Follow();
            hasActivated = false; // Reset activation state on new press
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == pressingObject)
        {
            isFollowing = false;
            pressingObject = null;
            hasActivated = false;
        }
    }

    void Update()
    {
        if (isFollowing)
        {
            Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
            Vector3 constrainedLocalTargetPosition = Vector3.Project(localTargetPosition, localAxis);

            visualTarget.position = visualTarget.TransformPoint(constrainedLocalTargetPosition);

            // Clamp local position
            visualTarget.localPosition = new Vector3(
                Mathf.Clamp(visualTarget.localPosition.x, maxDepth, maxHeight),
                Mathf.Clamp(visualTarget.localPosition.y, maxDepth, maxHeight),
                Mathf.Clamp(visualTarget.localPosition.z, maxDepth, maxHeight)
            );

            // Check activation along the axis
            float axisValue = Vector3.Dot(visualTarget.localPosition, localAxis.normalized);

            if (!hasActivated && axisValue <= activationDepth)
            {
                hasActivated = true;
                // Try to call Activate on the interactor script
                if (objectToActivate == null)
                {
                    Debug.LogWarning("No IButtonInteractor assigned to the button!" + gameObject.name);
                }
                else
                {
                    Debug.Log("Button Activated: " + gameObject.name);
                    interactorScript.Activate(); // Pass this button or any object you want
                }

            }
            // Reset activation if button is released above threshold
            else if (hasActivated && axisValue > activationDepth)
            {
                hasActivated = false;
            }
        }
        else
        {
            visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition, initialLocalPos, Time.deltaTime * resetSpeed);
        }
    }
}

// Define this interface somewhere in your project
public interface IButtonInteractor
{
    void Activate();
}