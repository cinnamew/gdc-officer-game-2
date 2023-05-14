using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Character character;
    public UIManager ui;

    // Start is called before the first frame update
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
    }

    Interaction CheckInteraction(Transform camera)
    {
        RaycastHit hit;
        Interaction interaction = null;
        if (Physics.Raycast(camera.position, camera.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            // this is kind of scuffed but just go up the hierarchy looking for an interaction, so we can have one interaction component cover a range of gameobjects
            Transform hitTransform = hit.transform;
            do {
                interaction = hitTransform.gameObject.GetComponent<Interaction>();
            } while (interaction == null && (hitTransform = hitTransform.parent));
        }

        return interaction;
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null)
        {
            Interaction interaction = CheckInteraction(character.cameraTransform);
            if (interaction == null)
            {
                ui.HideInteractionTooltip();
            }
            else
            {
                ui.UpdateInteractionTooltip(interaction.InteractionText);
            }
        }
    }
}
