using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CInteractable : MonoBehaviour
{
    public GameObject mesh;
    Rigidbody rb;
    [Header("Particle Systems")]
    public ParticleSystem breakParticles;
    public ParticleSystem smokeParticles;
    public ParticleSystem drinkParticles;
    public ParticleSystem collisionParticles;
    private ParticleSystem.EmissionModule drinkEmission;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip dropSound, breakSound;
    bool allowedPlaySound;
    public float maxTimeBetweenDropSound;
    [Header("destruction properties")]
    public bool destroyAfterTime;
    public bool breakAble;
    public float collisionThreshold;
    public float destroyAfter;
    public float breakThreshold = 0.5f;
    [Header("pouring properties")]
    //Only matters for drinkable objects, consider making some ScriptableObject for drinkable objects
    public bool drinkable; // If true, the object can pour out a drink
    public float drinkMinRotation = 120f; // Angle below which emission is 0
    public float drinkMaxRotation = 180f; // Angle at which emission is max
    public float maxDrinkEmissionRate = 50f;    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float angle;

    [Header("Spawnable Objects")]
    public bool Spawnable;
    public BoxCollider[] OGGrabColliders; // Colliders that are used to grab the object from spawner
    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();
        allowedPlaySound = true;
        if (destroyAfterTime)
        {
            Invoke("DestroyGameObject", destroyAfter);
        }
        if (drinkParticles != null)
        {
            drinkEmission = drinkParticles.emission;
        }
        SpawnableObjectTransition(Spawnable);
    }
    public void FixedUpdate()
    {
        if (drinkable) Drink();


    }
    /// <summary>
    /// Checks the angle of the object to determine if it is pouring a drink
    /// If the angle is less than drinkMinRotation, the emission rate is set to 0, otherwise is set based on the angle.
    /// Uses collision particles (slower sub particles that do that actual collisions, hidden from player via low alpha)
    /// </summary>
    public void Drink()
    {
        if (drinkParticles != null)
        {
            angle = Vector3.Angle(transform.up, Vector3.up);

            var emission = drinkParticles.emission; // Get a fresh copy each time

            //Add collisionParticles emission handling
            ParticleSystem.EmissionModule collisionEmission = default;
            if (collisionParticles != null)
                collisionEmission = collisionParticles.emission;

            //stop pouring
            if (angle < 90f)
            {
                emission.rateOverTime = 0f;
                //drinkParticles.Stop();

                //Stop collisionParticles emission
                if (collisionParticles != null)
                    collisionEmission.rateOverTime = 0f;

            }
            //Start pouring
            else
            {
                float t = Mathf.InverseLerp(90f, 180f, angle);
                emission.rateOverTime = t * maxDrinkEmissionRate;
                //drinkParticles.Play();

                // Set collisionParticles emission (slower, more direct)
                if (collisionParticles != null)
                    collisionEmission.rateOverTime = t * (maxDrinkEmissionRate * 0.5f); // Example: half the rate
            }
        }
    }

    /// <summary>
    /// Handles collisions, decides if the object should break or play a sound based on the collision force
    /// If the object breaks, it plays the break sound and destroys the object after the break particles
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (!allowedPlaySound) return;
        //Debug.Log(gameObject.name + " dropped with " + rb.GetRelativePointVelocity(transform.position).magnitude + " magnitude");

        if (breakAble && rb.GetRelativePointVelocity(transform.position).magnitude > breakThreshold)
        {
            Break();
        }
        else if (collision.relativeVelocity.magnitude > collisionThreshold)
        {
            if (dropSound != null && allowedPlaySound)
            {
                PlaySound(dropSound);

            }
        }
        else if (rb.GetRelativePointVelocity(transform.position).magnitude > collisionThreshold)
        {
            if (dropSound != null && allowedPlaySound)
            {
                PlaySound(dropSound);

            }
        }
    }
    /// <summary>
    /// Plays a sound if allowed, resets the allowedPlaySound flag after a delay
    /// This is to prevent spamming the sound when the object is dropped multiple times in quick succession
    /// </summary>
    /// <param name="sound"></param>
    void PlaySound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
        allowedPlaySound = false;
        Invoke("ResetAllowedPlaySound", maxTimeBetweenDropSound);
    }
    /// <summary>
    /// Resets the allowedPlaySound flag to true after a delays
    /// </summary>
    void ResetAllowedPlaySound()
    {
        allowedPlaySound = true;
    }
    void DestroyGameObject()
    {
        //Debug.Log(gameObject.name + " destroyed by Cinteracrable");
        Destroy(gameObject);
    }
    /// <summary>
    /// Does all actions required to break the object
    /// </summary>
    public void Break()
    {
        breakAble = false; // Prevent further breaks
        Debug.Log(gameObject.name + " broken by Cinteracrable");
        if (breakSound != null)
        {
            PlaySound(breakSound);
        }
        mesh.SetActive(false);
        gameObject.GetComponent<Collider>().enabled = false;
        //Destroy(gameObject);
        gameObject.GetComponent<XRGrabInteractable>().enabled = false;
        if (breakParticles != null) breakParticles.Play();

        Invoke("DestroyGameObject", breakParticles.main.duration);

    }
    /// <summary>
    /// equips interactable and begins all particle systems
    /// </summary>
    public void Equip()
    {
        if (smokeParticles != null)
        {
            smokeParticles.Play();
        }
        // if(gameObject.GetComponent<Collider>() != null)  
        //     gameObject.GetComponent<Collider>().enabled = false; 
    }
    /// <summary>
    /// Unequips interactable and stops all particle systems
    /// </summary>
    public void Unequip()
    {

        if (smokeParticles != null)
        {
            smokeParticles.Stop();
        }
        // if(gameObject.GetComponent<Collider>() != null)  
        //     gameObject.GetComponent<Collider>().enabled = true;
    }
    public void SpawnableObjectTransition(bool waitingToBeGrabbed)
    {
        foreach (BoxCollider collider in OGGrabColliders)
        {
            collider.enabled = waitingToBeGrabbed;
        }
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        //rb.mass = waitingToBeGrabbed ? 1 : int.MaxValue; // Lighter when waiting to be grabbed
        rb.useGravity = !waitingToBeGrabbed; // Disable gravity when waiting to be grabbed
        rb.isKinematic = waitingToBeGrabbed; // Make kinematic when waiting to be grabbed

    }
}
