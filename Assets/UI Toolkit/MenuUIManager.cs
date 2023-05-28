using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Mirror;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] NewNetworkManager networkManager;

    UIDocument document;
    VisualElement root;

    VisualElement playMenuExpander;
    bool playMenuOpen = false;
    Button playMenuButton;
    Button hostButton;
    Button joinButton;
    TextField serverField;

    Button optionsButton;

    void Start()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        playMenuExpander = root.Q<VisualElement>("PlayExpander");
        playMenuButton = root.Q<Button>("PlayButton");
        playMenuButton.clicked += TogglePlayMenu;
        hostButton = root.Q<Button>("HostButton");
        hostButton.clicked += TryHost;
        joinButton = root.Q<Button>("JoinButton");
        joinButton.clicked += TryJoin;
        serverField = root.Q<TextField>("ServerField");

        optionsButton = root.Q<Button>("OptionsButton");
    }

    void TryHost() {
        networkManager.networkAddress = serverField.value;
        networkManager.StartHost();
    }

    void TryJoin() {
        networkManager.networkAddress = serverField.value;
        networkManager.StartClient();
    }

    void TogglePlayMenu () {
        playMenuOpen = !playMenuOpen;
        playMenuExpander.EnableInClassList("play_menu_opened", playMenuOpen);
    }
}
