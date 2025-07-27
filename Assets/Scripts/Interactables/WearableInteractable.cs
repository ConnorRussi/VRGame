using UnityEngine;

public class WearableInteractable : MonoBehaviour
{
    public bool isWorn; // Indicates if the wearable item is currently worn
    public ParticleSystem smokeEffect; // The particle system for smoke effect
    public GameObject wearer; // The GameObject that is wearing this item
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isWorn = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Equip(GameObject putOnBy)
    {
        wearer = putOnBy;
        isWorn = true;
        rb.isKinematic = true;
        StartSmokeEffect(); // Start the smoke effect if applicable

    }
    public void Dequip()
    {
        isWorn = false;
        rb.isKinematic = false; // Make the Rigidbody non-kinematic when dequipped
        if (wearer != null)
        {
            wearer = null; // Clear the wearer reference
        }
        StopSmokeEffect(); // Stop the smoke effect if applicable
    }
    void StartSmokeEffect()
    {
        if (smokeEffect != null)
        {
            smokeEffect.Play();
        }
    }
    void StopSmokeEffect()
    {
        if (smokeEffect != null)
        {
            smokeEffect.Stop();
        }
    }
}
