using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CurrencyType
{
    Regular,
    Premium
}

public class CurrencyGenerator : MonoBehaviour
{
    // Maximum amount of currency this generator can accumulate
    [SerializeField]
    int MAX_ACCUMULATED = 8;
    // Delay (seconds) between subsequent currency generation
    [SerializeField]
    float SPAWN_DELAY = .2f;
    // Type of currency
    [SerializeField]
    CurrencyType currencyType;

    // Amount of currency accumulated
    int accumulatedCount = 0;
    // Random object to do rng with
    System.Random rand = new System.Random();
    // Keep track of the time elapsed since last currency generation
    private float generationDT = 0;
    // List which stays updated with all character Inventories inside of this Trigger
    private List<Inventory> inventoriesInside = new List<Inventory>();

    void TransferCurrency(Inventory inv, int amt)
    {
        inv.AddCurrency(currencyType, amt);
    }

    bool TryGenerateCurrency()
    {
        if (accumulatedCount >= MAX_ACCUMULATED)
        {
            return false;
        }
        if (inventoriesInside.Count > 0)
        {
            TransferCurrency(inventoriesInside[rand.Next(inventoriesInside.Count)], 1);
        } else
        {
            accumulatedCount++;
        }
        return true;
    }

    void Update()
    {
        generationDT += Time.deltaTime;
        while (generationDT > SPAWN_DELAY)
        {
            generationDT -= SPAWN_DELAY;

            TryGenerateCurrency();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Inventory inv = other.gameObject.GetComponent<Inventory>();
        if (inv && !inventoriesInside.Contains(inv))
        {
            inventoriesInside.Add(inv);
            if (accumulatedCount > 0)
            {
                TransferCurrency(inv, accumulatedCount);
                accumulatedCount = 0;
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        Inventory inv = other.gameObject.GetComponent<Inventory>();
        if (inv && inventoriesInside.Contains(inv))
        {
            inventoriesInside.Remove(inv);
        }
    }
}
