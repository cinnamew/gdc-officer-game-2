using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    [SerializeField] float damage = 0.2f;
    [SerializeField] float hitCooldown = 1.0f;
    [SerializeField] float hitRange;
    [SerializeField] LayerMask hittable; //bad variable name lol
    float lastHitTime;

    void TryHit() {
        if (Time.time - lastHitTime < hitCooldown) {
            // too fast!
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, hitRange)) {
            Health health = hit.collider.gameObject.GetComponent<Health>();
            if (health != null) {
                lastHitTime = Time.time;
                health.TakeDamage(damage);
            }
        }
    }

    public void OnPrimaryUse(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Canceled) {
            TryHit();
        }
    }
}
