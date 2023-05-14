using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    [SerializeField]
    public string InteractionText;

    public UnityEvent<Player> interacted;
    private void Start()
    {
        if (interacted == null)
        {
            interacted = new UnityEvent<Player>();
        }
    }
    public void Interact(Player player)
    {
        interacted.Invoke(player);
    }
}
