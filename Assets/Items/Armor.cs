using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Armor : NetworkBehaviour
{
    [Command]
    void ApplyArmor() {
        Character character = GetComponent<Item>().owner;
        character.GetComponent<Health>().armor = 0.5f;
        character.UnequipItem();
    }
    
    [Client]
    public void OnPrimaryUse(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) {
            ApplyArmor();
        }
    }
}
