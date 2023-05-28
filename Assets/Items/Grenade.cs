using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float delay;  
    [SerializeField] float explosionForce; 
    [SerializeField] float explosionRadius = 5f; 
    [SerializeField] GameObject explosionEffect; // use this for an explosion effect later if were doing that

    bool hasExploded = false; 

    void Start()
    {
        Invoke("Explode", delay);
    }

    void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                nearbyObject.gameObject.GetComponent<Health>().TakeDamage(0.5f);
            }
        }

        Destroy(gameObject);
    }
}
