using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float health = 1.0f; // health of this object as a proportion (out of 1)
    public UnityEvent onDeath;
    float regenDelay = 5.0f; // delay before this object can begin regenerating its health
    float regenRate = 0.05f; // health regenerated each second 
    
    float lastDamaged = 0.0f;
    bool dead = false;

    public void TakeDamage(float delta) {
        health = Mathf.Max(health - delta, 0.0f);
        lastDamaged = Time.time;

        if (health == 0.0f && !dead) {
            dead = true;
            onDeath.Invoke();
        }
    }

    void Start() {
        if (onDeath == null) {
            onDeath = new UnityEvent();
        }
    }

    void Update()
    {
        if (!dead && ((Time.time - lastDamaged) > regenDelay)) {
            health = Mathf.Min(health + regenRate*Time.deltaTime, 1.0f);
        }
    }
}
