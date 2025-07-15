using UnityEngine;

public class DrinkParticle : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Particle collided with: " + other.name);
        other.GetComponent<Cup>()?.FillCup(gameObject.GetComponent<ParticleSystem>().main.startColor.color);
    }
}
