using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private Grabbable[] _inventory;

    private Transform _itemSocketTransform;

    private GameObject _inventorySlotsGO;
    private Image[] _inventorySlots;
    private RectTransform _inventoryHighlight;
    private int[] _cachedLayers;

    public int SelectedItemIndex;
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
            Grabbable oldSelectedItem = GetSelectedItem();
            HideSelectedItem();
            DeselectSelectedItem();
        }

        SelectedItemIndex = index;
        _inventoryHighlight.anchoredPosition = new Vector3(50 + 90 * index, 0, 0);// fucking amazing.

        ShowSelectedItem();
        Grabbable newlySelectedItem = GetSelectedItem();

        if (!newlySelectedItem)
        {
            return true;
        }

        return true;
    }

    private int _savedLayer;
    private void HideSelectedItem()
    {
        Grabbable selectedItem = GetSelectedItem();
        if (!selectedItem)
        {
            return;
        }


        selectedItem.transform.Find("Model").gameObject.SetActive(false);
    }
    
    private void ShowSelectedItem()
    {
        Grabbable selectedItem = GetSelectedItem();

        if (!selectedItem)
        {
            return;
        }

        selectedItem.transform.Find("Model").gameObject.SetActive(true);
        selectedItem.transform.SetParent(_itemSocketTransform);
    }

    private bool DeselectSelectedItem()
    {
        Debug.Log("Deselect inventory at index " + SelectedItemIndex);
        Grabbable selectedItem = GetSelectedItem();
        if (selectedItem == null)
        {
            return false;
        }

        foreach (Transform trans in selectedItem.transform.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = _cachedLayers[SelectedItemIndex];
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
        _cachedLayers[firstFreeIndex] = grabbable.gameObject.layer;
        grabbable.InPlayerInventory = true;

        foreach (Transform trans in grabbable.transform.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = 11;
        }

        var rbs = grabbable.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        grabbable.transform.parent = _itemSocketTransform;
        grabbable.transform.localPosition = Vector3.zero;
        grabbable.transform.localRotation = Quaternion.identity;

        SelectItem(firstFreeIndex, distance);


        _inventorySlots[firstFreeIndex].enabled = true;
        _inventorySlots[firstFreeIndex].sprite = grabbable.icon;

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
            }
            _inventory[index] = null;
            grabbable.InPlayerInventory = false;
            grabbable.transform.parent = null;
            var rbs = grabbable.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
            }

            foreach (Transform trans in grabbable.transform.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = _cachedLayers[index];
            }

            _inventorySlots[index].enabled = false;
            _inventorySlots[index].sprite = null;

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
        GameObject player = GameManager.Instance.Player;
        _itemSocketTransform = player.transform.Find("FirstPersonCharacter/ItemSocket");
        _inventorySlotsGO = GameObject.Find("InventorySlots");

        _inventoryHighlight = _inventorySlotsGO.transform.Find("HIGHLIGHT").GetComponent<RectTransform>();

        _inventorySlots = new Image[InventoryCapacity];
        for (int i = 0; i < _inventorySlotsGO.transform.childCount; i++)
        {
            Transform child = _inventorySlotsGO.transform.GetChild(i);
            if (child.name.Contains("SLOT_"))
            {
                int index = int.Parse(child.name.Split('_')[1]);
                _inventorySlots[index] = child.Find("Image").GetComponent<Image>();
                Debug.Log("Found inventory slot " + _inventorySlots[index]);
            }
        }

        _cachedLayers = new int[InventoryCapacity];
        _inventory = new Grabbable[InventoryCapacity];
    }
}
