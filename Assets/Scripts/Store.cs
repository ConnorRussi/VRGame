using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using Unity.XR.CoreUtils;

public class Store : MonoBehaviour
{
    [Header("Store Setup")]
    //public int maxTopItems = 3;
    //public int maxBottomItems = 3;
    //public int minBottomItems = 1;
    //public int minTopItems = 1;
    public StoreItemSO[] topItems; // Assign in inspector
    public StoreItemSO[] bottomItems; // Assign in inspector
    public Transform[] topSockets;   // Assign in inspector, one per item
    
    
    public Transform[] bottomSockets; // Assign in inspector, one per item
    //public Transform[] itemSockets;  // Assign in inspector, one per item
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

    public void Update()
    {
        if (openLock)
        {
            PopOpen();
            openLock = false;
        }
    }
    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        // if (topSockets.Length < maxTopItems)
        // {
        //     maxTopItems = topSockets.Length;
        //     Debug.LogError("Not enough item sockets assigned for the maximum number of items changed. Adjusting maxTopItems to " + maxTopItems);
        // }
        // if (minBottomItems > maxBottomItems || minBottomItems > bottomSockets.Length)
        // {
        //     minBottomItems = Mathf.Min(maxBottomItems, topSockets.Length);
        //     Debug.LogError("Minimum bottom items cannot exceed maximum. Adjusting minBottomItems to " + minBottomItems);
        // }
        // if (bottomSockets.Length < maxBottomItems)
        // {
        //     maxBottomItems = bottomSockets.Length;
        //     Debug.LogError("Not enough item sockets assigned for the maximum number of items changed. Adjusting maxBottomItems to " + maxBottomItems);
        // }
        // if (minTopItems > maxTopItems || minTopItems > topSockets.Length)
        // {
        //     minTopItems = Mathf.Min(maxTopItems, topSockets.Length);
        //     Debug.LogError("Minimum top items cannot exceed maximum. Adjusting minTopItems to " + minTopItems);
        // }
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
    }
    void Start()
    {
        SpawnItems();
        //SetCosts();
        SetLockState(true);
    }

/// <summary>
/// COME BACK and make it spawn in a random socket not just the first N sockets
/// </summary>
    void SpawnItems()
    {
        //int topCount = UnityEngine.Random.Range(minTopItems, maxTopItems + 1);
        //int bottomCount = UnityEngine.Random.Range(minBottomItems, maxBottomItems + 1);



        //int i = 0;
        for (int i = 0; i < storeSockets.Count; i++)
        {
            storeSockets[i].costBoardObject.SetActive(false);
            StoreItemSO itemToSpawn = null;
            int itemIndex = -1;
            if (i < topSockets.Length)
            {
                itemIndex = UnityEngine.Random.Range(0, topItems.Length);
                itemToSpawn = topItems[itemIndex];
            }
            else
            {
                int botIndex = UnityEngine.Random.Range(0, bottomItems.Length);
                itemToSpawn = bottomItems[botIndex];
                itemIndex = topItems.Length + botIndex;
            }


            GameObject item = Instantiate(itemToSpawn.prefab, storeSockets[i].socket.transform.position, storeSockets[i].socket.transform.rotation, storeSockets[i].socket);
            spawnedItems.Add(item);

            // Optionally add a StoreItemGrabbable script for grab logic
            var grabbable = item.GetComponent<StoreItemGrabbable>();
            grabbable.inStore = true;
            grabbable.store = this;
            grabbable.itemIndex = itemIndex;

            storeSockets[i].costLabel.text = "$" + itemToSpawn.cost.ToString();
            item.GetComponent<Rigidbody>().isKinematic = true; // Prevent physics until grabbed
            storeSockets[i].costBoardObject.SetActive(false);
        }
    }



    void SetLockState(bool locked)
    {
        isLocked = locked;
        if (lockObject) lockObject.SetActive(locked);
        //if (hitMeArt) hitMeArt.SetActive(locked);
        if (!locked)
        {
            for (int i = 0; i < storeSockets.Count; i++)
            {
                storeSockets[i].costBoardObject.SetActive(true);
            }
        }
    }

    public void ApplyForceToLock(float force)
    {
        if (!isLocked) return;
        currentForce += force;
        if (currentForce >= breakForce)
        {
            PopOpen();
        }
    }

    void PopOpen()
    {

        if (explosionParticles) explosionParticles.Play();
        if (audioSource && explosionSound) audioSource.PlayOneShot(explosionSound);
        // Optionally animate lock flying off, etc.
        //StartCoroutine(OpenLid());
        animator.SetTrigger("OpenBox");
    }
    public void LidFullyOpen()
    {
        SetLockState(false);
    }
    //teh angles are not working maybe try and animate it?
    // private System.Collections.IEnumerator OpenLid()
    // {
    //     Debug.Log("Opening lid");
    //     Debug.Log("z = " + lid.transform.localEulerAngles.z);
    //     Debug.Log("lidopenangle " + lidOpenAngle);
    //     Debug.Log(lid.transform.localEulerAngles.z > lidOpenAngle);
    //     while (lid.transform.localEulerAngles.z < lidOpenAngle)
    //     {
    //         Debug.Log(lid.transform.localEulerAngles.z);
    //         float newAngle = Mathf.LerpAngle(lid.transform.localEulerAngles.z, lidOpenAngle, Time.deltaTime * lidSpeed);
    //         lid.transform.localEulerAngles = new Vector3(lid.transform.localEulerAngles.x, lid.transform.localEulerAngles.y, newAngle);
    //         yield return null;
    //     }
    //     Debug.Log("Lid fully opened");
    //     SetLockState(false);
    // }

    public bool CanAfford(int itemIndex)
    {
        return register && register.totalValue >= storeItems[itemIndex].cost;
    }

    public bool TryGrabItem(int itemIndex, GameObject item)
    {
        if (isLocked)
        {
            if(debug)Debug.Log("Store is locked. Cannot grab items.");
            return false;
        }
        if (!CanAfford(itemIndex))
        {
            if(debug)Debug.Log("Not enough funds to purchase this item.");
            // Optionally, play a denied sound or shake the item
            return false;
        }
        if(debug)Debug.Log("Item purchased: " + storeItems[itemIndex].name);
        register.totalValue -= storeItems[itemIndex].cost;
        register.UpdateRegScreen();
        // Detach from socket so it can be grabbed
        item.transform.SetParent(null);
        // Optionally, update UI or play a sound
        return true;
    }
    public bool IsLocked()
    {
        return isLocked;
    }
}
