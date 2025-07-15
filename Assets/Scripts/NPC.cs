using System;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPC : MonoBehaviour
{
    public bool thief;
    public bool isDead;
    public bool isHostile;
    public float currhealth;
    public float maxHealth;
    public float angerIncrement;
    public float angerLevel, angerCap;
    public float maxAngerCap, minAngerCap;
    public struct Order
    {
        public Color color;
        public string cupType;
        public bool ice, mint, lemon;
    }
    public Order myOrder;
    public Color orderColor;
    public string orderCupType;
    public bool orderIce, orderMint, orderLemon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        angerLevel = 0.0f;
        angerCap = Random.Range(minAngerCap, maxAngerCap);
        currhealth = maxHealth;
        myOrder.color = orderColor;
        myOrder.cupType = orderCupType;
        myOrder.ice = orderIce;
        myOrder.mint = orderMint;
        myOrder.lemon = orderLemon;
    }
    public void FixedUpdate()
    {
        angerLevel += Time.deltaTime * angerIncrement;
        if (angerLevel >= angerCap && !isHostile)
        {
            isHostile = true;
            Debug.Log("NPC has become hostile due to high anger level.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Cup cup = other.gameObject.GetComponent<Cup>();
        if (cup != null)
        {
            CompareDrinkToOrder(cup, other.gameObject.tag);
        }

    }
    void CompareDrinkToOrder(Cup cup, String tag)
    {
        // Compare the order with the NPC's order
        // This is a placeholder for actual comparison logic
        Debug.Log("Comparing order with NPC's order.");
        if (cup.ice == myOrder.ice &&
           cup.mint == myOrder.mint &&
           cup.lemon == myOrder.lemon &&
           tag == myOrder.cupType && ColorsAreClose(cup.drinkRenderer.material.color, myOrder.color))
        {
            Debug.Log("The drink matches the NPC's order.");
            Debug.Log(myOrder.color);
            Debug.Log(cup.drinkRenderer.material.color);
            // Logic for when the drink matches the order
        }
        else
        {
            Debug.Log(orderColor + " " + orderCupType + " " + orderIce + " " + orderMint + " " + orderLemon);
            Debug.Log(cup.drinkRenderer.material.color + " " + tag + " " + cup.ice + " " + cup.mint + " " + cup.lemon);
            Debug.Log((cup.ice == myOrder.ice) + " " + (cup.mint == myOrder.mint) + " " + (cup.lemon == myOrder.lemon) + " " + ColorsAreClose(cup.drinkRenderer.material.color, myOrder.color) + " " + (tag == myOrder.cupType));
            Debug.Log("The drink does not match the NPC's order.");
            // Logic for when the drink does not match the order
        }
    }
    public static bool ColorsAreClose(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
            Mathf.Abs(a.g - b.g) < tolerance &&
            Mathf.Abs(a.b - b.b) < tolerance; 
            //Mathf.Abs(a.a - b.a) < tolerance;
    }
    public void bulletHit(float damage)
    {
        angerLevel = angerCap;
        isHostile = true;
        if (isDead) return;
        currhealth -= damage;
        if (currhealth <= 0)
        {
            Die();
            // Logic for when the NPC dies
        }
        else
        {
            Debug.Log("NPC took damage, current health: " + currhealth);
            // Logic for when the NPC takes damage but is still alive
        }
    }
    void Die()
    {
        isDead = true;
        Debug.Log("NPC has died.");
        gameObject.SetActive(false); // Deactivate the NPC GameObject
        // Logic for when the NPC dies, such as playing an animation or removing the NPC from the game
    }
}
