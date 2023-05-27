using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Camera thumbnailCamera;
    [SerializeField]
    RenderTexture thumbnailRenderTexture;

    UIDocument document;
    VisualElement root;
    [SerializeField]
    VisualTreeAsset hotbarItemAsset;

    TextElement interactTooltip;
    VisualElement hotbar;
    VisualElement healthbar;
    TextElement currencyRegular;
    TextElement currencyPremium;

    Dictionary<GameObject, VisualElement> hotbarItems;
    Queue<GameObject> itemsToRender;
    GameObject beingRendered;
    GameObject renderClone;

    // Start is called before the first frame update
    void Start()
    {
        Camera.onPostRender += OnPostThumbnailRender;
        hotbarItems = new Dictionary<GameObject, VisualElement>();
        itemsToRender = new Queue<GameObject>();

        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        interactTooltip = root.Q<TextElement>("InteractionTooltip");
        hotbar = root.Q<VisualElement>("Hotbar");
        hotbar.Remove(hotbar.Q<VisualElement>("HotbarItem")); // by default we have a default instance of the template in the ui for previewing purposes
        healthbar = root.Q<VisualElement>("HealthbarHealth");
        currencyRegular = root.Q<VisualElement>("CurrencyRegular").Q<TextElement>();
        currencyPremium = root.Q<VisualElement>("CurrencyPremium").Q<TextElement>();
    }

    void OnPostThumbnailRender(Camera cam) {
        if (cam != thumbnailCamera) {
            return;
        }
        if (beingRendered) {
            Texture2D thumbnail = new Texture2D(thumbnailRenderTexture.width, thumbnailRenderTexture.height);
            thumbnail.ReadPixels(new Rect(0, 0, thumbnailRenderTexture.width, thumbnailRenderTexture.height), 0, 0);
            thumbnail.Apply();

            VisualElement respectiveHotbarElem = hotbarItems[beingRendered];
            if (respectiveHotbarElem != null) {
                respectiveHotbarElem.Q<VisualElement>("Image").style.backgroundImage = thumbnail;
            }

            Object.Destroy(renderClone);
            renderClone = null;
            beingRendered = null;
        }
    }

    // apparently Unity doesn't have a SetLayerRecursively or anything of the sort. just leaving this here
    static void SetLayerRecursively(GameObject obj, LayerMask layer) {
        obj.layer = layer;
        foreach(Transform child in obj.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    void Update() {
        if (beingRendered) {
            print("still waiting for result");
            return;
        }
        GameObject item;
        if (itemsToRender.TryDequeue(out item)) {
            renderClone = Object.Instantiate(item.GetComponent<Item>().visualObj, Vector3.zero, Quaternion.identity);
            SetLayerRecursively(renderClone, LayerMask.NameToLayer("ThumbnailRender"));
            beingRendered = item;

            RenderTexture.active = thumbnailRenderTexture;
            thumbnailCamera.Render();
        }
    }

    public void UpdateInteractionTooltip(string text)
    {
        interactTooltip.text = "[F]\n" + text;
    }

    public void HideInteractionTooltip()
    {
        interactTooltip.text = "";
    }

    public void UpdateCurrency(IDictionary<CurrencyType, int> amounts)
    {
        currencyRegular.text = amounts[CurrencyType.Regular].ToString();
        currencyPremium.text = amounts[CurrencyType.Premium].ToString();
    }

    public void UpdateHotbar(IList<GameObject> items, GameObject equippedItem)
    {
        foreach (GameObject item in items)
        {
            if (!hotbarItems.ContainsKey(item))
            {
                TemplateContainer container = hotbarItemAsset.Instantiate();
                hotbar.Insert(hotbar.childCount-1, container);
                itemsToRender.Enqueue(item);
                hotbarItems.Add(item, container);
            }
        }

        var tempDict = hotbarItems;
        foreach (KeyValuePair<GameObject, VisualElement> pair in hotbarItems)
        {
            int ind = items.IndexOf(pair.Key);
            if (ind != -1) {
                VisualElement hotbarItem = pair.Value.Q<VisualElement>("HotbarItem"); // apparently there is an extra root template container created, which is not part of the template
                hotbarItem.Q<TextElement>().text = (ind+1).ToString(); // a scuffed mapping from the slot to the respective keybind, assuming we only use number keys on the keyboard...
                hotbarItem.EnableInClassList("highlighted", pair.Key == equippedItem);
            } else {
                hotbar.Remove(pair.Value);
                tempDict.Remove(pair.Key);
            }
        }
        hotbarItems = tempDict;

        
    }
    
    public void UpdateHealth(float healthPercent) {
        healthbar.style.width = Length.Percent(healthPercent * 100.0f);
        healthbar.style.backgroundColor = Color.HSVToRGB(healthPercent / 3.0f, 0.6f, 0.7f);
    }
}
