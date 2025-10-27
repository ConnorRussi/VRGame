using System;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using JetBrains.Annotations;
using UnityEditor.Build.Content;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Analytics;

public class NPC : MonoBehaviour
{
    public GameManager gameManager;
    public NPCDefault defaults;
    private DrinkDataBase drinkDataBase;
    public GameObject player;
    public bool isDead;
    public int currhealth;
    public int bulletDamage; // Damage taken from a bullet
    public GameObject CoinPrefab;
    
    [Header("Anger Properties")]
    public float angerLevel;
    public float angerCap;
    public bool isHostile;
    public bool hostileStateSet;
    public float angerIncrement;
    [Header("Order Properties")]
    public Slider angerSlider;
    public struct Order
    {
        public DrinkDataBase.Drink drink;
        public bool ice, cherry, lemon;
    }
    public Order myOrder;
    public Coaster currentCoaster;
    public GameObject lemonMark, CherryMark, IceMark;
    public Image drinkImage, cupImage;
    public GameObject orderHolder;
    


    [Header("Revolver Properties")]
    public GameObject revolverPrefab;
    public GameObject revolver, spawnPoint, holdPoint;
    public RevolverShoot revolverShootSC;
    //these next two can be moved to scriptable object
    public float minShootInterval; // Minimum time between shots
    public float maxShootInterval; // Maximum time between
    public float accuracy; // Lower = more accurate
    public float smoothSpeed; //speed of rotation towards player

    [Header("NPC Pathfinding Properties")]
    private NPCPathFinding npcPathFinding;
    public GameObject coaster;
    public bool isPathing = false;
    public bool acceptedDrink = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
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

