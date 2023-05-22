using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    UIDocument document;
    VisualElement root;

    VisualTreeAsset hotbarItemAsset;

    TextElement interactTooltip;
    VisualElement hotbar;

    Dictionary<GameObject, VisualElement> hotbarItems;

    // Start is called before the first frame update
    void Start()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        hotbarItemAsset = Resources.Load<VisualTreeAsset>("HotbarItem");

        interactTooltip = root.Q<TextElement>("InteractionTooltip");
        hotbar = root.Q<VisualElement>("Hotbar");
        hotbar.Clear(); // by default we have a default instance of the template in the ui for previewing purposes
    }

    public void UpdateInteractionTooltip(string text)
    {
        interactTooltip.text = "[F]\n" + text;
    }

    public void HideInteractionTooltip()
    {
        interactTooltip.text = "";
    }

    public void UpdateCurrency(Dictionary<CurrencyType, int> amounts)
    {

    }

    public void UpdateHotbar(List<GameObject> items)
    {
        foreach (GameObject item in items)
        {
            if (!hotbarItems.ContainsKey(item))
            {
                TemplateContainer container = hotbarItemAsset.Instantiate();
                //container.Q<VisualElement>("Image").style.backgroundImage;
                hotbarItems.Add(item, container);

            }
        }
    }
}
