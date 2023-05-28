using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlacedBearTrap : NetworkBehaviour
{
    public float damage = 0.3f; // amount of damage when triggered
    public float lifetime = 120.0f; // how long before this trap despawns

    void Start() {
        if (isServer)
        {
            Object.Destroy(gameObject, lifetime);
        }
    }

    [ClientRpc]
    void OnActivate()
    {
        GetComponentInChildren<Animator>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        this.enabled = false;
        OnActivate();
        Object.Destroy(gameObject, 1.0f);
    }
}
