using UnityEngine;

public class DrinkParticle : MonoBehaviour
{
    public ParticleSystem drinkParticles;
    /// <summary>
    /// Handles the particle collision with the cup.
    /// </summary>
    /// <param name="other"></param>
    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Particle collided with: " + other.name);
        other.GetComponent<Cup>()?.FillCup(drinkParticles.main.startColor.color);
    }
}
