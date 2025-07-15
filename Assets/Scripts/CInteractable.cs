using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CInteractable : MonoBehaviour
{
    public GameObject mesh;
    public ParticleSystem breakParticles, smokeParticles, drinkParticles;
    Rigidbody rb;
    AudioSource audioSource;
    public AudioClip dropSound, breakSound;
    public float collisionThreshold;
    bool allowedPlaySound;
    public float maxTimeBetweenDropSound;
    public bool destroyAfterTime, breakAble, drinkable;
    public float destroyAfter;
    public float breakThreshold = 0.5f;
    public float drinkMinRotation = 120f; // Angle below which emission is 0
    public float drinkMaxRotation = 180f; // Angle at which emission is max
    public float maxDrinkEmissionRate = 50f;    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float angle;
    private ParticleSystem.EmissionModule drinkEmission;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();
        allowedPlaySound = true;
        //breakParticles = gameObject.GetComponent<ParticleSystem>();
        if (destroyAfterTime)
        {
            Invoke("DestroyGameObject", destroyAfter);
        }
        if (drinkParticles != null)
        {
            drinkEmission = drinkParticles.emission;
        }
    }
    public void FixedUpdate()
    {
        if (drinkable)
            Drink();
        
    }
    public void Drink()
    {
        if (drinkParticles != null)
        {
            angle = Vector3.Angle(transform.up, Vector3.up);

            var emission = drinkParticles.emission; // Get a fresh copy each time

            if (angle < 90f)
            {
                emission.rateOverTime = 0f;
                //drinkParticles.Stop();
            }
            else
            {
            float t = Mathf.InverseLerp(90f, 180f, angle);
                emission.rateOverTime = t * maxDrinkEmissionRate;
                //drinkParticles.Play();
            }
        }
    }
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
    void PlaySound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
        allowedPlaySound = false;
        Invoke("ResetAllowedPlaySound", maxTimeBetweenDropSound);
    }
    void ResetAllowedPlaySound()
    {
        allowedPlaySound = true;
    }
    void DestroyGameObject()
    {
        //Debug.Log(gameObject.name + " destroyed by Cinteracrable");
        Destroy(gameObject);
    }
    //Does all actions required to break the object
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
        if(breakParticles != null) breakParticles.Play();
        
        Invoke("DestroyGameObject", breakParticles.main.duration);

    }
    public void Equip()
    {
        if (smokeParticles != null)
        {
            smokeParticles.Play();
        } 
        // if(gameObject.GetComponent<Collider>() != null)  
        //     gameObject.GetComponent<Collider>().enabled = false; 
    }
    public void Unequip()
    {

        if (smokeParticles != null)
        {
            smokeParticles.Stop();
        } 
        // if(gameObject.GetComponent<Collider>() != null)  
        //     gameObject.GetComponent<Collider>().enabled = true;
    }
}
