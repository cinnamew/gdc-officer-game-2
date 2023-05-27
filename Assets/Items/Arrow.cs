using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Arrow : NetworkBehaviour
{
    [SyncVar] public Vector3 initialVelocity;
    public float damage = 0.1f;
    float startTime;

    void Start() {
        startTime = Time.time;
    }

    [Server]
    void HitCheck(Vector3 displacement) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, displacement.normalized, out hit, displacement.magnitude)) {
            GameObject hitObj = hit.collider.gameObject;

            Health health = hitObj.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            this.enabled = false;
            transform.position = hit.point;
            Object.Destroy(gameObject, 30.0f);
            if (hitObj.GetComponent<NetworkIdentity>()) {
                transform.SetParent(hitObj.transform, true);
                ReplicateHit(transform.localPosition, transform.localRotation, hitObj);
            } else {
                ReplicateHit(transform.position, transform.rotation, null);
            }
            // if not synced by mirror by default make sure to implement this
        }
    }

    [ClientRpc]
    void ReplicateHit(Vector3 localPosition, Quaternion localRotation, GameObject parent) {
        this.enabled = false;
        if (parent) {
            transform.SetParent(parent.transform);
            transform.SetLocalPositionAndRotation(localPosition, localRotation);
        } else {
            transform.SetPositionAndRotation(localPosition, localRotation);
        }
    }

    void Update()
    {
        Vector3 velocity = initialVelocity + Physics.gravity * (Time.time - startTime);

        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPos = transform.position + displacement;
        if (isServer) {
            HitCheck(displacement);
        }
        transform.LookAt(newPos);
        transform.position = newPos;

    }
}
