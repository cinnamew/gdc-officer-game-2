using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Interaction : NetworkBehaviour
{
    [SerializeField]
    public string InteractionText;

    public UnityEvent<Character> interacted;
    private void Start()
    {
        if (interacted == null)
        {
            interacted = new UnityEvent<Character>();
        }
    }

    [Command(requiresAuthority = false)]
    public void Interact(NetworkConnectionToClient sender = null)
    {
        interacted.Invoke(sender.identity.GetComponent<Character>());
    }
}
