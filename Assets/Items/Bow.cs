using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Update()
    {
        
    }

    public void PrimaryUse()
    {
        GameObject arrow = Instantiate(arrowAsset, item.owner.cameraTransform.position, Quaternion.identity); // rotation will be set by arrow component anyway
        Arrow arrowComp = arrow.GetComponent<Arrow>();
        arrowComp.velocity = item.owner.cameraTransform.forward * arrowLaunchSpeed;
    }
}
