using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Coaster : MonoBehaviour
{
    public bool claimed;
    public NPC owner;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // HIGHLIGHT: Coroutine to ensure cup is placed and not held
    private System.Collections.IEnumerator WaitAndCheckCup(Cup cup, GameObject cupObject)
    {
        // Wait a short time to ensure the cup is placed (tweak as needed)
        yield return new WaitForSeconds(0.75f);

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
