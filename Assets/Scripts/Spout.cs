using UnityEngine;

public class Spout : MonoBehaviour
{
    public HingeJoint lever;
    public ParticleSystem drinkParticles, collisionParticles;
    public float angle, maxPourAngle;
    public float checkDelay;
    public bool canPour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            if (angle < maxPourAngle)
            {
                Debug.Log("Pouring drink at angle: " + angle);
              
                //drinkParticles.Play();
                drinkEmission.rateOverTime = 20f;
                //collisionParticles.Play();
                collisionEmission.rateOverTime = 2f;
            
            }
            else
            {
                Debug.Log("not Pouring drink at angle: " + angle);
                
                //drinkParticles.Stop();
                drinkEmission.rateOverTime = 0f;
                //collisionParticles.Stop();
                collisionEmission.rateOverTime = 0f;
                
            }
        }
        //Debug.Log("Checking drink particles angle: " + angle + " with minPourAngle: " + maxPourAngle + " - " + (angle > maxPourAngle ? "Pouring" : "Not Pouring"));
    }
}
