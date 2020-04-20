using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    Box = 0,
    Button = 1,
    Item = 2,
    ComponentSlot = 3,
}

// Todo, make subclasses of the script, rather than relying on the enum
public class Interactable : MonoBehaviour
{
    public InteractableType type;

    public virtual bool Interact(RaycastHit hitInfo)
    {
        switch(type)
        {
            case InteractableType.Button:
                transform.parent.GetComponent<WorldButton>().Press();
                return true;
            case InteractableType.ComponentSlot:
                GetComponent<ComponentSlot>().Interact();
                return true;
            default:
                return true;
        }
    }

    public virtual bool InteractSecondary(RaycastHit hitInfo)
    {
        switch (type)
        {
            default:
                return false;
        }
    }

    public virtual bool InteractTertiary(RaycastHit hitInfo)
    {
        switch (type)
        {
            case InteractableType.Box:
                transform.GetComponent<Box>().open = !transform.GetComponent<Box>().open;
                return true;
            default:
                return false;
        }
    }
}
