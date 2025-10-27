using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StoreItemGrabbable : XRGrabInteractable
{
    [HideInInspector] public Store store;
    public int itemIndex = 0; // Use nullable int for "unset"
    public bool inStore;
    public InteractionLayerMask interactAbleLayer;
    private bool debug;
    private Rigidbody rb;

    /// <summary>
    /// Initializes grabbable, sets kinematic if in store, stores debug flag and interaction layer.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        if (inStore)
        {
            rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; //make kinematic so it doesn't fall out of the store
                rb.useGravity = false;
            }
            else
            {
                Debug.LogError("No Rigidbody found on store item: " + gameObject.name);
            }
        }
        if (store != null) debug = store.debug;
        interactAbleLayer = interactionLayers;
    }

    /// <summary>
    /// Updates the interaction layer to allow or block grabbing.
    /// </summary>
    public void UpdateInteractionLayer(bool grabbable)
    {
        //Debug.Log("update interaction layer called grabbable: " + grabbable + " inStore: " + inStore + " item: " + gameObject.name);
        if (!inStore) return;
        if (!grabbable) interactionLayers = 0;
        else interactionLayers = interactAbleLayer;
    }

    /// <summary>
    /// Called when item is grabbed; marks item as not in store and calls base logic.
    /// </summary>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (inStore)
        {
            store.PurchaseItem(itemIndex, gameObject);
            inStore = false;//mark not in store

        }
        //not in store so base logic

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if(rb != null)
        {
            rb.isKinematic = false; //make non kinematic so it can be thrown around
            rb.useGravity = true;
        }
    }
}