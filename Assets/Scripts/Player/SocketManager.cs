using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketManager : MonoBehaviour
{
    public GameObject mouthSocket;
    public GameObject headSocket;
    public GameObject rHolsterSocket;
    public GameObject lHolsterSocket;

    public void Equip(SelectEnterEventArgs args)
    {
        var obj = args.interactableObject.transform.gameObject;
        var cInt = obj.GetComponent<CInteractable>();
        if (cInt == null)
        {
            Debug.LogError("EquipMouth: Selected object does not have a CInteractable component! (" + obj.name + ")");
            return;
        }
        if (obj.CompareTag("MouthWearable"))
        {
            // Mouth logic
            mouthSocket = obj;
        }
        else if (obj.CompareTag("Hat"))
        {
            // Head logic
            headSocket = obj;
        }
        else if (obj.CompareTag("Gun"))
        {
            obj.GetComponent<Revolver>().Holster();
            // Gun holster logic
            var holster = args.interactorObject.transform.gameObject;
            if (holster.CompareTag("Right Hand"))
            {
                rHolsterSocket = obj;
            }
            else
            {
                lHolsterSocket = obj;
            }
        }
        else
        {
            Debug.LogError("Equip: Selected object does not have a valid tag! (" + obj.name + ")");
            return;
        }
        cInt.Equip();
    }
    public void Unequip(SelectExitEventArgs args)
    {
        var obj = args.interactableObject.transform.gameObject;
        var cInt = obj.GetComponent<CInteractable>();
        if (cInt == null)
        {
            Debug.LogError("Unequip: Selected object does not have a CInteractable component! (" + obj.name + ")");
            return;
        }
        if (obj.CompareTag("MouthWearable"))
        {
            // Mouth logic
            mouthSocket = null;
        }
        else if (obj.CompareTag("Hat"))
        {
            // Head logic
            headSocket = null;
        }
        else if (obj.CompareTag("Gun"))
        {
            // Gun holster logic
            var holster = args.interactorObject.transform.gameObject;
            if (holster.CompareTag("RHolster"))
            {
                rHolsterSocket = null;
            }
            else
            {
                lHolsterSocket = null;
            }
            obj.GetComponent<Revolver>().UnHolster();
        }
        else
        {
            Debug.LogError("Equip: Selected object does not have a valid tag! (" + obj.name + ")" + obj.tag);
            return;
        }
        cInt.Unequip();
    }
   
   
}
