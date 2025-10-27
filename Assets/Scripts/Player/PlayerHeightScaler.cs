using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NormalizedHeight : MonoBehaviour
{
    public GameObject camera;
    public GameObject fullRig;
    public float camHeight;
    public float targetHeight = 1.7f; // Standard eye level (meters)
    public bool calibrate;

    void Start()
    {
        Invoke("Calibrate", 1f); // Delay to ensure XR rig is fully initialized
    }
    void Update()
    {
        if (calibrate)
        {
            calibrate = false;
            Calibrate();
        }
    }
    public void Calibrate()
    {
        camHeight = camera.transform.localPosition.y;
        float heightOffset = targetHeight - camHeight;
        
        fullRig.transform.position = new Vector3(fullRig.transform.position.x, fullRig.transform.position.y + heightOffset, fullRig.transform.position.z);

        Debug.Log($"NormalizedHeight: Applied offset of {heightOffset} meters.");
    }
}
