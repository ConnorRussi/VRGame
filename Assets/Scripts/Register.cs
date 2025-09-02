using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
public class Register : MonoBehaviour, IButtonInteractor
{
    public GameObject registerDrawer; // Reference to the drawer GameObject
    Drawer drawer; // Reference to the Drawer script
    //public bool isOpen = false; // Track if the register is open or closed
    //public bool closed = true; // Track if the register is closed
    public Stack<GameObject> itemsToProcess = new Stack<GameObject>();
    public Vector3 boxOffset = new Vector3(0f, 0f, 0.5f);
    public Vector3 boxHalfExtents = new Vector3(0.2f, 0.2f, 0.2f);
    public LayerMask itemLayerMask; // Layer mask to filter items in the register 
    public int pushForce;
    public TextMeshProUGUI valueText;
    public int totalValue;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip registerOpenSound;
    public AudioClip registerCloseSound;
    [Header("debug")]
    public bool process;
    public bool openDrawer;
    public bool close;

    void Awake()
    {
        drawer = registerDrawer.GetComponent<Drawer>(); // Get the Drawer component attached to the same GameObject
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    public void Update()
    {
        if (process)
        {
            ProcessDrawer(); // Call the ProcessDrawer method if process is true
            process = false; // Reset process to false after processing
        }
        if (openDrawer)
        {
            OpenRegister(); // Call the OpenRegister method if openDrawer is true
            openDrawer = false; // Reset openDrawer to false after opening
        }
        if(close)
        {
            drawer.LockDrawer(); // Call the LockDrawer method to close the drawer
            // closed = true; // Set closed to true
            // isOpen = false; // Set isOpen to false
            close = false; // Reset close to false after closing
        }
    }
    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.layer == LayerMask.NameToLayer("bullet"))
    //     {
    //             OpenRegister();
    //     }
    // }
    public void OpenRegister()
    {
        if (drawer.isLocked)
        {
            drawer.ReleaseDrawer(); // Call the ReleaseDrawer method to open the drawer
            audioSource.PlayOneShot(registerOpenSound);
            // isOpen = true; // Set isOpen to true
            // closed = false; // Set closed to false
        }
        else
        {
            Debug.Log("register is already open");
        }
    }

    public void ProcessDrawer()
    {
        if (!drawer.isLocked)
        {
            Debug.LogError("DRAWER IS OPEN YET TRYING TO PROCESS");
            return; // Prevent processing if the drawer is already open
        }
        // Use the public fields for box center and size
        Vector3 boxCenter = transform.position + transform.rotation * boxOffset;

        // Get all colliders in the box area
        Collider[] colliders = Physics.OverlapBox(boxCenter, boxHalfExtents, transform.rotation, itemLayerMask);

        // Clear the stack before adding new items
        itemsToProcess.Clear();

        foreach (Collider col in colliders)
        {
            // Optionally filter by tag/layer/component
            itemsToProcess.Push(col.gameObject);
            totalValue = col.GetComponent<CInteractable>().coinValue + totalValue;
            UpdateRegScreen();
            Destroy(col.gameObject, 1f); // destroy after 2 seconds
        }

        Debug.Log($"Found {itemsToProcess.Count} items in the register area.");
    }
    public void UpdateRegScreen()
    {
        valueText.text = "$" + totalValue.ToString();
    }
     // Draw the box in the Scene view for easy tweaking
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = transform.position + transform.rotation * boxOffset;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }
    void IButtonInteractor.Activate()
    {
        OpenRegister();
    }
}



