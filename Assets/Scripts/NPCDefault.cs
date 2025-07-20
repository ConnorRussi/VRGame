using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NPCDefault", menuName = "Scriptable Objects/NPCDefault")]
public class NPCDefault : ScriptableObject
{
    public float maxHealth;
    public float angerIncrement, wrongDrinkIncrement, maxAngerCap, minAngerCap;
    
    
}