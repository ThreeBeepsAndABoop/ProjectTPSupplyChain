using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    Box = 0,
    Button = 1,

    Item = 2,
}

public class Interactable : MonoBehaviour
{
    public InteractableType type;

    public virtual void Interact(RaycastHit hitInfo)
    {
        switch(type)
        {
            case InteractableType.Button:
                transform.parent.GetComponent<WorldButton>().Press();
                break;
            default:
                break;
        }
    }

    public virtual void InteractSecondary(RaycastHit hitInfo)
    {
        switch (type)
        {
            default:
                break;
        }
    }

    public virtual void InteractTertiary(RaycastHit hitInfo)
    {
        switch (type)
        {
            case InteractableType.Box:
                transform.GetComponent<Box>().open = !transform.GetComponent<Box>().open;
                break;
            default:
                break;
        }
    }
}
