using System;
using System.Collections.Generic;
using UnityEngine;

public class DrinkDataBase : MonoBehaviour
{
    public enum CupType { ShotGlass, Mug }

    public string[] drinkNames;
    public CupType[] cupTypes; // Use enum array instead of string[]
    public Color[] colors;
    public bool[] iceAllowed;
    public bool[] cherryAllowed;
    public bool[] lemonAllowed;

    [System.Serializable]
    public struct Drink
    {
        public string liquidName;
        public CupType cupType; // Use enum here
        public Color color;
        public bool iceAllowed, cherryAllowed, lemonAllowed;
    }

    public List<Drink> drinkList = new List<Drink>();

    void Start()
    {
        for (int i = 0; i < drinkNames.Length; i++)
        {
            Drink newDrink = new Drink
            {
                liquidName = drinkNames[i],
                cupType = cupTypes[i],
                color = colors[i],
                iceAllowed = iceAllowed[i],
                cherryAllowed = cherryAllowed[i],
                lemonAllowed = lemonAllowed[i]
            };
            drinkList.Add(newDrink);
        }
    }
}