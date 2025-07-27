using Unity.VisualScripting;
using UnityEngine;

public class Cup : MonoBehaviour
{
    public GameObject drink, iceGO, mintGO, lemonGO;
    public Renderer drinkRenderer;

    //These may be able to go to a scriptable object
    public float maxDrinkHeight = 1.0f; // Maximum scale for the y cordinate of the drink
    public float drinkHeightIncrement = 0.01f; // Increment for the height of the drink

    
    public float currDrinkHeight = 0.0f; // Current scale for the y cordinate of the drink

    //these are used to maintain the origonal x and z scale of the drink
    //so that the drink does not stretch when the height is changed
    float defaulXScale,defaulZScale;


    public bool Dump; //empties the cup
    public bool ice, mint, lemon;
    public enum CupType
    {
        Ice,
        Mint,
        Lemon
    }
    public Color color;


    void Start()
    {
        if (drink == null)
        {
            Debug.LogError(gameObject.name + " has no drink GameObject assigned.");
        }
        defaulXScale = drink.transform.localScale.x;
        defaulZScale = drink.transform.localScale.z;
        drink.transform.localScale = new Vector3(defaulXScale, currDrinkHeight, defaulZScale);
        
    }
    /// <summary>
    /// checks if the Dump flag is set to true
    /// if it is, it empties the cup and resets the Dump flag
    /// </summary>
    void FixedUpdate()
    {
        if (Dump)
        {
            EmptyCup();
            Dump = false; // Reset the Dump flag after emptying the cup
        }
    }
    /// <summary>
    /// fills cup a little and updates the color of the drink based on the color passed in
    /// This method is called by the DrinkParticle script when a particle collides with the cup
    /// </summary>
    /// <param name="color"></param>
    public void FillCup(Color color)
    {
        //Debug.Log("Filling cup with color: " + color);
        //Debug.Log("Old color: " + this.color);
        ChangeColor(color);
        currDrinkHeight += drinkHeightIncrement;
        currDrinkHeight = Mathf.Min(currDrinkHeight, maxDrinkHeight); // Ensure we don't exceed the maximum height
        drink.transform.localScale = new Vector3(defaulXScale, currDrinkHeight, defaulZScale);

    }
    /// <summary>
/// Changes the color of the drink in the cup
/// This method is called by the DrinkParticle script when a particle collides with the cup
/// </summary>
/// <param name="color"></param>
    void ChangeColor(Color color)
    {
        //Debug.Log("Changing drink color to: " + color);
        //color.a = this.color.a; // Preserve the alpha value
        drinkRenderer.material.color = color;
        this.color = color;
    }
    /// <summary>
    /// dumps the cup
    /// </summary>
    public void EmptyCup()
    {
        currDrinkHeight = 0.0f;
        drink.transform.localScale = new Vector3(0, currDrinkHeight, 0);
    }
    public void GarnishChange(bool newValue, CupType cupType)
    {
        switch (cupType)
        {
            case CupType.Ice:
                iceGO.SetActive(newValue);
                break;
            case CupType.Mint:
                mintGO.SetActive(newValue);
                break;
            case CupType.Lemon:
                lemonGO.SetActive(newValue);
                break;
        }
    }
    
}
