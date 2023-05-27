using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Inventory : NetworkBehaviour
{
    // Keep track of how much currency the character is holding
    public readonly SyncDictionary<CurrencyType, int> currenciesHeld = new SyncDictionary<CurrencyType, int>();
    // List of items
    public readonly SyncList<GameObject> items = new SyncList<GameObject>();

    private void Start()
    {
        if (!isServer) return;
        foreach (int i in System.Enum.GetValues(typeof(CurrencyType)))
        {
            currenciesHeld.Add((CurrencyType)i, 0);
        }
    }

    public void AddCurrency(CurrencyType currencyType, int amt)
    {
        currenciesHeld[currencyType] += amt;
    }

    public bool TryTakeCurrencies(IList<CurrencyAmountPair> toTake)
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
