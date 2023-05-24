using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Armor : MonoBehaviour
{
    public void OnPrimaryUse(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) {
            Character character = GetComponent<Item>().owner;
            character.GetComponent<Health>().armor = 0.5f;
            character.UnequipItem();
        }
    }
}
