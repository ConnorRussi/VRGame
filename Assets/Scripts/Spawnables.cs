using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Spawnables : MonoBehaviour
{
    //when hand has entered into the trigger area if the hand grabs spawn in 
    //the object into the players hand
    public GameObject spawnablePrefab;
    public GameObject currentSpawnedObject;
    public Transform spawnPoint;
    public float spawnDelay = 0.5f; // Delay before spawning the object
    bool onDelays = false;
    public float checkradius = 0.5f; // Radius to check for existing objects
    public float checkDelay = 1.0f; // Delay between checks for existing objects
    private bool spawning = true; // Flag to control the spawning process
    public LayerMask checkLayerMask; // Layer mask to filter objects in the sphere
    void Start()
    {
        SpawnObject();
        // if (currentSpawnedObject == null)
        // {
        //     SpawnObject();
        // }
        
    }
    private System.Collections.IEnumerator CheckForObject()
    {
        while (spawning)
        {   
            if(onDelays)
            {
                yield return new WaitForSeconds(checkDelay);
                continue; // Skip the rest of the loop if on delay
            }
            if(Vector3.Distance(spawnPoint.position, currentSpawnedObject.transform.position) < checkradius)
            {
                yield return new WaitForSeconds(checkDelay);
                continue; // Skip the rest of the loop if an object is found
            }
            //spawn if the object is not found
            Destroy(currentSpawnedObject);
            currentSpawnedObject = null;
            onDelays = true; // Set the delay flag to prevent immediate respawning
            SpawnObject();
            yield return new WaitForSeconds(checkDelay);
            
        }
       
    }
    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(spawnPoint.position, checkradius);
    // }
    void SpawnedObjectGrabbed(SelectEnterEventArgs args)
    {
        if (onDelays)
        {
            return; // Prevent spawning if already on delay
        }
        onDelays = true;
        currentSpawnedObject.GetComponent<CInteractable>().SpawnableObjectTransition(false);
        // Disable the colliders used for grabbing the object from the spawner
        Invoke("SpawnObject", spawnDelay);
    }
    void SpawnObject()
    {
        currentSpawnedObject = Instantiate(spawnablePrefab, spawnPoint.position, spawnPoint.rotation);

        XRGrabInteractable grabInteractable = currentSpawnedObject.GetComponent<XRGrabInteractable>();
        currentSpawnedObject.GetComponent<CInteractable>().SpawnableObjectTransition(true);
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(SpawnedObjectGrabbed);
        }
        else
        {
            Debug.LogWarning("currentSpawnedObject does not have XRGrabInteractable!");
        }
        
        onDelays = false; // Reset the delay flag
    }
    
}
