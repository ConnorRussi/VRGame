using UnityEngine;

public class ForceLayer : MonoBehaviour
{
    //This is made mainly for the Hands so if someone changes the XR rig layer the hand colliders will still be on the correct layer
    public LayerMask forceLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.layer = Mathf.RoundToInt(Mathf.Log(forceLayer.value, 2));
    }

    
}
