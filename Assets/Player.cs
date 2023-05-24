using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Character character;
    public Inventory inventory;
    public UIManager ui;
    private int hitPoints = 3;

    public List<InputAction> hotbarBindings;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= 9; ++i) {
            InputAction action = new InputAction("Slot" + i, InputActionType.Button, "<Keyboard>/" + i);
            action.performed += TryEquip;
            action.Enable();
            hotbarBindings.Add(action);
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void TryEquip(InputAction.CallbackContext context)
    {
        int slot = Int32.Parse(context.action.name.Substring(4));
        if (inventory != null) {
            GameObject item = inventory.items[slot-1];
            if (item != null) {
                character.EquipItem(item);
            }
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

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Canceled) {
            return;
        }
        Interaction interaction = CheckInteraction(character.cameraTransform);
        if (interaction) {
            interaction.interacted.Invoke(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        }
        if (inventory != null)
        {
            ui.UpdateCurrency(inventory.currenciesHeld);
            ui.UpdateHotbar(inventory.items, character.equippedItem);
        }
    }

    public int getHitPoints() {
        return hitPoints;
    }

    public void setHitPoints(int newHp) {
        hitPoints = newHp;
    }
}
