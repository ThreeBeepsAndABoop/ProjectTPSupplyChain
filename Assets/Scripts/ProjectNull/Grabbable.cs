using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : Interactable
{
    public bool Grabbed = false;

    public override void Interact(RaycastHit hitInfo)
    {
        GameManager.Instance.GrabIt.Grab(hitInfo, this);
    }

    public override void InteractSecondary(RaycastHit hitInfo)
    {

        Debug.Log("Yeet");
        GameManager.Instance.GrabIt.Yeet(this);
    }
}
