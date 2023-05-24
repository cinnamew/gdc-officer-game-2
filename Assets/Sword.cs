using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] float hitRange;
    [SerializeField] LayerMask hittable; //bad variable name lol
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, hitRange)) {
            Player p = hit.collider.gameObject.GetComponent<Player>();
            if (p != null) {
               p.setHitPoints(p.getHitPoints()-1); 
            }
        }
    }
}
