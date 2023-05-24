using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    // Reference to camera transform
    [SerializeField]
    public Transform cameraTransform;

    [SerializeField]
    Transform equipTransform;
    public GameObject equippedItem = null;

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
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputDir2D = context.ReadValue<Vector2>();
        inputDir = new Vector3(inputDir2D.x, 0, inputDir2D.y);
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        Vector2 turnDir = context.ReadValue<Vector2>();
        lookAngle.x = (lookAngle.x + turnDir.x * Time.deltaTime * turnSpeed) % 360.0f;
        lookAngle.y = Mathf.Clamp(lookAngle.y - turnDir.y * Time.deltaTime * turnSpeed, MIN_PITCH, MAX_PITCH);
    }

    void UnequipItem() {
        equippedItem.SetActive(false);
        equippedItem = null;
    }

    public void EquipItem(GameObject item) {
        GameObject prevEquipped = equippedItem;
        if (equippedItem) {
            UnequipItem();
            if (item == prevEquipped) {
                // allows unequipping of everything
                return;
            }
        }
        Item itemComp = item.GetComponent<Item>();
        item.transform.SetParent(equipTransform, false);
        itemComp.owner = this;
        item.SetActive(true);
        equippedItem = item;
    }
}
