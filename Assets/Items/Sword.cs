using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Sword : NetworkBehaviour
{
    [SerializeField] float damage = 0.2f;
    [SerializeField] float hitCooldown = 1.0f;
    [SerializeField] float hitRange;
    [SerializeField] LayerMask hittable; //bad variable name lol
    float lastHitTime;

    [Command]
    void TryHit(GameObject hitObj) {
        if (Time.time - lastHitTime < hitCooldown) {
            // too fast!
            return;
        }
        Health health = hitObj.GetComponent<Health>();
        if (health != null) {
            lastHitTime = Time.time;
            health.TakeDamage(damage);
        }
    }

    [Client]
    public void OnPrimaryUse(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Canceled) {
            RaycastHit hit;
            // since hit check on client, a hacker can hit people across the map, but there shouldn't be any hackers anyway :)
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, hitRange)) {
                TryHit(hit.collider.gameObject);
            }
        }
    }
}
