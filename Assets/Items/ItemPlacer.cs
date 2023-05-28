using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class ItemPlacer : NetworkBehaviour
{
    [SerializeField]
    GameObject trapAsset;
    [SerializeField]
    Material placementMaterial;
    Item item;
    GameObject visualClone;

    float placementRange = 3.0f; // distance from our camera that we can place something at
    void Start()
    {
        item = GetComponent<Item>();
        if (isClient)
        {
            visualClone = Instantiate(item.visualObj); // for a placement preview
            Renderer[] renderers = visualClone.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material = placementMaterial;
            }
        }
    }

    // shorthand for passing in camera args and stuff to Physics.Raycast
    bool GetPlacementHit(out RaycastHit hit)
    {
        RaycastHit result;
        bool success = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out result, placementRange) && result.rigidbody == null;
        hit = result;
        return success;
    }

    void Update()
    {
        if (visualClone != null)
        {
            RaycastHit hit;
            if (item.owner && GetPlacementHit(out hit))
            {
                visualClone.SetActive(true);
                visualClone.transform.SetPositionAndRotation(hit.point, Quaternion.identity);
            } else
            {
                visualClone.SetActive(false);
            }
        }
    }

    [Command]
    void PlaceTrap(Vector3 position)
    {
        GameObject trap = Instantiate(trapAsset, position, Quaternion.identity);
        NetworkServer.Spawn(trap);

        item.owner.UnequipItem();
        item.owner.GetComponent<Inventory>().items.Remove(gameObject);
        Object.Destroy(gameObject);
    }

    public void OnPrimaryUse(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            RaycastHit hit;
            if (GetPlacementHit(out hit))
            {
                Object.Destroy(visualClone);
                visualClone = null;
                PlaceTrap(hit.point);
            }
        }
    }
}
