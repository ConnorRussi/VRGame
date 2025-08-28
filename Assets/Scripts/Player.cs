using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    public int maxHealth = 3; // Maximum health for the player
    public int bulletDamage = 1; // Damage taken from a bullet
    public Animator[] healthNodes; // Animators for different health 
    public bool hurt, heal;
    public bool debug = false;
    public void Start()
    {
        health = maxHealth;
    }
    void Update()
    {
        if (hurt)
        {
            TakeDamage(bulletDamage);
            hurt = false; // Reset hurt state after taking damage
        }
        if(heal)
        {
            Heal(1); // Heal by 1 for demonstration
            heal = false; // Reset heal state after healing
        }
        
            
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthNodes[health].SetTrigger("TakeDamage");
        if(debug)Debug.Log("Player took damage, current health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }
    public void Heal(int amount)
    {
        healthNodes[health].SetTrigger("Heal");

        health += amount;
       
        if (health > healthNodes.Length - 1)
        {
            health = healthNodes.Length - 1; // Cap health to the maximum number of nodes
        }
        
        if(debug)Debug.Log("Player healed, current health: " + health);
    }
    public void Die()
    {
        Debug.Log("Player has died.");
        // Handle player death logic here, such as playing an animation or reloading the scene
        // For example:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
