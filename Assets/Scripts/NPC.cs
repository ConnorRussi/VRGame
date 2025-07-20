using System;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using JetBrains.Annotations;

public class NPC : MonoBehaviour
{
    public bool isDead;
    public bool isHostile;
    public float currhealth;
    public NPCDefault defaults;

    public float angerLevel;

    public TextMeshPro orderText;
    public struct Order
    {
        public NPCDefault.drink drink;
        public string cupType;
        public Color color;
        public bool ice, cherry, lemon;
    }
    public Order myOrder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        angerLevel = 0.0f;
        defaults.angerCap = Random.Range(defaults.minAngerCap, defaults.maxAngerCap);
        currhealth = defaults.maxHealth;
        CreateOrder();
        //chose an open coaster for npc to walk to and claim coaster
        //Set path for npc to walk (enter and exit path)

    }
    public void FixedUpdate()
    {
        angerLevel += Time.deltaTime * defaults.angerIncrement;
        if (angerLevel >= defaults.angerCap && !isHostile)
        {
            isHostile = true;
            Debug.Log("NPC has become hostile due to high anger level.");
        }
    }
    void CreateOrder()
    {
        // Create a random order for the NPC
        int drinkIndex = Random.Range(0, defaults.drinkList.Count);
        myOrder = new Order();
        myOrder.drink = defaults.drinkList[drinkIndex];
        myOrder.cupType = myOrder.drink.cupType;
        myOrder.color = myOrder.drink.color;
        myOrder.ice = myOrder.drink.iceAllowed && Random.Range(0, 2) == 0; // 50% chance to have ice
        myOrder.cherry = myOrder.drink.cherryAllowed && Random.Range(0, 2) == 0; // 50% chance to have cherry
        myOrder.lemon = myOrder.drink.lemonAllowed && Random.Range(0, 2) == 0; // 50% chance to have lemon
        orderText.text = $"Order: {myOrder.drink.liquidName} in a {myOrder.cupType} cup with " +
                         $"{(myOrder.ice ? "ice" : "no ice")}, " +
                         $"{(myOrder.cherry ? "cherry" : "no cherry")}, " +
                         $"{(myOrder.lemon ? "lemon" : "no lemon")}.";
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
           cup.mint == myOrder.cherry &&
           cup.lemon == myOrder.lemon &&
           tag == myOrder.cupType && ColorsAreClose(cup.drinkRenderer.material.color, myOrder.color))
        {
            Debug.Log("The drink matches the NPC's order.");
            // Logic for when the drink matches the order
        }
        else
        {
           
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
        angerLevel = defaults.angerCap;
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
