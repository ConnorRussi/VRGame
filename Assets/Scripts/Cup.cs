using Unity.VisualScripting;
using UnityEngine;

public class Cup : MonoBehaviour
{
    public GameObject drink, iceGO, mintGO, lemonGO;
    public Renderer drinkRenderer;
    public float maxDrinkHeight = 1.0f; // Maximum scale for the y cordinate of the drink
    public float currDrinkHeight = 0.0f; // Current scale for the y cordinate of the drink
    public float drinkHeightIncrement = 0.01f; // Increment for the height of the drink
    float defaulXScale;
    float defaulZScale;
    public bool Dump;
    public bool ice, mint, lemon;
    public Color color;


    void Start()
    {
        if (drink == null)
        {
            Debug.LogError(gameObject.name + " has no drink GameObject assigned.");
        }
        defaulXScale = drink.transform.localScale.x;
        defaulZScale = drink.transform.localScale.z;
        drink.transform.localScale = new Vector3(0, currDrinkHeight, 0);
        IceChange(ice);
        MintChange(mint);
        LemonChange(lemon);

    }
    void FixedUpdate()
    {
        if (Dump)
        {
            EmptyCup();
            Dump = false; // Reset the Dump flag after emptying the cup
        }
    }
    public void FillCup(Color color)
    {
        ChangeColor(color);
        currDrinkHeight += drinkHeightIncrement;
        currDrinkHeight = Mathf.Min(currDrinkHeight, maxDrinkHeight); // Ensure we don't exceed the maximum height
        drink.transform.localScale = new Vector3(defaulXScale, currDrinkHeight, defaulZScale);

    }
    void ChangeColor(Color color)
    {
        color.a = this.color.a; // Preserve the alpha value
        drinkRenderer.material.color = color;
        this.color = color;
    }
    public void EmptyCup()
    {
        currDrinkHeight = 0.0f;
        drink.transform.localScale = new Vector3(0, currDrinkHeight, 0);
    }
    public void IceChange(bool iceChange)
    {
        ice = iceChange;
        iceGO.SetActive(ice);
        Debug.Log("Ice changed to: " + ice);
    }
    public void MintChange(bool mintChange)
    {
        mint = mintChange;
        mintGO.SetActive(mint);
        Debug.Log("Mint changed to: " + mint);
    }
    public void LemonChange(bool lemonChange)
    {
        lemon = lemonChange;
        lemonGO.SetActive(lemon);
        Debug.Log("Lemon changed to: " + lemon);
    }
}
