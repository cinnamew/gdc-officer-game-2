using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    public List<CurrencyAmountPair> costs;

    [SerializeField]
    GameObject itemToPurchase;

    Interaction interaction;

    // Start is called before the first frame update
    void Start()
    {
        interaction = gameObject.GetComponent<Interaction>();

        interaction.InteractionText = "Purchase\nCosts: " + string.Join(" and ", costs);
        interaction.interacted.AddListener(AttemptPurchase);
    }

    void AttemptPurchase(Player player)
    {
        if (player.inventory.TryTakeCurrencies(costs))
        {
            GameObject item = Instantiate(itemToPurchase);
            player.inventory.items.Add(item);
        }
    }
}