using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

public class Register : MonoBehaviour, IButtonInteractor
{
    public GameObject registerDrawer; // Reference to the drawer GameObject
    Drawer drawer; // Reference to the Drawer script
    public Stack<GameObject> itemsToProcess = new Stack<GameObject>();
    public Vector3 boxOffset = new Vector3(0f, 0f, 0.5f);
    public Vector3 boxHalfExtents = new Vector3(0.2f, 0.2f, 0.2f);
    public LayerMask itemLayerMask; // Layer mask to filter items in the register 
    public int pushForce;
    public TextMeshProUGUI valueText;
    public int totalValue;
    public Store store;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip registerOpenSound;
    public AudioClip registerCloseSound;
    [Header("debug")]
    public bool debug;
    public bool process;
    public bool openDrawer;
    public bool close;

    /// <summary>
    /// Initializes references to the drawer and audio source.
    /// initializes regscreen and start value
    /// </summary>
    void Awake()
    {
        drawer = registerDrawer.GetComponent<Drawer>();
        audioSource = gameObject.GetComponent<AudioSource>();
        totalValue = 0;
        UpdateRegScreen();
    }

    /// <summary>
    /// Handles process, open, and close flags each frame.
    /// </summary>
    public void Update()
    {
        if (process)
        {
            ProcessDrawer();
            process = false;
        }
        if (openDrawer)
        {
            OpenRegister();
            openDrawer = false;
        }
        if (close)
        {
            drawer.LockDrawer();
            close = false;
        }
    }

    /// <summary>
    /// Opens the register drawer if it is locked, plays sound.
    /// </summary>
    public void OpenRegister()
    {
        if (drawer.isLocked)
        {
            drawer.ReleaseDrawer();
            audioSource.PlayOneShot(registerOpenSound);
        }
        else
        {
            Debug.Log("register is already open");
        }
    }

    /// <summary>
    /// Processes all items in the register area, adds their value, destroys them, should occur only when drawer is closed.
    /// </summary>
    public void ProcessDrawer()
    {
        if (!drawer.isLocked)
        {
            Debug.LogError("DRAWER IS OPEN YET TRYING TO PROCESS");
            return;
        }
        Vector3 boxCenter = transform.position + transform.rotation * boxOffset;
        Collider[] colliders = Physics.OverlapBox(boxCenter, boxHalfExtents, transform.rotation, itemLayerMask);
        itemsToProcess.Clear();

        foreach (Collider col in colliders)
        {
            itemsToProcess.Push(col.gameObject);
            if (col.GetComponent<CInteractable>() != null)
            {
                Debug.Log("FOUND COIN IN REGISTER: " + col.name);
                int value = col.GetComponent<CInteractable>().coinValue;
                if (value != 0) UpdateTotalValue(value);
            }
            
            Destroy(col.gameObject, 1f);
        }

        if (debug) Debug.Log($"Found {itemsToProcess.Count} items in the register area.");
    }

    /// <summary>
    /// Updates the register screen UI with the current total value.
    /// </summary>
    public void UpdateRegScreen()
    {
        valueText.text = "$" + GetTotalValue().ToString();
    }

    /// <summary>
    /// Called when a button is activated, opens the register.
    /// </summary>
    void IButtonInteractor.Activate()
    {
        OpenRegister();
    }

    

    /// <summary>
    /// Returns the current total value in the register.
    /// </summary>
    public int GetTotalValue()
    {
        return totalValue;
    }

    /// <summary>
    /// Adds value to the register, updates UI, and updates purchasable items if store is unlocked.
    /// </summary>
    public void UpdateTotalValue(int valueToAdd)
    {
        totalValue = totalValue + valueToAdd;
        UpdateRegScreen();
        if (store != null && !store.IsLocked())
        {
            Debug.Log("updating purchasable items list because added coins");
            store.UpdatePurchaseAblesList();
        }
    }
}