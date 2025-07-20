using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> coasters;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find all objects with the "Coaster" tag and add them to the coasters list
        coasters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Coaster"));
        foreach (GameObject coaster in coasters)
        {
            coaster.SetActive(false); // Deactivate all coasters initially
        }
        Debug.Log("all coasters turned off");
    }
    public void assignCoaster(NPC npc)
    {
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
        
   
}
