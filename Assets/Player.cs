using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar] public Character character;
    [SerializeField] GameObject uiAsset;
    public UIManager ui;

    public List<InputAction> hotbarBindings;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer) {
            GameObject uiPrefab = Instantiate(uiAsset);
            ui = uiPrefab.GetComponent<UIManager>();
            for (int i = 1; i <= 9; ++i) {
                InputAction action = new InputAction("Slot" + i, InputActionType.Button, "<Keyboard>/" + i);
                action.performed += TryEquip;
                action.Enable();
                hotbarBindings.Add(action);
            }
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    [Client]
    private void TryEquip(InputAction.CallbackContext context)
    {
        int slot = Int32.Parse(context.action.name.Substring(4));
        GameObject item = character.GetComponent<Inventory>().items[slot-1];
        if (item != null) {
            character.EquipItem(item);
        }
    }

    Interaction CheckInteraction(Transform camera)
    {
        RaycastHit hit;
        Interaction interaction = null;
        if (Physics.Raycast(camera.position, camera.TransformDirection(Vector3.forward), out hit, 2.0f))
        {
            // this is kind of scuffed but just go up the hierarchy looking for an interaction, so we can have one interaction component cover a range of gameobjects
            Transform hitTransform = hit.transform;
            do {
                interaction = hitTransform.gameObject.GetComponent<Interaction>();
            } while (interaction == null && (hitTransform = hitTransform.parent));
        }

        return interaction;
    }

    [Client]
    public void OnInteract(InputAction.CallbackContext context) {
        if (!isLocalPlayer) return;
        if (context.phase != InputActionPhase.Canceled) {
            return;
        }
        Interaction interaction = CheckInteraction(character.cameraTransform);
        if (interaction) {
            interaction.Interact();
        }
    }

    /*[TargetRpc]
    void ReplicateHealth(float health) {
        ui.UpdateHealth(health);
    }*/

    // Update is called once per frame
    void Update()
    {
        /*if (isServer) {
            ReplicateHealth(character.GetComponent<Health>().health);
        }*/
        if (!isLocalPlayer) return;
        if (character != null)
        {
            Interaction interaction = CheckInteraction(character.cameraTransform);
            if (interaction == null)
            {
                ui.HideInteractionTooltip();
            }
            else
            {
                ui.UpdateInteractionTooltip(interaction.InteractionText);
            }
            ui.UpdateHealth(character.GetComponent<Health>().health);
            ui.UpdateCurrency(character.GetComponent<Inventory>().currenciesHeld);
            ui.UpdateHotbar(character.GetComponent<Inventory>().items, character.equippedItem);
        }
    }
}
