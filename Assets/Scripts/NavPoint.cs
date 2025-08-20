using UnityEngine;

public class NavPoint : MonoBehaviour
{
    public GameObject[] connectedPoints; // Array of connected NavPoints that require to be linked in order to walk and not lock up the navigation
    public bool claimed = false; // Flag to indicate if this NavPoint is claimed by an NPC
    public GameObject owner;


    public void claim()
    {
        claimed = true; // Set the NavPoint as claimed
        owner = gameObject; // Set the owner of the NavPoint
        if (connectedPoints != null)
        {
            foreach (GameObject point in connectedPoints)
            {
                NavPoint navPoint = point.GetComponent<NavPoint>();
                if (navPoint != null && !navPoint.claimed)
                {
                    navPoint.claim(); // Recursively claim connected NavPoints
                }
            }
        }
    }
    public void release()
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
    }
}
