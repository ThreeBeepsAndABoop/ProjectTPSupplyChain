using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    public MachineWindow window;
    public MachineIO rejectOutput;
    public bool canAccept = true;

    public ConveyorSectorColor color = ConveyorSectorColor.black;
    protected Box box;

    public virtual void ObjectWasSchloinked(GameObject go)
    {
        box = go.GetComponent<Box>();
        if (ShouldRejectEnteringBox(box))
        {
            // reject the box
            rejectOutput.YeetObject(go);
        } else
        {
            window.DisplayObject(go);
        }
    }

    public virtual bool ShouldRejectEnteringBox(Box b)
    {
        return box.Task.sorted && box.Task.sectorColor != color || !box.Task.sorted && color != ConveyorSectorColor.black || !box.Task.IsPacked();
    }

    public virtual void ObjectWasDisplayed(GameObject go)
    {
    }

    public virtual void ObjectWasSchlorped(GameObject go)
    {
        box = null;
        canAccept = true;
    }

    public virtual void ObjectWasYeeted(GameObject go)
    {
        box = null;
        canAccept = true;
    }

    public virtual void ObjectWasRemovedFromDisplay(GameObject go)
    {
        if (targetOutput == null)
        {
            return;
        }

        if (targetOutput == rejectOutput)
        {
            targetOutput.YeetObject(box.gameObject);
        }
        else
        {
            BoxAccepted(box);
            targetOutput.SchlorpObject(box.gameObject);
        }
    }

    public virtual void BoxAccepted(Box box)
    {

    }

    protected MachineIO targetOutput;
    public virtual void ValidateBox()
    {
        if (!box) { return; }
        window.RemoveObjectFromDisplay();
    }
}
