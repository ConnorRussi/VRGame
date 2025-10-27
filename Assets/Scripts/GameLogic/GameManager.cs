using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour, IButtonInteractor
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
    public Register register; // Reference to the register
    [Header("day and night cycle")]
    public int day;
    public enum timeOfDay { Day, Night };
    public timeOfDay currentTime;
    public int startingNPCs;
    public float NPCsScaleFactor; // How much to scale the number of NPCs by each day
    public int npcsToday;
    public int todayNPCsSpawned;
    public GameObject dayNightButton;
    public float difficultyScale = 0.3f; // How much to scale the difficulty by each day
                                         //CHANGE
    public GameObject storePrefab;
    public Transform storeSpawnPoint;
    public Store store;



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
        if (spawning) StartCoroutine(NPCManagment()); // Start managing NPCs
        dayNightButton.SetActive(false); // Hide the day/night button at the start
        npcsToday = startingNPCs;
        currentTime = timeOfDay.Day;
        if (NPCsScaleFactor <= 1)
        {
            Debug.LogError("NPCsScaleFactor must be greater than 1 to increase NPCs each day. Setting to default value of 1.1");
            NPCsScaleFactor = 1.1f; // Set a default value to prevent
        }
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
       // Debug.Log("all coasters turned off");
    }
    /// <summary>
    /// Assigns an unclaimed coaster to the NPC.
    /// If no unclaimed coasters are available, it logs a warning.
    /// If there are unclaimed coasters, it randomly selects one and claims it for the NPC
    /// </summary>
    /// <param name="npc"></param>
    public GameObject assignCoaster(GameObject npc)
    {
        //Debug.Log("claiming a coaster for " + npc.name);
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
        //Debug.Log("Spawned NPC: " + npc.name);



    }
    System.Collections.IEnumerator NPCManagment()
    {
        //Debug.Log("Starting NPC management");
        while (spawning)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second before checking again
            //Debug.Log("spawning");
            while (npcs.Count < maxNPC)
            {
                //Debug.Log("not at max count");
                yield return new WaitForSeconds(Random.Range(minNpcSpawnDelay, maxNpcSpawnDelay)); // Wait for __ seconds before spawning a new NPC
                if (!npcSpawnPoint.GetComponent<NavPoint>().claimed && unclaimedCoasters.Count > 0) // Check if the spawn point is not claimed and there are unclaimed coasters
                {
                    todayNPCsSpawned++;
                    if (todayNPCsSpawned > npcsToday && currentTime == timeOfDay.Day)
                    {
                        spawning = false;
                        //Debug.Log("Finished spawning for the day");
                        break;
                    }
                    SpawnNPC();
                    // Exit the inner while loop to recheck conditions
                }

            }
        }
        while (npcs.Count > 0)
        {
            yield return new WaitForSeconds(5f); // Wait for 1 second before checking again
        }
        // Transition to night or reset for next day
        if (currentTime == timeOfDay.Day)
        {
            currentTime = timeOfDay.Night;
            //Debug.Log("Transitioning to Night");
            // Additional night-time logic here
            dayNightButton.SetActive(true);
            SpawnStore();
        }
        else
        {
            Debug.LogError("NPC management ended but it is already night. This should not happen.");
        }

    }
    void IButtonInteractor.Activate()
    {
        if (currentTime == timeOfDay.Night)
        {
            day++;
            currentTime = timeOfDay.Day;
            //Debug.Log("Starting Day " + day);
            //Edit math to make days not get too crazy long
            npcsToday = Mathf.RoundToInt(startingNPCs * Mathf.Pow(NPCsScaleFactor, day));
            //Edit math to make NPCs not get too crazy hard
            difficultyScale *= day;
            todayNPCsSpawned = 0;
            spawning = true;
            store.CloseShop();
            store = null;
            Invoke("turnOffDayButton", 3f); // Hide the day/night button after a short delay

            StartCoroutine(NPCManagment()); // Restart managing NPCs for the new day
        }
        else
        {
            Debug.LogError("Day/Night button activated but it is already day. This should not happen.");
        }
    }
    public void turnOffDayButton()
    {
        dayNightButton.SetActive(false);
    }
    void SpawnStore()
    {
        if (store != null) Debug.LogError("Spawning a new store but there is already a store");
        store = Instantiate(storePrefab, storeSpawnPoint).GetComponent<Store>();
        
    }
}
