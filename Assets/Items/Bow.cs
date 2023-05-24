using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    [SerializeField]
    GameObject arrowAsset;
    Item item;

    float arrowLaunchSpeed = 20.0f; // initial arrow speed
    void Start()
    {
        item = GetComponent<Item>();
    }

    public void OnPrimaryUse(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) {
            GameObject arrow = Instantiate(arrowAsset, item.owner.cameraTransform.position, Quaternion.identity); // rotation will be set by arrow component anyway
            Arrow arrowComp = arrow.GetComponent<Arrow>();
            arrowComp.velocity = item.owner.cameraTransform.forward * arrowLaunchSpeed;
        }
    }
}
