using UnityEngine;

public class NavPoint : MonoBehaviour
{
    public GameObject[] connectedPoints; // Array of connected NavPoints that require to be linked in order to walk and not lock up the navigation
    public bool claimed = false; // Flag to indicate if this NavPoint is claimed by an NPC
    public GameObject owner;
    public bool spawnPoint = false; // Flag to indicate if this NavPoint is the spawn point

    public void claim(GameObject claimer)
    {
        claimed = true; // Set the NavPoint as claimed
        owner = claimer; // Set the owner of the NavPoint
        if (connectedPoints != null)
        {
            foreach (GameObject point in connectedPoints)
            {
                NavPoint navPoint = point.GetComponent<NavPoint>();
                if (navPoint != null && !navPoint.claimed)
                {
                    navPoint.claim(claimer); // Recursively claim connected NavPoints
                }
            }
        }
        Debug.Log("SpawnPoint claimed by: " + owner.name);
    }
    public bool release()
    {
        claimed = false; // Set the NavPoint as unclaimed
        owner = null; // Clear the owner of the NavPoint
        if (connectedPoints != null)
        {
            foreach (GameObject point in connectedPoints)
            {
                NavPoint navPoint = point.GetComponent<NavPoint>();
                if (navPoint != null && navPoint.claimed)
                {
                    navPoint.release(); // Recursively release connected NavPoints
                }
            }
        }
        return true;
    }
}
