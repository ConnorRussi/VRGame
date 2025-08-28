using System.Collections.Specialized;
using UnityEngine;

public class NPCPathFinding : MonoBehaviour
{
    public GameObject[] path; // Array of waypoints for the NPC to follow
    private int currentWaypointIndex = 0; // Index of the current waypoint
    //***************could do a max/min speed from NPC defeaults
    public float speed = 2f; // Speed at which the NPC moves
    NPC npc; // Reference to the NPC that will follow the path
    bool reachedEnd = false; // Flag to check if the NPC has reached the end of the path
    float tolerance = 0.05f; // Distance tolerance to consider the waypoint reached

    void Start()
    {
        npc = GetComponent<NPC>();
    }
    private bool MoveTowardsWaypoint()
    {
        //Debug.Log("Moving towards waypoint: " + currentWaypointIndex);
        Transform targetWaypoint = path[currentWaypointIndex].transform;

        // Direction only on XZ plane
        Vector3 direction = targetWaypoint.position - transform.position;
        direction.y = 0f;
        direction = direction.normalized;

        // Move towards the waypoint
        transform.position += direction * Time.deltaTime * speed;

        // Check distance only on XZ plane
        Vector3 flatTarget = new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z);
        if (Vector3.Distance(transform.position, flatTarget) < tolerance)
        {
            return true; // Reached waypoint (ignoring Y)
        }

        return false; // Still moving
    }
    public System.Collections.IEnumerator PathFindIn()
    {
        path[currentWaypointIndex].GetComponent<NavPoint>().claim(gameObject); // Claim the current waypoint
                                                                               // Debug.Log("Starting pathfinding for NPC: " + gameObject.name);
        while (!reachedEnd)
        {
            yield return new WaitForFixedUpdate(); // Wait for the next physics update

            if (MoveTowardsWaypoint())
            {
                if (currentWaypointIndex + 1 >= path.Length || !path[currentWaypointIndex + 1].GetComponent<NavPoint>().claimed)
                {
                    path[currentWaypointIndex].GetComponent<NavPoint>().release(); // Release the waypoint if it was claimed
                }
                Debug.Log("Reached waypoint: " + currentWaypointIndex + " " + path[currentWaypointIndex].name);
                currentWaypointIndex++;
                if (currentWaypointIndex >= path.Length)
                {
                    //Debug.Log(gameObject.name + " has reached the end of the path, enter.");
                    StartCoroutine(npc.UpdateAngerLevel());
                    yield break; // Exit the coroutine
                }
                path[currentWaypointIndex].GetComponent<NavPoint>().claim(gameObject); // Claim the current waypoint

            }
            else
            {
                yield return null; // Wait for the next frame if not moving
            }
        }
        // Debug.Log("end of pathfinding for NPC: " + gameObject.name);
        reachedEnd = true; // Set the flag to true when the end of the path is
    }
    public System.Collections.IEnumerator PathFindOut()
    {
        currentWaypointIndex--; // Start from the last waypoint
        reachedEnd = false; // Reset the flag for the return path
        path[currentWaypointIndex].GetComponent<NavPoint>().claim(gameObject); // Claim the current waypoint

        while (!reachedEnd)
        {
            yield return new WaitForFixedUpdate(); // Wait for the next physics update
            if (MoveTowardsWaypoint())
            {
                if (currentWaypointIndex == 0)
                {
                    //Debug.Log(gameObject.name + " has reached the end of the path, exit.");
                    path[currentWaypointIndex].GetComponent<NavPoint>().release(); // Release the waypoint if it was claimed
                    reachedEnd = true;
                    npc.Die();
                    yield break; // Exit the coroutine
                }
                if (!path[currentWaypointIndex - 1].GetComponent<NavPoint>().claimed)
                {
                    path[currentWaypointIndex].GetComponent<NavPoint>().release(); // Release the waypoint if it was claimed
                }
                //path[currentWaypointIndex].GetComponent<NavPoint>().release();
                currentWaypointIndex--;


                path[currentWaypointIndex].GetComponent<NavPoint>().claim(gameObject); // Claim the current waypoint

            }
            else
            {
                yield return null; // Wait for the next frame if not moving
            }
        }

    }


    public void ForceReleaseNavPoints()
    {   Debug.Log("Force releasing nav points for NPC: " + gameObject.name);
        if (currentWaypointIndex < path.Length && currentWaypointIndex >= 0)
        {
            NavPoint point = path[currentWaypointIndex].GetComponent<NavPoint>();
            if(point == null)
            {
                Debug.LogError("NavPoint component is missing on waypoint: " + path[currentWaypointIndex].name);
                return;
            }
            if (point.claimed && point.owner == gameObject) path[currentWaypointIndex].GetComponent<NavPoint>().release();
        }
        
            
        
    } 


}
