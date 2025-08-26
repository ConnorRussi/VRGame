using UnityEngine;

public class ColliderRelay : MonoBehaviour
{
    public GameObject parentObject; // Reference to the parent object
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
}
