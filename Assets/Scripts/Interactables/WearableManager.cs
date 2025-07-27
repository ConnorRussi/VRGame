using UnityEngine;

public class WearableManager : MonoBehaviour
{
    public bool mouthWorn = false; // Indicates if the mouth wearable is currently worn
    public bool headWorn = false; // Indicates if the head wearable is currently worn
    public GameObject mouthWearable; // Reference to the mouth wearable GameObject
    public GameObject headWearable; // Reference to the head wearable GameObject
    public GameObject mouthPosition; // Reference to the mouth position GameObject
    public GameObject headPosition; // Reference to the head position GameObject
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mouthWorn = false;
        headWorn = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("WearableManager: OnTriggerEnter with " + other.gameObject.name);
        if (other.CompareTag("MouthWearable"))
        {
            //check if mouth wearable is already worn and if so unequip it
            EquipMouth(other.gameObject); // Equip the mouth wearable
        }
        else if (other.CompareTag("HeadWearable"))
        {
            EquipHead(other.gameObject); // Equip the head wearable
        }
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log("WearableManager: OnTriggerExit with " + other.gameObject.name);
        if (other.CompareTag("MouthWearable"))
        {
            DequipMouth(); // Unequip the mouth wearable
        }

    }
    void EquipMouth(GameObject mouthObject)
    {
        mouthWearable = mouthObject; // Assign the mouth wearable GameObject
        if (mouthWearable != null)
        {
            mouthWearable.transform.SetParent(mouthPosition.transform); // Parent to mouth position
            //mouthWearable.transform.localPosition = Vector3.zero;       // Align position
            //mouthWearable.transform.localRotation = Quaternion.identity; // Align rotation
            mouthWorn = true;
            mouthWearable.GetComponent<WearableInteractable>().Equip(gameObject); // Call the equip method on the wearable interactable
            // Additional logic for equipping the mouth wearable
        }
    }
    void EquipHead(GameObject headObject)
    {
        headWearable = headObject; // Assign the head wearable GameObject
        if (headWearable != null)
        {
            headWearable.transform.SetParent(headPosition.transform); // Parent to head position
            headWearable.transform.localPosition = Vector3.zero;       // Align position
            headWearable.transform.localRotation = Quaternion.identity; // Align rotation
            headWorn = true;
            // Additional logic for equipping the head wearable
        }
    }
    void DequipMouth()
    {
        if (mouthWearable != null)
        {
            mouthWearable.transform.SetParent(null); // Remove parent
            mouthWearable.GetComponent<WearableInteractable>().Dequip();
            mouthWorn = false;
            mouthWearable = null; // Clear the reference
        }
    }
}   
