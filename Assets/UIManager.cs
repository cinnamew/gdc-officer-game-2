using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    TextElement interactTooltip;
    // Start is called before the first frame update
    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;
        interactTooltip = root.Q<TextElement>("InteractionTooltip");
    }

    public void UpdateInteractionTooltip(string text)
    {
        interactTooltip.text = "[F]\n" + text;
    }

    public void HideInteractionTooltip()
    {
        interactTooltip.text = "";
    }
}
