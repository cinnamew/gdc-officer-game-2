using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Bow : NetworkBehaviour
{
    [SerializeField]
    GameObject arrowAsset;
    Item item;

    float arrowLaunchSpeed = 20.0f; // initial arrow speed
    void Start()
    {
        item = GetComponent<Item>();
    }

    [Command]
    void ShootArrow(Vector3 direction) {
        GameObject arrow = Instantiate(arrowAsset, item.owner.cameraTransform.position, Quaternion.identity); // rotation will be set by arrow component anyway
        NetworkServer.Spawn(arrow);

        Arrow arrowComp = arrow.GetComponent<Arrow>();
        arrowComp.initialVelocity = direction * arrowLaunchSpeed;
    }

    public void OnPrimaryUse(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) {
            ShootArrow(Camera.main.transform.forward);
        }
    }
}
