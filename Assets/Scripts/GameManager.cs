using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject MugSpawnHolder, GlassSpawnHolder, bottleSpawnHolder, respawnAbleBottleSpawner;
    public GameObject MugPrefab, glassPrefab;
    public List<GameObject> bottles;
    //public List<GameObject> bottleSpawns;
    public List<GameObject> coasters, unclaimedCoasters; // List of all coasters and unclaimed coasters
    //public List<GameObject> mugSpawns;
    //public List<GameObject> glassSpawns;
    public bool onDelays = false; // Flag to control the delay before respawning
    public float checkDelay; // Delay in seconds between checks for existing objects



    [System.Serializable]
    public class SocketInfo
    {
        public Transform socket;       // The socket position
        public GameObject prefab;      // Prefab to spawn
        [HideInInspector] public GameObject currentObject; // *** CHANGED *** Tracks what's currently in the socket
        [HideInInspector] public float emptyTime;           // *** CHANGED *** Time since object 
        public float maxDistance = 3.0f; //Should be set for each socket based on how far, this is done to just work currently but can be changed later;
    }
    [Header("Socket (bottles, mug, glasses) Information")]
    public List<SocketInfo> shotGlassSockets = new List<SocketInfo>();
    public List<SocketInfo> mugSockets = new List<SocketInfo>();
    public List<SocketInfo> bottleSockets = new List<SocketInfo>();
    public List<SocketInfo> NonReSpawnBottleSockets = new List<SocketInfo>();
    public float respawnDelay = 3f; //How long to wait before respawning an object in a socket

    [Header("NPC properties")]
    public GameObject npcPrefab; // Prefab for the NPC
    public GameObject npcSpawnPoint; // Point where NPCs will be spawned
    public int maxNPC; // Number of NPCs to spawn
    public List<GameObject> npcs = new List<GameObject>(); // List to hold spawned NPCs
    public float minNpcSpawnDelay, maxNpcSpawnDelay = 3.0f; // Delay between NPC spawns
    public bool spawning = true; // Flag to control the spawning process



    void Awake()
    {
        FindAllCoasters();
        CollectSockets(MugSpawnHolder, MugPrefab, mugSockets);
        CollectSockets(GlassSpawnHolder, glassPrefab, shotGlassSockets);
        CollectSockets(bottleSpawnHolder, null, NonReSpawnBottleSockets, bottles);
        CollectSockets(respawnAbleBottleSpawner, null, bottleSockets, bottles);
        StartCoroutine(CheckRespawns());
    }
    public void Start()
    {
        StartCoroutine(NPCManagment()); // Start managing NPCs
    }
    /// <summary>
    /// Finds all coasters in the scene and stores them in the coasters list.
    /// Initially, all coasters are deactivated.
    /// </summary>
    void FindAllCoasters()
    {
        coasters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Coaster"));
        unclaimedCoasters = new List<GameObject>(coasters);
        foreach (GameObject coaster in coasters)
        {
            coaster.GetComponent<Coaster>().gameManager = this; // Set the GameManager reference in each coaster
            coaster.GetComponent<Coaster>().wrapper.SetActive(false); // Deactivate all coasters initially
        }
        Debug.Log("all coasters turned off");
    }
    /// <summary>
    /// Assigns an unclaimed coaster to the NPC.
    /// If no unclaimed coasters are available, it logs a warning.
    /// If there are unclaimed coasters, it randomly selects one and claims it for the NPC
    /// </summary>
    /// <param name="npc"></param>
    public GameObject assignCoaster(GameObject npc)
    {
        Debug.Log("claiming a coaster for " + npc.name);
        //*****&Later if want to save some performance can use a order of coasters to assign instead of random
        if (coasters.Count == 0)
        {
            Debug.LogWarning("There are No coasters.");
            return null;
        }
        if (unclaimedCoasters.Count == 0)
        {
            Debug.LogWarning("All coasters are claimed.");
            return null;
        }

        // Assign a random unclaimed coaster
        int randomIndex = Random.Range(0, unclaimedCoasters.Count);
        GameObject chosenCoaster = unclaimedCoasters[randomIndex];
        Coaster chosenComponent = chosenCoaster.GetComponent<Coaster>();
        chosenComponent.Claim(npc.GetComponent<NPC>());
        unclaimedCoasters.RemoveAt(randomIndex); // Remove the chosen coaster from the unclaimed list
        NPCPathFinding npcPathFinding = npc.GetComponent<NPCPathFinding>();
        npcPathFinding.path = chosenComponent.path; // Assign the path from the coaster to the
        StartCoroutine(npcPathFinding.PathFindIn());
        return chosenCoaster; // Return the assigned coaster
    }
    // void FindAllSpawns(GameObject spawnHolder, GameObject prefab, SocketInfo[] socketInfos)
    // {
    //     foreach (Transform child in spawnHolder.transform)
    //     {
    //         socketInfos.Add(child.gameObject);
    //         if (prefab == null) continue; // Skip if no prefab is provided for bottles
    //         Instantiate(prefab, child.position, child.rotation);
    //     }
    // }
    // void SpawnBottles()
    // {
    //     for (int i = 0; i < bottleSpawns.Count; i++)
    //     {
    //         if (i < bottles.Count)
    //         {
    //             Instantiate(bottles[i], bottleSpawns[i].transform.position, bottleSpawns[i].transform.rotation);
    //         }
    //         else
    //         {
    //             Debug.LogWarning("Not enough bottles to spawn at all bottle spawns.");
    //             break;
    //         }
    //     }
    // }
    // public void RespawnObject(GameObject spawnPoint, GameObject prefab)
    // {
    //     //yield return new WaitForSeconds(5f); // Wait for 5 seconds before respawning
    //     Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    //     Debug.Log("Respawned object at: " + spawnPoint.name);
    // }
    // public System.Collections.IEnumerator CheckBottleRespawn()
    // {
    //     while (spawning)
    //     {   
    //         if(onDelays)
    //         {
    //             yield return new WaitForSeconds(checkDelay);
    //             continue; // Skip the rest of the loop if on delay
    //         }
    //         foreach (GameObject bottleSpawn in bottleSpawns)
    //         {
    //         //    if (Vector3.Distance(bottleSpawn.transform.position, currentSpawnedObject.transform.position) < checkradius)
    //         //     {
    //         //         yield return new WaitForSeconds(checkDelay);
    //         //         continue; // Skip the rest of the loop if an object is found
    //         //     }
    //         }

    //         //spawn if the object is not found
    //         onDelays = true; // Set the delay flag to prevent immediate respawning
    //         //SpawnObject();
    //         yield return new WaitForSeconds(checkDelay);

    //     }
    // }
    void CollectSockets(GameObject spawnHolder, GameObject prefab, List<SocketInfo> socketList)
    {
        foreach (Transform child in spawnHolder.transform)
        {
            SocketInfo info = new SocketInfo();
            info.socket = child;
            info.prefab = prefab;
            info.currentObject = prefab != null ? Instantiate(prefab, child.position, child.rotation) : null;
            socketList.Add(info);
        }
    }

    // *** CHANGED *** Overload for bottles with different prefabs per socket
    void CollectSockets(GameObject spawnHolder, GameObject unused, List<SocketInfo> socketList, List<GameObject> prefabList)
    {
        int i = 0;
        foreach (Transform child in spawnHolder.transform)
        {
            SocketInfo info = new SocketInfo();
            info.socket = child;
            if (i < prefabList.Count)
            {
                info.prefab = prefabList[i];
                info.currentObject = Instantiate(prefabList[i], child.position, child.rotation);
            }
            socketList.Add(info);
            i++;
        }
    }

    // *** CHANGED *** Continuous check for respawn
    System.Collections.IEnumerator CheckRespawns()
    {
        while (true)
        {
            CheckSocketList(mugSockets);
            CheckSocketList(shotGlassSockets);
            CheckSocketList(bottleSockets);
            yield return new WaitForSeconds(checkDelay);
        }
    }

    // *** CHANGED *** Timer per socket
    void CheckSocketList(List<SocketInfo> sockets)
    {
        foreach (var socket in sockets)
        {
            if (socket.currentObject == null || Vector3.Distance(socket.socket.position, socket.currentObject.transform.position) > socket.maxDistance)
            {
                socket.emptyTime += checkDelay;
                if (socket.emptyTime >= respawnDelay)
                {
                    socket.currentObject = Instantiate(socket.prefab, socket.socket.position, socket.socket.rotation);
                    socket.emptyTime = 0f;
                }
            }
            else
            {
                socket.emptyTime = 0f; // reset if occupied
            }
        }
    }

    void SpawnNPC()
    {
        if (npcs.Count >= maxNPC)
        {
            Debug.LogWarning("Maximum number of NPCs already spawned.");
            return;
        }
        GameObject npc = Instantiate(npcPrefab, npcSpawnPoint.transform.position, Quaternion.identity);
        npc.GetComponent<NPC>().gameManager = this; // Set the GameManager reference in each NPC
        npcs.Add(npc);
        Debug.Log("Spawned NPC: " + npc.name);
        
        
        
    }
    System.Collections.IEnumerator NPCManagment()
    {
        while (spawning)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second before checking again
            while (npcs.Count < maxNPC)
            {
                yield return new WaitForSeconds(Random.Range(minNpcSpawnDelay,maxNpcSpawnDelay)); // Wait for __ seconds before spawning a new NPC
                if (!npcSpawnPoint.GetComponent<NavPoint>().claimed)
                {
                    SpawnNPC();
                }

            }
        }

    }
}
