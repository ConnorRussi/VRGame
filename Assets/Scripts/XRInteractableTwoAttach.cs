using System.Data;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRInteractableTwoAttach : XRGrabInteractable
{
    public Transform leftAttachPoint;
    public Transform rightAttachPoint;
    public bool debug = false;
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        if(debug) Debug.Log("override OnSelectEntered called" + args.interactableObject.transform.name + " " + args.interactorObject.transform.tag);

        if (args.interactorObject.transform.CompareTag("Left Hand"))
        {
            attachTransform = leftAttachPoint;
        }
        else if (args.interactorObject.transform.CompareTag("Right Hand"))
        {
            attachTransform = rightAttachPoint;
        }
        base.OnSelectEntering(args);
    }
    


}
