using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    public List<CurrencyAmountPair> costs;

    [SerializeField]
    GameObject itemToPurchase;

    Interaction interaction;
    UnityEvent onPurchase;

    // Start is called before the first frame update
    void Start()
    {
        interaction = gameObject.GetComponent<Interaction>();

        interaction.InteractionText = "Purchase" + (itemToPurchase != null ? " "+itemToPurchase.name+" " : "") + "\nCosts: " + string.Join(" and ", costs);
        interaction.interacted.AddListener(AttemptPurchase);

        onPurchase = new UnityEvent();
    }

    void AttemptPurchase(Player player)
    {
        if (player.inventory.TryTakeCurrencies(costs))
        {
            onPurchase.Invoke();
            if (itemToPurchase != null) {
                GameObject item = Instantiate(itemToPurchase);
                player.inventory.items.Add(item);
            }
        }
    }
}