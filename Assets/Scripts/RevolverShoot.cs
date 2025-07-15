using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class RevolverShoot : MonoBehaviour
{
    public ActionBasedController grabbingXRController;
    public InputData inputData;
    public Revolver revolverSC;
    public Animator revAnimator;
    public bool grabbed;
    public bool leftHand;
    public bool rightHand;
    public bool cylinderOpened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created    
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireGun);
        grabbable.selectEntered.AddListener(OnGrab);
        grabbable.selectExited.AddListener(OnRelease);
        grabbed = false;
        inputData = GameObject.Find("XR Origin (XR Rig)").GetComponent<InputData>();
        if (inputData == null)
        {
            Debug.LogError("InputData component not found in GameManager.");
        }
        cylinderOpened = false;
    }

    // Update is called once per frame
    private bool prevLeftSecondaryButton = false;
    private bool prevRightSecondaryButton = false;

    void Update()
    {
        // Left hand logic
        bool leftSecondaryButton = false;
        if (leftHand && inputData._leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out leftSecondaryButton))
        {
            if (leftSecondaryButton && !prevLeftSecondaryButton)
            {
                MoveCylinder();
            }
        }
        prevLeftSecondaryButton = leftSecondaryButton;

        // Right hand logic
        bool rightSecondaryButton = false;
        if (rightHand && inputData._rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out rightSecondaryButton))
        {
            if (rightSecondaryButton && !prevRightSecondaryButton)
            {
                MoveCylinder();
            }
        }
        prevRightSecondaryButton = rightSecondaryButton;
    }
    //public GameObject bulletSpawnPoint, bulletPrefab;
    //public GameObject muzzleFlashPrefab;
    
    //public float bulletSpeed;

    void FireGun(ActivateEventArgs args)
    {

        if (!revolverSC.readyToFire) return;
        if (revolverSC.currentAmmo > 0)
        {
            Debug.Log("play animation");
            revAnimator.Play("ShootAnimation");
            return;
            //normal shoot animation
        }
        //dry fire animation
        revAnimator.Play("DryFire");




    }
    void OnGrab(SelectEnterEventArgs args)
    {
        grabbed = true;
        // Try to get the ActionBasedController from the interactor
        var interactorObj = args.interactorObject as XRBaseInteractor;
        if (interactorObj != null)
        {
            if(interactorObj.CompareTag("Left Hand"))
            {
                leftHand = true;
                //rightHand = false;
            }
            else if (interactorObj.CompareTag("Right Hand"))
            {
                //leftHand = false;
                rightHand = true;
            }
            else
            {
                Debug.LogWarning("Interactor does not have a recognized tag.");
            }
            grabbingXRController = interactorObj.GetComponent<ActionBasedController>();
            Debug.Log("Grabbed by: " + interactorObj.name);
        }
        else
        {
            grabbingXRController = null;
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        var interactorObj = args.interactorObject as XRBaseInteractor;
        if (interactorObj.CompareTag("Left Hand"))
        {
            leftHand = false;
        }
        else if (interactorObj.CompareTag("Right Hand"))
        {
            rightHand = false;
        }
        else
        {
            Debug.LogWarning("Interactor does not have a recognized tag to release.");
        }
        //Debug.Log("released");
        grabbed = false;
        grabbingXRController = null;
    }
    void MoveCylinder()
    {
        cylinderOpened = true;
        Debug.Log("Cylinder opened!");
        revolverSC.OpenCylinder(leftHand);
        // Put your cylinder opening logic here
    }
}
