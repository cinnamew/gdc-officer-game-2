using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector3 velocity;
    public float damage = 0.1f;

    void Update()
    {
        velocity += Physics.gravity * Time.deltaTime;

        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPos = transform.position + displacement;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, displacement.normalized, out hit, displacement.magnitude)) {
            GameObject hitObj = hit.collider.gameObject;

            Health health = hitObj.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            transform.position = hit.point;
            transform.SetParent(hitObj.transform, true);
            this.enabled = false;
            Object.Destroy(gameObject, 30.0f);
        }
        transform.LookAt(newPos);
        transform.position = newPos;

    }
}
