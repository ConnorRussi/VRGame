using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Drawer : MonoBehaviour
{
    [Header("Universal Drawer Settings")]
    public Rigidbody drawerRb;
    public float closeThreshold = 0.05f; // how close (in meters) before it moves back to closed position
    public Vector3 closedLocalPos; // store the local position when closed

    [Header("Register Drawer")]
    public bool regDrawer = false; // if true will act like a register drawer
    public bool isLocked = false;
    public float lockThreshold = 0.01f; // how close (in meters) before it locks in place
    public bool isOpening = false;
    public Vector3 openTargetPosition = new Vector3(0f, 0f, -0.25f); // editable in inspector
    public XRGrabInteractable grabInteractable; 
    public float drawerOpenDelay; // time to wait before finishing opening the drawer
    public Register register; // reference to the register script


    [Header("Jointed Drawer Settings")]
    public ConfigurableJoint joint; // reference to the Configurable Joint component

    public enum DriveAxis { X, Y, Z }
    public DriveAxis driveAxis = DriveAxis.Z; // set in Inspector

    public Vector3 closedTargetPosition = new Vector3(0f, 0f, 0.25f); // editable in inspector
    public float driveForce = 50f; // editable in inspector

    void Start()
    {
        closedLocalPos = transform.localPosition;
        drawerRb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (regDrawer)
        {
            LockDrawer(); // start locked if a register drawer
            if(register == null)
            {
                Debug.LogError("Register drawer missing reference to Register script");
            }
        }
        
    }

    void Update()
    {
        if (!isLocked)
        {
            float distance = Vector3.Distance(transform.localPosition, closedLocalPos);
            //Debug.Log("Distance to closed position: " + distance);

            if (distance < closeThreshold && !isOpening)
            {
                Debug.Log("Closing drawer");

                if (joint != null)
                {
                    joint.targetPosition = closedTargetPosition;
                    ApplyDrive(driveForce);
                }

                if (distance < lockThreshold)
                {
                    LockDrawer(); // snap back into place if close enough
                }
                return;
            }
            if(!isOpening) ApplyDrive(0f); // stop spring if moved away from closed position
        }
    }

    public void ReleaseDrawer()
    {
        Debug.Log("Releasing drawer");
        if (regDrawer)
        {
            isOpening = true;
            isLocked = false;
            drawerRb.isKinematic = false; // allow it to move freely
            if (grabInteractable != null)
                grabInteractable.enabled = true;
            if (joint != null)
            {
                joint.targetPosition = openTargetPosition;
                ApplyDrive(driveForce);
            }

            StartCoroutine(FinishOpenDrawer());
        }
    }

    private System.Collections.IEnumerator FinishOpenDrawer()
    {
        yield return new WaitForSeconds(drawerOpenDelay); // Wait for the drawer to move out

        if (joint != null)
        {
            ApplyDrive(0f); // stop spring
            joint.targetPosition = Vector3.zero;
        }
        isOpening = false;
    }
    /// <summary>
    /// Moves the drawer to the closed position, if reg drawer it locsk it in place
    /// </summary>
    public void LockDrawer()
    {
        
        Debug.Log("Locking drawer");
        // snap back into place
        transform.localPosition = closedLocalPos;

        drawerRb.linearVelocity = Vector3.zero;
        drawerRb.angularVelocity = Vector3.zero;

        if (regDrawer)
        {
            drawerRb.isKinematic = true; // lock it in place
            isLocked = true;
            if (grabInteractable != null)grabInteractable.enabled = false; // Disable grabbing
            register.ProcessDrawer(); // process items in the register
        }
    }

    /// <summary>
    /// Applies the drive force to the selected axis.
    /// </summary>
    private void ApplyDrive(float force)
    {
        JointDrive drive = new JointDrive
        {
            positionSpring = force,
            positionDamper = 0f,
            maximumForce = Mathf.Infinity
        };

        switch (driveAxis)
        {
            case DriveAxis.X:
                joint.xDrive = drive;
                break;
            case DriveAxis.Y:
                joint.yDrive = drive;
                break;
            case DriveAxis.Z:
                joint.zDrive = drive;
                break;
        }
    }
}
