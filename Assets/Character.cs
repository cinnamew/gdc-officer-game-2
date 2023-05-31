using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Character : NetworkBehaviour
{
    // Reference to camera transform
    [SerializeField]
    public Transform cameraTransform;

    [SerializeField]
    Transform equipTransform;
    [SyncVar(hook = nameof(OnEquippedChanged))] public uint equippedItem = 0;

    [SerializeField] GameObject uiAsset;
    UIManager ui;

    public List<InputAction> hotbarBindings;

    new Rigidbody rigidbody;
    Health health;
    Inventory inv;
    float deathTime = 0.0f;

    float walkSpeed = 5.0f;
    float turnSpeed = 10.0f;

    Vector3 inputDir = Vector3.zero;
    Vector2 lookAngle = Vector2.zero; // note: x is horizontal / yaw, y is vertical / pitch, contrary to unity euler angles convention

    const float MIN_PITCH = -85.0f;
    const float MAX_PITCH = 85.0f;

    Vector3 startPosition;
    Quaternion startRotation;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        inv = GetComponent<Inventory>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        if (isLocalPlayer) {
            GameObject uiPrefab = Instantiate(uiAsset);
            ui = uiPrefab.GetComponent<UIManager>();
            for (int i = 1; i <= 9; ++i) {
                InputAction action = new InputAction("Slot" + i, InputActionType.Button, "<Keyboard>/" + i);
                action.performed += TryEquip;
                action.Enable();
                hotbarBindings.Add(action);
            }
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            cameraTransform.gameObject.SetActive(false);
        }
        if (isServer) {
            health.onDeath.AddListener(Die);
        }
    }

    void Die() {
        deathTime = Time.time;
    }

    void Update()
    {
        if (health.dead && (Time.time - deathTime > 10.0f)) {
            health.health = 1.0f;
            health.dead = false;
            transform.SetPositionAndRotation(startPosition, startRotation);
        }
        if (isLocalPlayer) {
            Vector3 moveDir = transform.TransformVector(inputDir); // transform the relative input direction to world space
            moveDir.y = 0; // shouldnt be necessary as long as the character doesn't rotate pitch-wise anyway but just incase
            moveDir.Normalize();

            if (health.dead) moveDir = Vector3.zero;

            rigidbody.velocity = moveDir * walkSpeed + rigidbody.velocity.y * Vector3.up;

            Interaction interaction = CheckInteraction(cameraTransform);
            if (interaction == null)
            {
                ui.HideInteractionTooltip();
            }
            else
            {
                ui.UpdateInteractionTooltip(interaction.InteractionText);
            }
            ui.UpdateHealth(health.health);
            ui.UpdateCurrency(inv.currenciesHeld);
            ui.UpdateHotbar(inv.items, equippedItem == 0 ? null : NetworkClient.spawned[equippedItem].gameObject);
        }
        cameraTransform.localRotation = Quaternion.Euler(lookAngle.y, 0, 0);
        transform.rotation = Quaternion.Euler(0, lookAngle.x, 0);
    }

    [Client]
    private void TryEquip(InputAction.CallbackContext context)
    {
        int slot = Int32.Parse(context.action.name.Substring(4));
        if ((slot-1) < inv.items.Count) {
            GameObject item = inv.items[slot-1];
            EquipItem(item.GetComponent<NetworkIdentity>().netId);
            
            PlayerInput input = item.GetComponent<PlayerInput>();
            if (input != null) {
                input.enabled = true;
            }
        }
    }

    Interaction CheckInteraction(Transform camera)
    {
        RaycastHit hit;
        Interaction interaction = null;
        if (Physics.Raycast(camera.position, camera.TransformDirection(Vector3.forward), out hit, 2.0f))
        {
            // this is kind of scuffed but just go up the hierarchy looking for an interaction, so we can have one interaction component cover a range of gameobjects
            Transform hitTransform = hit.transform;
            do {
                interaction = hitTransform.gameObject.GetComponent<Interaction>();
            } while (interaction == null && (hitTransform = hitTransform.parent));
        }

        return interaction;
    }

    [Client]
    public void OnInteract(InputAction.CallbackContext context) {
        if (!isLocalPlayer) return;
        if (context.phase != InputActionPhase.Canceled) {
            return;
        }
        Interaction interaction = CheckInteraction(cameraTransform);
        if (interaction) {
            interaction.Interact();
        }
    }

    // note: this component is attached to the same transform/gameobject as the character hitbox
    [Client]
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        Vector2 inputDir2D = context.ReadValue<Vector2>();
        inputDir = new Vector3(inputDir2D.x, 0, inputDir2D.y);
    }

    // Command & ClientRpc needed to get around not being able to set syncdirection per syncvar...
    // Sync character rotation state (note: no interpolation performed)
    [Command]
    void ReplicateTurn(Vector2 newLook) {
        ReceiveTurn(newLook);
    }
    [ClientRpc(includeOwner = false)]
    void ReceiveTurn(Vector2 newLook) {
        lookAngle = newLook;
    }

    [Client]
    public void OnTurn(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        Vector2 turnDir = context.ReadValue<Vector2>();
        lookAngle.x = (lookAngle.x + turnDir.x * Time.deltaTime * turnSpeed) % 360.0f;
        lookAngle.y = Mathf.Clamp(lookAngle.y - turnDir.y * Time.deltaTime * turnSpeed, MIN_PITCH, MAX_PITCH);
        ReplicateTurn(lookAngle);
    }

    [Server]
    public void UnequipItem() {
        equippedItem = 0;
    }

    [Command]
    public void EquipItem(uint item) {
        ulong prevEquipped = equippedItem;
        if (equippedItem != 0) {
            UnequipItem();
            if (item == prevEquipped) {
                // allows unequipping of everything
                return;
            }
        }
        equippedItem = item;
        NetworkServer.spawned[item].AssignClientAuthority(GetComponent<Character>().connectionToClient);
    }

    IEnumerator OnEquippedChangedCoroutine(uint oldEquippedId, uint newEquippedId) {
        Dictionary<uint, NetworkIdentity> spawned = isClient ? NetworkClient.spawned : NetworkServer.spawned;
        if (oldEquippedId != 0) {
            while (!spawned.ContainsKey(oldEquippedId)) yield return null;
            GameObject oldEquipped = spawned[oldEquippedId].gameObject;
            oldEquipped.SetActive(false);
        }
        if (newEquippedId != 0) {
            while (!spawned.ContainsKey(newEquippedId)) yield return null;
            GameObject newEquipped = spawned[newEquippedId].gameObject;
            newEquipped.SetActive(true);
            
            newEquipped.transform.SetParent(equipTransform, false);

            Item itemComp = newEquipped.GetComponent<Item>();
            itemComp.owner = this;
        }
    }

    void OnEquippedChanged(uint oldEquippedId, uint newEquippedId) {
        StartCoroutine(OnEquippedChangedCoroutine(oldEquippedId, newEquippedId));
    }
}
