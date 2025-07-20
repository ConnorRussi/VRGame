using UnityEngine;

public class DrinkParticle : MonoBehaviour
{
    public ParticleSystem drinkParticles;
    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Particle collided with: " + other.name);
        other.GetComponent<Cup>()?.FillCup(drinkParticles.main.startColor.color);
    }
}
