﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : Interactable
{
    public bool Grabbed = false;
    public bool InPlayerInventory;

    public Sprite icon;

    public override void Interact(RaycastHit hitInfo)
    {
        //GameManager.Instance.GrabIt.Grab(hitInfo.distance, this);
        if(!InPlayerInventory)
        {
            GameManager.Instance.PlayerInventory.PickUp(this, hitInfo.distance);
        }
    }

    public override void InteractSecondary(RaycastHit hitInfo)
    {
        //GameManager.Instance.GrabIt.Yeet(this);
    }
}