        coaster = gameManager.assignCoaster(gameObject);
        if (currentCoaster == null)
        {
            Debug.LogError("No coaster assigned to NPC: " + gameObject.name);
            return;
        }
        //Set path for npc to walk (enter and exit path)
        angerSlider.value = 1.0f; // Set the anger slider to max value
        npcPathFinding = GetComponent<NPCPathFinding>();
        angerIncrement *= gameManager.difficultyScale;

    }
    /// <summary>
    /// updates angerlevel and checks if npc is hostile
    /// </summary>
    public System.Collections.IEnumerator UpdateAngerLevel()
    {
        while (!isPathing && !isDead)
        {
            yield return new WaitForFixedUpdate();
            if (!hostileStateSet)
            {
                angerLevel += Time.deltaTime * angerIncrement;
                if (angerCap <= 0) Debug.LogError("Anger cap is less than or equal to zero, this should not happen.");
                angerSlider.value = 1 - Mathf.Clamp01(angerLevel / angerCap); // Update the slider value based on anger level
                if (angerLevel >= angerCap && !isHostile)
                {
                    isHostile = true;
                    EnterHostileState();
                    Debug.Log("NPC has become hostile due to high anger level.");
                }
                continue; // Exit if not hostile
            }
            //do hostile state things
            Vector3 aimDirection = GetDirection();
            if (revolver != null)
            {
                // Smoothly rotate the revolver towards the aim direction
                Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
                revolver.transform.rotation = Quaternion.Slerp(
                    revolver.transform.rotation,
                    targetRotation,
                    Time.deltaTime * smoothSpeed
                );
            }
        }

    }
    /// <summary>
    /// Creates a random order for the NPC 
    /// this includes a drink, ice, cherry, and lemon
    /// The order is displayed in the orderText UI element.
    /// NEEDS TO BE CHANGEED TO USE RANDOM DRINKS
    /// </summary>
    void CreateOrder()
    {
        // Create a random order for the NPC
        //the +1 is for the 0, 0  case
        int drinkIndex = Random.Range(0, drinkDataBase.drinkList.Count);
        //int drinkIndex = 0;
        myOrder = new Order();
        myOrder.drink = drinkDataBase.drinkList[drinkIndex];
       // Debug.Log("drink base index: " + drinkIndex);
        myOrder.ice = myOrder.drink.iceAllowed && Random.Range(0, 2) == 0; // 50% chance to have ice
        myOrder.cherry = myOrder.drink.cherryAllowed && Random.Range(0, 2) == 0; // 50% chance to have cherry
        myOrder.lemon = myOrder.drink.lemonAllowed && Random.Range(0, 2) == 0; // 50% chance to have lemon
                                                                               // orderText.text = $"Order: {myOrder.drink.liquidName} in a {myOrder.drink.cupType} cup with " +
                                                                               //                  $"{(myOrder.ice ? "ice" : "no ice")}, " +
                                                                               //                  $"{(myOrder.cherry ? "cherry" : "no cherry")}, " +
                                                                               //                  $"{(myOrder.lemon ? "lemon" : "no lemon")}.";
        drinkImage.sprite = myOrder.drink.bottleSprite;
        cupImage.sprite = myOrder.drink.cupSprite;
        IceMark.SetActive(myOrder.ice);
        CherryMark.SetActive(myOrder.cherry);
        lemonMark.SetActive(myOrder.lemon);

    }

    /// <summary>
    /// Compares the drink in the cup to the NPC's order, provides logic for match or not
    /// </summary>
    /// <param name="cup"></param>
    /// <param name="tag"></param>
    /// <param name="cupObject"></param>
    public void CompareDrinkToOrder(Cup cup, String tag, GameObject cupObject)
    {
        // Compare the order with the NPC's order
        // This is a placeholder for actual comparison logic
        Debug.Log("Comparing order with NPC's order.");
        bool iceMatch = cup.ice == myOrder.ice;
        bool cherryMatch = cup.cherry == myOrder.cherry;
        bool lemonMatch = cup.lemon == myOrder.lemon;
        bool tagMatch = tag == myOrder.drink.cupType.ToString();
        bool colorMatch = ColorsAreClose(cup.drinkRenderer.material.color, myOrder.drink.color);
        bool filledEnough = cup.currDrinkHeight >= drinkDataBase.fillThreshold; // Check if the cup is filled enough
        if (iceMatch && cherryMatch && lemonMatch && tagMatch && colorMatch && filledEnough)
        {
            AcceptDrink(cupObject, cup);

        }
        else
        {
            Debug.Log("The drink does not match the NPC's order.");
            if (!iceMatch) Debug.Log($"Mismatch: ice (Cup: {cup.ice}, Order: {myOrder.ice})");
            if (!cherryMatch) Debug.Log($"Mismatch: cherry (Cup: {cup.cherry}, Order: {myOrder.cherry})");
            if (!lemonMatch) Debug.Log($"Mismatch: lemon (Cup: {cup.lemon}, Order: {myOrder.lemon})");
            if (!tagMatch) Debug.Log($"Mismatch: cup type/tag (Cup: {tag}, Order: {myOrder.drink.cupType})");
            if (!colorMatch) Debug.Log($"Mismatch: color (Cup: {cup.drinkRenderer.material.color}, Order: {myOrder.drink.color})");
            if(!filledEnough) Debug.Log($"Mismatch: filled enough (Cup height: {cup.currDrinkHeight}, Required: {drinkDataBase.fillThreshold})");
            Debug.Log("The drink does not match the NPC's order.");
            // Logic for when the drink does not match the order
            angerLevel += defaults.wrongDrinkIncrement;

        }
    }
    void AcceptDrink(GameObject cupObject, Cup cup) {
        if (!acceptedDrink)
        {
            acceptedDrink = true;
            Debug.Log("The drink matches the NPC's order.");
            // Logic for when the drink matches the order
            cupObject.GetComponent<CInteractable>().Break();
            //give coins, set up for different coin amounts later EX: diff colors for diff values
            GameObject currCoin = Instantiate(CoinPrefab, coaster.GetComponent<Coaster>().spawnPoint.transform.position, Quaternion.identity);
            currCoin.GetComponent<CInteractable>().coinValue = (int)Math.Round(myOrder.drink.basePrice * (angerLevel / angerCap)) + defaults.iceValue + defaults.cherryValue + defaults.lemonValue;
            //set to leave and have leave on path
            isPathing = true;
            isHostile = false;
            orderHolder.SetActive(false); // Hide the order holder UI

            StartCoroutine(npcPathFinding.PathFindOut());
        }
    }
    /// <summary>
    /// Checks if two colors are close enough to be considered a match.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    public static bool ColorsAreClose(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
            Mathf.Abs(a.g - b.g) < tolerance &&
            Mathf.Abs(a.b - b.b) < tolerance;
        //Mathf.Abs(a.a - b.a) < tolerance;
    }
    public void bulletHit(int damage)
    {
        angerLevel = angerCap;
        if (isDead) Debug.LogWarning("NPC is already dead, bullet hit should not be processed.");
        currhealth = (currhealth - damage);
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
    void EnterHostileState()
    {
        //spawn revolver and strart shooting at player, 
        revolver = Instantiate(revolverPrefab, spawnPoint.transform.position, Quaternion.identity);
        revolverShootSC = revolver.GetComponent<RevolverShoot>();
        if (revolverShootSC != null)
        {
            revolver.GetComponent<Revolver>().SetOwner(Revolver.GunOwner.NPC);
        }

        // Start coroutine to animate revolver rising and aiming
        StartCoroutine(RaiseAndAimRevolver());
    }

    private System.Collections.IEnumerator RaiseAndAimRevolver()
    {
        // Start at spawnPoint, end at holdPoint
        Vector3 start = spawnPoint.transform.position;
        Vector3 end = holdPoint.transform.position;
        float duration = 1.0f; // seconds to rise
        float elapsed = 0f;

        // Point at the ground while rising
        if (revolver != null)
            revolver.transform.rotation = Quaternion.LookRotation(Vector3.down);

        // Move from spawnPoint to holdPoint
        while (elapsed < duration)
        {
            if (revolver != null)
            {
                revolver.transform.position = Vector3.Lerp(start, end, elapsed / duration);
                // Keep pointing at the ground
                revolver.transform.rotation = Quaternion.LookRotation(Vector3.down);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (revolver != null)
        {
            revolver.transform.position = end;
            // Now rotate to point at the player using GetDirection
            Quaternion startRot = revolver.transform.rotation;
            Vector3 aimDirection = GetDirection();
            Quaternion endRot = Quaternion.LookRotation(aimDirection);
            float rotateDuration = 0.5f; // seconds for smooth rotation
            float rotateElapsed = 0f;
            while (rotateElapsed < rotateDuration)
            {
                if (revolver != null)
                {
                    revolver.transform.rotation = Quaternion.Slerp(
                        startRot,
                        endRot,
                        rotateElapsed / rotateDuration
                    );
                }
                rotateElapsed += Time.deltaTime;
                yield return null;
            }
            // Ensure final rotation is exact
            if (revolver != null)
                revolver.transform.rotation = endRot;
        }
        hostileStateSet = true; // Set the hostile state to true after aiming
        StartCoroutine(RevolverAimRoutine());

        // Start shooting at the player
        StartCoroutine(RevolverShootRoutine());
    }
    private IEnumerator RevolverShootRoutine()
    {
        while (revolver != null && isHostile && !isDead)
        {
            ShootAtPlayer();
            // Wait for a random time between minShootInterval and maxShootInterval
            yield return new WaitForSeconds(Random.Range(minShootInterval, maxShootInterval));
        }
    }
    Vector3 GetDirection()
    {
        Vector3 targetPos = player.transform.position;
        Vector3 toTarget = (targetPos - transform.position).normalized;

        // Create a random offset in local space (right/up, not forward)
        Vector3 right = Vector3.Cross(toTarget, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, toTarget).normalized;

        float horizontalSpread = Random.Range(-accuracy, accuracy);
        float verticalSpread = Random.Range(-accuracy, accuracy);

        Vector3 offset = right * horizontalSpread + up * verticalSpread;

        Vector3 aimPoint = targetPos + offset;
        Vector3 direction = (aimPoint - transform.position).normalized;
        return direction;
    }
    void ShootAtPlayer()
    {
        // Get the direction to aim
        //Vector3 shootDirection = GetDirection();

        // Point the revolver's barrel (transform.forward) toward the target direction
        // if (revolverShootSC != null && revolverShootSC.transform != null)
        // {
        //     revolverShootSC.transform.forward = shootDirection;
        // }

        // Fire the gun as NPC
        revolverShootSC.FireGun(Revolver.GunOwner.NPC);
    }
    private System.Collections.IEnumerator RevolverAimRoutine(float aimUpdateInterval = 0.2f)
    {
        while (revolver != null && isHostile && !isDead)
        {
            Vector3 aimDirection = GetDirection();
            if (revolver != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
                revolver.transform.rotation = Quaternion.Slerp(
                    revolver.transform.rotation,
                    targetRotation,
                    Time.deltaTime * smoothSpeed
                );
            }
            yield return new WaitForSeconds(aimUpdateInterval);
        }
    }

    public void Die()
    {
        //Debug.Log(gameObject.name + " NPC has died.");
        //forces wait until released before death
        if (coaster != null)
        {
            bool released = coaster.GetComponent<Coaster>().releaseCoaster();

            if (!released)
            {
                Debug.LogWarning("Failed to release coaster on NPC death: " + gameObject.name);
            }
        }
        //NOT REALEASING NAV POINTS BEFORE DEATH
        npcPathFinding.ForceReleaseNavPoints();
        
        isDead = true;
        if (revolver != null)
        {
            Destroy(revolver); // Destroy the revolver if it exists
        }
        gameManager.npcs.Remove(gameObject); // Remove this NPC from the GameManager's list
        Destroy(gameObject); // Destroy the NPC GameObject

    }
   

}
