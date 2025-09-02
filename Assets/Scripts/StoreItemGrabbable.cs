using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StoreItemGrabbable : XRGrabInteractable
{
    [HideInInspector] public Store store;
    public int itemIndex = 0; // Use nullable int for "unset"
    public bool inStore;
    private bool debug;
    protected override void Awake()
    {
        base.Awake();
        // Optionally, set interaction layers or other properties here
        if (inStore)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true; // Prevent physics until grabbed

        }
        if (store != null) debug = store.debug;
    }
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        // If not set up as a store item, allow normal grab
        if (!inStore)
        {
            base.OnSelectEntering(args);
            return;
        }

        // Check for null and valid index
        if (store == null ||  itemIndex < 0)
        {
            Debug.LogWarning("Store or itemIndex not set or out of range, denying grab.");
            return;
        }

        if (!store.TryGrabItem(itemIndex, gameObject))
        {
            Debug.Log("denying grab in sig");
            // Deny grab by not calling base method
            return;
        }
        Debug.Log("passed grab in sig");
        inStore = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = false; // Enable physics when grabbed
        base.OnSelectEntering(args);
    }
}