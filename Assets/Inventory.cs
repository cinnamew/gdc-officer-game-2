using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Keep track of how much currency the character is holding
    public Dictionary<CurrencyType, int> currenciesHeld;
    // List of items
    public List<GameObject> items;

    private void Start()
    {
        currenciesHeld = new Dictionary<CurrencyType, int>();
        foreach (int i in System.Enum.GetValues(typeof(CurrencyType)))
        {
            currenciesHeld.Add((CurrencyType)i, 0);
        }
    }

    public void AddCurrency(CurrencyType currencyType, int amt)
    {
        currenciesHeld[currencyType] += amt;
    }

    public bool TryTakeCurrencies(List<CurrencyAmountPair> toTake)
    {
        foreach (CurrencyAmountPair pair in toTake)
        {
            if (currenciesHeld[pair.currencyType] < pair.amount)
            {
                return false;
            }
        }

        // we have enough of each currency
        foreach (CurrencyAmountPair pair in toTake)
        {
            currenciesHeld[pair.currencyType] -= pair.amount;
        }
        return true;
    }
}
