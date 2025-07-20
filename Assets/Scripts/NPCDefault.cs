using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCDefault", menuName = "Scriptable Objects/NPCDefault")]
public class NPCDefault : ScriptableObject
{
    public float maxHealth;
    public float angerIncrement, angerCap, maxAngerCap, minAngerCap;
    //potential drinks linked with their adjacent cup types and color types
    public List<drink> drinkList;
    public struct drink
    {
        public string liquidName;
        public string cupType;
        public Color color;
        public bool iceAllowed, cherryAllowed, lemonAllowed;
    }
}
