using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NPCDefault", menuName = "Scriptable Objects/NPCDefault")]
public class NPCDefault : ScriptableObject
{
    public int maxHealth, iceValue, cherryValue, lemonValue;
    public float wrongDrinkIncrement, maxAngerCap, minAngerCap;
    
    
}