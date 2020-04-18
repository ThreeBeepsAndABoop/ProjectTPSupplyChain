using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Grabbable[] _inventory;

    public int SelectedItemIndex = 0;
    public int InventoryCapacity = 4;
    public int InventoryCount { get { return _inventory.Length; } }

    public Grabbable GetInventoryItem(int index)
    {
        if (index >= InventoryCount)
        {
            return null;
        }

        return _inventory[index];
    }

    public void SelectNextItem()
    {
        SelectItem((SelectedItemIndex + 1) % InventoryCapacity);
    }

    public void SelectPreviousItem()
    {
        SelectItem((SelectedItemIndex - 1) % InventoryCapacity);
    }

    private const float DEFAULT_GRAB_DISTANCE = 1;
    public bool SelectItem(int index)
    {
        return SelectItem(index, DEFAULT_GRAB_DISTANCE);
    }

    private bool SelectItem(int index, float distance)
    {
        Debug.Log("Select inventory at index " + index);
        if (index < 0 || index >= InventoryCount)
        {
            return false;
        }

        if (index != SelectedItemIndex) {
            DeselectSelectedItem();
        }
        
        SelectedItemIndex = index;
        Grabbable newlySelectedItem = GetSelectedItem();
        if (!newlySelectedItem)
        {
            return true;
        }

        if (!newlySelectedItem.Grabbed && GameManager.Instance.GrabIt.GrabbedObject == null)
        {
            GameManager.Instance.GrabIt.Grab(distance, newlySelectedItem);
        }

        return true;
    }

    private bool DeselectSelectedItem()
    {
        Debug.Log("Deselect inventory at index " + SelectedItemIndex);
        Grabbable selectedItem = GetSelectedItem();
        if (selectedItem == null)
        {
            return false;
        }

        // Loop forward to find next item to select
        int newSelectedItemIndex = 0;
        for (int i = 1; i < InventoryCapacity; i++)
        {
            int index = (SelectedItemIndex + i) % InventoryCapacity;
            if (GetInventoryItem(index) != null)
            {
                newSelectedItemIndex = index;
            }
        }
        SelectedItemIndex = newSelectedItemIndex;

        return true;
    }

    public Grabbable GetSelectedItem()
    {
        if (SelectedItemIndex < 0)
        {
            return null;
        } else
        {
            return _inventory[SelectedItemIndex];
        }
    }

    public bool PickUp(Grabbable grabbable, float distance)
    {
        int firstFreeIndex = -1;
        for (int i = 0; i < _inventory.Length; i++)
        {
            if (_inventory[i] == null)
            {
                firstFreeIndex = i;
                break;
            }
        }

        if(firstFreeIndex == -1)
        {
            // inventory is full
            return false;
        }

        Debug.Log("Inventory add " + grabbable + " at index " + firstFreeIndex);
        _inventory[firstFreeIndex] = grabbable;
        grabbable.InPlayerInventory = true;
        SelectItem(firstFreeIndex, distance);
        return true;
    }

    public bool Drop(Grabbable grabbable)
    {
        int index = -1;
        for (int i = 0; i < _inventory.Length; i++)
        {
            if (_inventory[i] == grabbable)
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
        {
            Debug.Log("Inventory remove " + grabbable + " at index " + index);
            if (grabbable == GetSelectedItem())
            {
                DeselectSelectedItem();
                if (GameManager.Instance.GrabIt.GrabbedObject == grabbable.gameObject)
                {
                    GameManager.Instance.GrabIt.ReleaseGrabbed();
                }
            }
            _inventory[index] = null;
            grabbable.InPlayerInventory = false;
            return true;
        }
        else
        {
            Debug.LogError("Trying to drop " + grabbable.gameObject + "from player inventory, but the player doesn't have it!");
            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _inventory = new Grabbable[InventoryCapacity];
    }
}
