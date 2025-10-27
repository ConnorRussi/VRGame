using UnityEngine;

public class ColliderRelay : MonoBehaviour
{
    public GameObject parentObject; // Reference to the parent object
    public bool sendCollisionMessage = false;
    public T relay<T>() where T : MonoBehaviour
    {
        if (parentObject == null)
        {
            Debug.LogWarning($"{name} has no parentObject assigned!");
            return null;
        }

        T script = parentObject.GetComponent<T>();
        if (script == null)
        {
            Debug.LogWarning($"{parentObject.name} has no component of type {typeof(T)}!");
        }
        return script;
    }
    
     void OnCollisionEnter(Collision collision)
    {
        if (sendCollisionMessage && parentObject != null)
        {
            // Try to get a script on the parent that implements ICollisionReceiver
            var receiver = parentObject.GetComponent<ICollisionReciever>();
            if (receiver != null)
            {
                receiver.ReceiveCollisionInfo(collision);
            }
            else
            {
                Debug.LogWarning($"{parentObject.name} does not implement ICollisionReceiver!");
            }
        }
    }
}
