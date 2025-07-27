using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Coaster : MonoBehaviour
{
    public bool claimed;
    float timeBetweenCupCheck = 0.75f; // Time to wait before checking if the cup is placed correctly
    public NPC owner;

    /// <summary>
    /// Claims the coaster for the NPC.
    /// If the coaster is already claimed, it logs a warning.
    /// </summary>
    /// <param name="npc"></param>
    public void Claim(NPC npc)
    {
        if (claimed)
        {
            Debug.LogWarning("Coaster is already claimed by: " + owner.name);
            return;
        }

        claimed = true;
        owner = npc;
        npc.currentCoaster = this;
        gameObject.SetActive(true); // Reactivate the coaster if it was deactivated
        foreach (Transform child in transform) // HIGHLIGHT
        {
            child.gameObject.SetActive(true); // HIGHLIGHT
        }
        Debug.Log("Coaster claimed by: " + owner.name);
    }
/// <summary>
/// Releases the coaster from the NPC.
/// If the coaster is not claimed, it logs a warning.
/// </summary>
    public void releaseCoaster()
    {
        if (!claimed)
        {
            Debug.LogWarning("Coaster is not claimed, cannot release.");
            return;
        }

        Debug.Log("Releasing coaster from: " + owner.name);
        claimed = false;
        owner.currentCoaster = null;
        owner = null;
        gameObject.SetActive(false); // Optionally deactivate the coaster
    }
/// <summary>
/// Handles the trigger event when a cup enters the coaster's collider.
/// If the coaster is claimed, it starts a coroutine to check if the cup is placed correctly
/// </summary>
/// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Cup>() == null)
        {
            Debug.Log("Coaster triggered by non-cup object: " + other.name);
            return;
        }
        if (claimed)
        {
            // Start coroutine to check placement and XR grab status
            StartCoroutine(WaitAndCheckCup(other.gameObject.GetComponent<Cup>(), other.gameObject));
        }
    }

/// <summary>
/// Coroutine to check if the cup is placed correctly on the coaster.
/// If the cup is still overlapping the coaster after a short delay, it compares the drink in the cup to the NPC's order.
/// If the cup is still being held by XR Interaction, it does not compare.
/// </summary>
/// <param name="cup"></param>
/// <param name="cupObject"></param>
/// <returns></returns>
    private System.Collections.IEnumerator WaitAndCheckCup(Cup cup, GameObject cupObject)
    {
        // Wait a short time to ensure the cup is placed (tweak as needed)
        yield return new WaitForSeconds(timeBetweenCupCheck);
        if(cup == null || cupObject == null)
        {
           yield break; // Exit if cup or cupObject is null
        }
        // Check if the cup is still overlapping the coaster
        Collider coasterCollider = GetComponent<Collider>();
        Collider cupCollider = cupObject.GetComponent<Collider>();
        bool isStillOnCoaster = coasterCollider != null && cupCollider != null && coasterCollider.bounds.Intersects(cupCollider.bounds);

        // Check if the cup is still being held by XR Interaction
        XRGrabInteractable grabInteractable = cupObject.GetComponent<XRGrabInteractable>();
        bool isHeld = grabInteractable != null && grabInteractable.isSelected;

        if (isStillOnCoaster && !isHeld)
        {
            owner.CompareDrinkToOrder(cup, cupObject.tag, cupObject);
        }
        else
        {
            Debug.Log("Cup was not left on the coaster or is still being held.");
        }
    }
    
    
}
