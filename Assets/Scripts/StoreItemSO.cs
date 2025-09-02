using UnityEngine;

[CreateAssetMenu(menuName = "Store/StoreItem")]
public class StoreItemSO : ScriptableObject
{
    public string itemName;
    public GameObject prefab;
    public int cost;
    public bool topItem; // True if this item should spawn in the top row
    //public Sprite icon;
}