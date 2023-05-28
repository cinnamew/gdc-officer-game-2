using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

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
        if (player.GetComponent<Inventory>().TryTakeCurrencies(costs))
        {
            onPurchase.Invoke();
            if (itemToPurchase != null) {
                GameObject item = Instantiate(itemToPurchase);
                NetworkServer.Spawn(item, player.connectionToClient);
                player.GetComponent<Inventory>().items.Add(item);
            }
        }
    }
}