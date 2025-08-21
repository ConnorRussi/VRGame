using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    public Player playerScript;
    //its just a relay to the main player script from where the bullets will hit
    public Player GetPlayer()
    {
        return playerScript;
    }
}
