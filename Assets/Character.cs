using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Reference to the player this character is being controlled by
    public Player owner;
    // Reference to camera transform
    [SerializeField]
    Transform cameraTransform;

    // Keep track of how much currency the character is holding
    [SerializeField]
    Dictionary<CurrencyType, int> currenciesHeld;

    public void AddCurrency(CurrencyType currencyType, int amt)
    {
        currenciesHeld[currencyType] += amt;
    }

    // Start is called before the first frame update
    void Start()
    {
        currenciesHeld = new Dictionary<CurrencyType, int>();
        foreach (int i in System.Enum.GetValues(typeof(CurrencyType)))
        {
            currenciesHeld.Add((CurrencyType)i, 0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
