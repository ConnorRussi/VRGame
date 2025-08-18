using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PhysicsSocket : XRSocketInteractor
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // Ensure physics stays enabled
        Rigidbody rb = args.interactableObject.transform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // keep reacting to physics
            rb.useGravity = true;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // Nothing special – let physics handle it when leaving
    }

    // Optional: only accept bottles
    // public override bool CanSelect(IXRSelectInteractable interactable)
    // {
    //     return interactable.transform.CompareTag("Bottle") && base.CanSelect(interactable);
    // }
}