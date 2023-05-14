using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CurrencyAmountPair
{
    [SerializeField]
    CurrencyType currencyType;
    [SerializeField]
    int amount;

    public override string ToString()
    {
        return amount.ToString() + " " + currencyType.ToString();
    }
}

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    public List<CurrencyAmountPair> costs;

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

    }
}