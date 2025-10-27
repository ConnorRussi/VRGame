using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using Unity.XR.CoreUtils;
using System.Security.Cryptography;
using UnityEngine.Rendering;

public class Store : MonoBehaviour, ICollisionReciever
{
    [Header("Store Setup")]
    public StoreItemSO[] topItems; // Assign in inspector
    public StoreItemSO[] bottomItems; // Assign in inspector
    public Transform[] topSockets;   // Assign in inspector, one per item
    public Transform[] bottomSockets; // Assign in inspector, one per item
    public GameObject lockObject;    // The lock visual
    public GameObject hitMeArt;      // Art to show when locked
    public ParticleSystem explosionParticles;
    public AudioSource audioSource;
    public AudioClip explosionSound;
    public TMP_Text[] costLabels;    // top then bottom labels, assign in inspector
    public Animator animator;

    [Header("Lock Settings")]
    public float breakForce = 50f;
    private float currentForce = 0f;
    private bool isLocked = true;

    [Header("Register")]
    public Register register; // Reference to your player script with currency

    private List<GameObject> spawnedItems = new List<GameObject>();
    private List<StoreSocket> storeSockets = new List<StoreSocket>();
    public List<StoreItemSO> storeItems = new List<StoreItemSO>();
    private struct StoreSocket
    {
        public Transform socket;
        public bool occupied;
        public GameObject costBoardObject;
        public TMP_Text costLabel;
    }

    public bool openLock;
    public float lidSpeed = 1f;
    public float lidOpenAngle;
    public float lidTolerance;
    public GameObject lid;
    public bool debug;
    public bool storeClosed;

    public List<StoreItemGrabbable> purchasableItems;


    /// <summary>
    /// Checks if the lock should be opened (via openLock flag).
    /// </summary>
    public void Update()
    {
        if (openLock)
        {
            PopOpen();
            openLock = false;
        }
    }

