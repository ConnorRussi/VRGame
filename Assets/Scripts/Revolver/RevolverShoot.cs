using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class RevolverShoot : MonoBehaviour
{
    [Header("Controller Variables")]
    public ActionBasedController grabbingXRController;
    public InputData inputData;
    public Revolver revolverSC;
    private bool prevLeftSecondaryButton = false;
    private bool prevRightSecondaryButton = false;


    [Header("Revolver Variables")]
    public Animator revAnimator;
    public bool grabbed;
    public bool leftHand;
    public bool rightHand;
    public bool cylinderOpened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created    
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireGunSignal);
        grabbable.selectEntered.AddListener(OnGrab);
        grabbable.selectExited.AddListener(OnRelease);
        grabbed = false;
        inputData = GameObject.Find("XR Origin (XR Rig)").GetComponent<InputData>();
        if (inputData == null)
        {
            Debug.LogError("InputData component not found in XR Origin (XR Rig).");
        }
        cylinderOpened = false;
    }

   /// <summary>
   /// Handles opening the cylinder when the secondary button is pressed on based on hadn it is held in
   /// </summary>
    void FixedUpdate()
    {
        if(revolverSC.owner == Revolver.GunOwner.NPC)
        {
            return; // Do not process input for NPCs
        }
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

    /// <summary>
    /// Handles the firing of the revolver when activated by starting the shoot animation
    /// </summary>
    /// <param name="args"></param>
    public void FireGun(Revolver.GunOwner gunOwner)
    {
        revolverSC.owner = gunOwner; // Set the owner of the revolver
        if (!revolverSC.readyToFire) return;
        if (revolverSC.currentAmmo > 0 || gunOwner == Revolver.GunOwner.NPC)
        {
           // Debug.Log("play animation");
            revAnimator.Play("ShootAnimation");
            return;
            //normal shoot animation
        }
        //dry fire animation
        revAnimator.Play("DryFire");
    }
    /// <summary>
    /// signals fun has been fired by the player
    /// </summary>
    /// <param name="args"></param>
    void FireGunSignal(ActivateEventArgs args)
    {
        FireGun(Revolver.GunOwner.Player); //player said to shoot so owned by player
       
    }
    /// <summary>
    /// Determines which hand it has been grabbed by and sets the leftHand or rightHand variable accordingly
    /// </summary>
    /// <param name="args"></param>
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
    /// <summary>
    /// Handles the release of the revolver when the grab is released resetting the hand variables accordingly
    /// </summary>
    /// <param name="args"></param>
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
    /// <summary>
    /// tells the revolver to open the cylinder
    /// </summary>
    void MoveCylinder()
    {
        cylinderOpened = true;
        Debug.Log("Cylinder opened!");
        revolverSC.OpenCylinder(leftHand);
        // Put your cylinder opening logic here
    }
}
