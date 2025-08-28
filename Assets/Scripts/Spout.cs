using UnityEngine;

public class Spout : MonoBehaviour
{
    public HingeJoint lever;
    public ParticleSystem drinkParticles, collisionParticles;
    public float angle, maxPourAngle;
    public float checkDelay;
    public bool canPour;
    public bool pouring;
    public AudioSource audioSource;
    public AudioClip pourSound;
    //public AudioClip loopingPourSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        DrinkCoroutine();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PourDrink();
    }
    
    private System.Collections.IEnumerator DrinkCoroutine()
    {
        while (canPour)
        {
            if (drinkParticles != null)
            {
                angle = lever.angle;
                if (angle < maxPourAngle)
                {
                    drinkParticles.Play();
                    collisionParticles.Play();
                }
                else
                {
                    drinkParticles.Stop();
                    collisionParticles.Stop();
                }
            }
            Debug.Log("Checking drink particles angle: " + angle + " with minPourAngle: " + maxPourAngle + " - " + (angle < maxPourAngle ? "Pouring" : "Not Pouring"));
            yield return new WaitForSeconds(checkDelay);
        }
    }
        
    public void PourDrink()
    {
        if (drinkParticles != null)
        {
            var drinkEmission = drinkParticles.emission;
            var collisionEmission = collisionParticles.emission;
            angle = lever.angle;
            bool isPouring = angle < maxPourAngle;

            if (isPouring)
            {
                drinkEmission.rateOverTime = 20f;
                collisionEmission.rateOverTime = 5f;

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = pourSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else
            {
                drinkEmission.rateOverTime = 0f;
                collisionEmission.rateOverTime = 0f;

                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
        //Debug.Log("Checking drink particles angle: " + angle + " with minPourAngle: " + maxPourAngle + " - " + (angle > maxPourAngle ? "Pouring" : "Not Pouring"));
    }
}
