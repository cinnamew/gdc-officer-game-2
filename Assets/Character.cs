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
    [SyncVar(hook = nameof(OnEquippedChanged))] public GameObject equippedItem = null;

    new Rigidbody rigidbody;

    float walkSpeed = 5.0f;
    float turnSpeed = 10.0f;

    Vector3 inputDir = Vector3.zero;
    Vector2 lookAngle = Vector2.zero; // note: x is horizontal / yaw, y is vertical / pitch, contrary to unity euler angles convention

    const float MIN_PITCH = -85.0f;
    const float MAX_PITCH = 85.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        if (!isLocalPlayer) {
            cameraTransform.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = transform.TransformVector(inputDir); // transform the relative input direction to world space
        moveDir.y = 0; // shouldnt be necessary as long as the character doesn't rotate pitch-wise anyway but just incase
        moveDir.Normalize();

        rigidbody.velocity = moveDir * walkSpeed + rigidbody.velocity.y * Vector3.up;
        cameraTransform.localRotation = Quaternion.Euler(lookAngle.y, 0, 0);
        transform.rotation = Quaternion.Euler(0, lookAngle.x, 0);
    }

    // note: this component is attached to the same transform/gameobject as the character hitbox
    [Client]
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        Vector2 inputDir2D = context.ReadValue<Vector2>();
        inputDir = new Vector3(inputDir2D.x, 0, inputDir2D.y);
    }

    [Client]
    public void OnTurn(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        Vector2 turnDir = context.ReadValue<Vector2>();
        lookAngle.x = (lookAngle.x + turnDir.x * Time.deltaTime * turnSpeed) % 360.0f;
        lookAngle.y = Mathf.Clamp(lookAngle.y - turnDir.y * Time.deltaTime * turnSpeed, MIN_PITCH, MAX_PITCH);
    }

    [Server]
    public void UnequipItem() {
        equippedItem = null;
    }

    [Command]
    public void EquipItem(GameObject item) {
        GameObject prevEquipped = equippedItem;
        if (equippedItem) {
            UnequipItem();
            if (item == prevEquipped) {
                // allows unequipping of everything
                return;
            }
        }
        equippedItem = item;
        item.GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<Player>().connectionToClient);
    }

    void OnEquippedChanged(GameObject oldEquipped, GameObject newEquipped) {
        if (oldEquipped != null) {
            oldEquipped.SetActive(false);
        }
        if (newEquipped != null) {
            newEquipped.SetActive(true);

            newEquipped.transform.SetParent(equipTransform, false);

            Item itemComp = newEquipped.GetComponent<Item>();
            itemComp.owner = this;
            if (!isLocalPlayer) {
                // prevent our player from trying to perform input on items that arent ours
                PlayerInput input = newEquipped.GetComponent<PlayerInput>();
                if (input != null) {
                    input.enabled = false;
                }
            }
        }
    }
}