    /// <summary>
    /// Initializes sockets, cost labels, and links to the register.
    /// </summary>
    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        //fills top sockets first then bottom sockets
        for (int i = 0; i < costLabels.Length; i++)
        {
            if (costLabels[i] == null)
            {
                Debug.LogError("Cost label at index " + i + " is not assigned in the inspector.");
            }
            storeSockets.Add(new StoreSocket()
            {
                socket = (i < topSockets.Length) ? topSockets[i] : bottomSockets[i - topSockets.Length],
                occupied = false,
                costLabel = costLabels[i],
                costBoardObject = costLabels[i].transform.parent.gameObject
            });
        }
        for (int i = 0; i < topItems.Length; i++)
        {
            storeItems.Add(topItems[i]);
        }
        for (int i = 0; i < bottomItems.Length; i++)
        {
            storeItems.Add(bottomItems[i]);
        }
        register = GameObject.Find("Register").GetComponent<Register>();
        register.store = this;
    }

    /// <summary>
    /// Spawns items into sockets, sets lock state.
    /// </summary>
    void Start()
    {
        SpawnItems();
        SetLockState(true);
    }

    /// <summary>
    /// Instantiates store items into sockets, sets up grabbable logic and cost labels.
    /// </summary>
    void SpawnItems()
    {
        for (int i = 0; i < storeSockets.Count; i++)
        {
            storeSockets[i].costBoardObject.SetActive(false);
            StoreItemSO itemToSpawn = null;
            int itemIndex = -1;
            if (i < topSockets.Length) //asserts top item
            {
                itemIndex = UnityEngine.Random.Range(0, topItems.Length);
                itemToSpawn = topItems[itemIndex];
            }
            else //must be a bottom item
            {
                int botIndex = UnityEngine.Random.Range(0, bottomItems.Length);
                itemToSpawn = bottomItems[botIndex];
                itemIndex = topItems.Length + botIndex;
            }
            //Spawns items now that we have the item to spawn and where to spawn it
            GameObject item = Instantiate(itemToSpawn.prefab, storeSockets[i].socket.transform.position, storeSockets[i].socket.transform.rotation, storeSockets[i].socket);
            spawnedItems.Add(item);

            var grabbableItem = item.GetComponent<StoreItemGrabbable>();
            grabbableItem.inStore = true;
            grabbableItem.store = this;
            grabbableItem.itemIndex = itemIndex;
            //adds this item to the purchasable list
            purchasableItems.Add(grabbableItem);
            storeSockets[i].costLabel.text = "$" + itemToSpawn.cost.ToString(); //sets label text
            item.GetComponent<Rigidbody>().isKinematic = true;
            storeSockets[i].costBoardObject.SetActive(false); //starts off because box is locked when spawned
        }
        Debug.Log("updating purchaseable at spawn");
        UpdatePurchaseAblesList(); //initial update to make sure nothing is grabbable at start
    }

    /// <summary>
    /// Receives collision info (e.g., from a bullet), applies force to the lock.
    /// </summary>
    void ICollisionReciever.ReceiveCollisionInfo(Collision collision)
    {
        Debug.Log("lock force: " + collision.impulse.magnitude);
        ApplyForceToLock(collision.impulse.magnitude);
    }

    /// <summary>
    /// Sets the lock visual and enables/disables cost boards.
    /// </summary>
    void SetLockState(bool locked)
    {
        isLocked = locked;
        if (lockObject) lockObject.SetActive(locked);
        if (!locked)
        {
            for (int i = 0; i < storeSockets.Count; i++)
            {
                storeSockets[i].costBoardObject.SetActive(true);
            }
        }
        Debug.Log("updating purchaseable items list because lock state changed");
        UpdatePurchaseAblesList(); // updates what can be bought now that store is unlocked

    }

    /// <summary>
    /// Adds force to the lock; if enough, pops open the lock.
    /// </summary>
    public void ApplyForceToLock(float force)
    {
        if (!isLocked) return;
        currentForce += force;
        if (currentForce >= breakForce)
        {
            PopOpen();
        }
    }

    /// <summary>
    /// Plays explosion effects and triggers lid opening animation.
    /// </summary>
    void PopOpen()
    {
        if (explosionParticles) explosionParticles.Play();
        if (audioSource && explosionSound) audioSource.PlayOneShot(explosionSound);
        animator.SetTrigger("OpenBox");
    }

    /// <summary>
    /// Called when lid animation finishes; unlocks store and resets register value.
    /// </summary>
    public void LidFullyOpen()
    {
        SetLockState(false);
        register.UpdateTotalValue(0); //makes sure the value on screen is updated
    }

    /// <summary>
    /// Checks if the player has enough funds to buy an item.
    /// </summary>
    public bool CanAfford(int itemIndex)
    {
        //register not null and have enough funds
        Debug.Log("checking can afford for item index: " + itemIndex + " cost: " + storeItems[itemIndex].cost + " total funds: " + register.GetTotalValue());
        bool canAfford = register && register.GetTotalValue() >= storeItems[itemIndex].cost;
        Debug.Log("can afford: " + canAfford);
        return canAfford;
    }

    /// <summary>
    /// Handles item purchase: charges player, updates UI, detaches item from socket.
    /// </summary>
    public void PurchaseItem(int itemIndex, GameObject item)
    {
        if (debug) Debug.Log("Item purchased: " + storeItems[itemIndex].name);
        purchasableItems.Remove(item.GetComponent<StoreItemGrabbable>()); //removes from purchasable list

        register.UpdateTotalValue(-1 * storeItems[itemIndex].cost); //since being purchased we need to update the value
        register.UpdateRegScreen();
        item.transform.SetParent(null); //detach from socket

        
    }

    /// <summary>
    /// Returns whether the store is locked.
    /// </summary>
    public bool IsLocked()
    {
        return isLocked;
    }

    /// <summary>
    /// Updates which items are purchasable based on funds and lock state.
    /// </summary>
    public void UpdatePurchaseAblesList()
    {
        foreach (StoreItemGrabbable storeItemGrabbable in purchasableItems)
        {
            //checks if the item can be bought currently
            if (IsItemPurchaseable(storeItemGrabbable))
            {
                storeItemGrabbable.UpdateInteractionLayer(true);
                continue;
            }
            storeItemGrabbable.UpdateInteractionLayer(false);
        }
    }

    /// <summary>
    /// Checks if a specific item can be bought.
    /// </summary>
    public bool IsItemPurchaseable(StoreItemGrabbable storeItemGrabbable)
    {
        if (isLocked)
        {
            if (debug) Debug.Log("locked cant buy");
            return false;
        }
        if (!storeItemGrabbable.inStore) //will always be in store unless already purchased
        {
            if (debug) Debug.LogWarning("item you are trying to buy is already purchased");
            return false;
        }
        if (!CanAfford(storeItemGrabbable.itemIndex))
        {
            if (debug) Debug.Log("Not enough funds to purchase this item.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Cleans up the store and disables further interaction.
    /// </summary>
    public void CloseShop()
    {
        register.store = null;
        Destroy(gameObject);
    }
}