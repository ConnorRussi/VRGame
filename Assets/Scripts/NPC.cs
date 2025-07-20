using System;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using JetBrains.Annotations;
using UnityEditor.Build.Content;

public class NPC : MonoBehaviour
{
    //private GameObject gameManger;
    public GameManager gameManager;
    public bool isDead;
    public bool isHostile;
    public float currhealth;
    public NPCDefault defaults;
    public DrinkDataBase drinkDataBase;
    public GameObject coaster;
    public float angerLevel, angerCap;
    public Coaster currentCoaster;
    public TextMeshPro orderText;
    public struct Order
    {
        public DrinkDataBase.Drink drink;
        public bool ice, cherry, lemon;
    }
    public Order myOrder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
            return;
        }
        drinkDataBase = GameObject.Find("GameManager").GetComponent<DrinkDataBase>();
        angerLevel = 0.0f;
        angerCap = Random.Range(defaults.minAngerCap, defaults.maxAngerCap);
        currhealth = defaults.maxHealth;
        CreateOrder();
        //chose an open coaster for npc to walk to and claim coaster
        gameManager.assignCoaster(this);
        if (currentCoaster == null)
        {
            Debug.LogError("No coaster assigned to NPC: " + gameObject.name);
            return;
        }
        //Set path for npc to walk (enter and exit path)

    }
    public void FixedUpdate()
    {
        angerLevel += Time.deltaTime * defaults.angerIncrement;
        if (angerLevel >= angerCap && !isHostile)
        {
            isHostile = true;
            Debug.Log("NPC has become hostile due to high anger level.");
        }
    }
    void CreateOrder()
    {
        // Create a random order for the NPC
        int drinkIndex = Random.Range(0, drinkDataBase.drinkList.Count);
        myOrder = new Order();
        myOrder.drink = drinkDataBase.drinkList[drinkIndex];
        myOrder.ice = myOrder.drink.iceAllowed && Random.Range(0, 2) == 0; // 50% chance to have ice
        myOrder.cherry = myOrder.drink.cherryAllowed && Random.Range(0, 2) == 0; // 50% chance to have cherry
        myOrder.lemon = myOrder.drink.lemonAllowed && Random.Range(0, 2) == 0; // 50% chance to have lemon
        orderText.text = $"Order: {myOrder.drink.liquidName} in a {myOrder.drink.cupType} cup with " +
                         $"{(myOrder.ice ? "ice" : "no ice")}, " +
                         $"{(myOrder.cherry ? "cherry" : "no cherry")}, " +
                         $"{(myOrder.lemon ? "lemon" : "no lemon")}.";
    }
   
    public void CompareDrinkToOrder(Cup cup, String tag, GameObject cupObject)
    {
        // Compare the order with the NPC's order
        // This is a placeholder for actual comparison logic
        Debug.Log("Comparing order with NPC's order.");
        bool iceMatch = cup.ice == myOrder.ice;
        bool mintMatch = cup.mint == myOrder.cherry;
        bool lemonMatch = cup.lemon == myOrder.lemon;
        bool tagMatch = tag == myOrder.drink.cupType.ToString();
        bool colorMatch = ColorsAreClose(cup.drinkRenderer.material.color, myOrder.drink.color);

        if (iceMatch && mintMatch && lemonMatch && tagMatch && colorMatch)
        {
            Debug.Log("The drink matches the NPC's order.");
            // Logic for when the drink matches the order
            cupObject.GetComponent<CInteractable>().Break();
            coaster.GetComponent<Coaster>().releaseCoaster();
            //set to leave and have leave on path
        }
        else
        {
            Debug.Log("The drink does not match the NPC's order.");
            if (!iceMatch) Debug.Log($"Mismatch: ice (Cup: {cup.ice}, Order: {myOrder.ice})");
            if (!mintMatch) Debug.Log($"Mismatch: mint/cherry (Cup: {cup.mint}, Order: {myOrder.cherry})");
            if (!lemonMatch) Debug.Log($"Mismatch: lemon (Cup: {cup.lemon}, Order: {myOrder.lemon})");
            if (!tagMatch) Debug.Log($"Mismatch: cup type/tag (Cup: {tag}, Order: {myOrder.drink.cupType})");
            if (!colorMatch) Debug.Log($"Mismatch: color (Cup: {cup.drinkRenderer.material.color}, Order: {myOrder.drink.color})");
            Debug.Log("The drink does not match the NPC's order.");
            // Logic for when the drink does not match the order
            angerLevel += defaults.wrongDrinkIncrement;

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
