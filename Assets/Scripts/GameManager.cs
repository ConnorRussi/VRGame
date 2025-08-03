using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject MugSpawnHolder, GlassSpawnHolder, bottleSpawnHolder;
    public GameObject MugPrefab, glassPrefab;
    public List<GameObject> bottles;
    public List<GameObject> bottleSpawns;
    public List<GameObject> coasters;
    public List<GameObject> mugSpawns;
    public List<GameObject> glassSpawns;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindAllCoasters();
        FindAllSpawns(MugSpawnHolder, MugPrefab);
        FindAllSpawns(GlassSpawnHolder, glassPrefab);
        SpawnBottles();
    }
    /// <summary>
    /// Finds all coasters in the scene and stores them in the coasters list.
    /// Initially, all coasters are deactivated.
    /// </summary>
    void FindAllCoasters()
    {
        coasters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Coaster"));
        foreach (GameObject coaster in coasters)
        {
            coaster.SetActive(false); // Deactivate all coasters initially
        }
        Debug.Log("all coasters turned off");
    }
    /// <summary>
    /// Assigns an unclaimed coaster to the NPC.
    /// If no unclaimed coasters are available, it logs a warning.
    /// If there are unclaimed coasters, it randomly selects one and claims it for the NPC
    /// </summary>
    /// <param name="npc"></param>
    public void assignCoaster(NPC npc)
    {
        //*****&Later if want to save some performance can use a order of coasters to assign instead of random
        if (coasters.Count == 0)
        {
            Debug.LogWarning("No coasters available to assign.");
            return;
        }

        // Find an unclaimed coaster
        // foreach (GameObject coaster in coasters)
        // {
        //     Coaster coasterComponent = coaster.GetComponent<Coaster>();
        //     if (coasterComponent != null && !coasterComponent.claimed)
        //     {
        //         coasterComponent.Claim(npc);
        //         //npc.currentCoaster = coasterComponent;
        //         Debug.Log("Assigned coaster: " + coaster.name + " to NPC: " + npc.name);
        //         return;
        //     }
        // }
        List<GameObject> unclaimedCoasters = new List<GameObject>();
        foreach (GameObject coaster in coasters)
        {
            Coaster coasterComponent = coaster.GetComponent<Coaster>();
            if (coasterComponent != null && !coasterComponent.claimed)
            {
                unclaimedCoasters.Add(coaster);
            }
        }

        if (unclaimedCoasters.Count == 0)
        {
            Debug.LogWarning("All coasters are claimed.");
            return;
        }

        // Assign a random unclaimed coaster
        int randomIndex = Random.Range(0, unclaimedCoasters.Count);
        GameObject chosenCoaster = unclaimedCoasters[randomIndex];
        Coaster chosenComponent = chosenCoaster.GetComponent<Coaster>();
        chosenComponent.Claim(npc);
    }
    void FindAllSpawns(GameObject spawnHolder, GameObject prefab)
    {
        foreach (Transform child in spawnHolder.transform)
        {
            mugSpawns.Add(child.gameObject);
            if (prefab == null) continue; // Skip if no prefab is provided for bottles
            Instantiate(prefab, child.position, child.rotation);
        }
    }
    void SpawnBottles()
    {
        for(int i = 0; i < bottleSpawns.Count; i++)
        {
            if (i < bottles.Count)
            {
                Instantiate(bottles[i], bottleSpawns[i].transform.position, bottleSpawns[i].transform.rotation);
            }
            else
            {
                Debug.LogWarning("Not enough bottles to spawn at all bottle spawns.");
                break;
            }
        }
    }
     
   
}
