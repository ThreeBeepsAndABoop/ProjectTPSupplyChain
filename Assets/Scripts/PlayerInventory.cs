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
    private GameObject _quitPanelGO;
    private Text _selectedItemText;
    private int[] _cachedLayers;

    public int Count { get; private set; }

    public int SelectedItemIndex;
    public int InventoryCapacity = 4;
    public int InventoryCount { get { return _inventory.Length; } }

    public bool IsFull()
    {
        return Count >= InventoryCapacity;
    }

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
        int index = (SelectedItemIndex - 1);
        if (index < 0)
        {
            index = InventoryCapacity - 1;
        }
        SelectItem(index);
    }

    public bool SelectItem(int index)
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
            _quitPanelGO.SetActive(false);
            return true;
        }

        Transform particles = newlySelectedItem.transform.Find("Particles");
        if (particles)
        {
            particles.gameObject.SetActive(true);
        }

        _selectedItemText.text = newlySelectedItem.name;
        if(newlySelectedItem.GetComponent<MachineComponent>())
        {
            var mc = newlySelectedItem.GetComponent<MachineComponent>();
            _selectedItemText.text += " - " + (mc.Condition * 100).ToString("0.##\\%");
        }
        _quitPanelGO.GetComponent<RectTransform>().anchoredPosition = new Vector3(15 + 90 * index, 30, 0);// fucking amazing.
        _quitPanelGO.SetActive(true);

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

        Transform particles = selectedItem.transform.Find("Particles");
        if (particles)
        {
            particles.gameObject.SetActive(false);
        }

        _selectedItemText.text = "";

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

    public bool PickUp(Grabbable grabbable)
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

        SelectItem(firstFreeIndex);

        _inventorySlots[firstFreeIndex].enabled = true;

        MachineComponent machineComponent = grabbable.GetComponent<MachineComponent>();
        if (machineComponent)
        {
            if (machineComponent.isBroken)
            {
                _inventorySlots[firstFreeIndex].color = new Color(45/255.0f, 45/255.0f, 45/255.0f, 125/255.0f);
                Debug.Log("BROKEN " + _inventorySlots[firstFreeIndex] + " color = " + _inventorySlots[firstFreeIndex].color);
            }
            else
            {
                _inventorySlots[firstFreeIndex].color = Color.white;
            }
        }
        _inventorySlots[firstFreeIndex].sprite = grabbable.icon;



        Count += 1;

        GameManager.Instance.RequestPlayPickUpSound();

        return true;
    }


    public bool DropSelected()
    {
        return Drop(GetSelectedItem());
    }

    public bool Drop(Grabbable grabbable)
    {
        if(grabbable == null) { return false; }
        int index = -1;
        for (int i = 0; i < _inventory.Length; i++)
        {
            if (_inventory[i] == grabbable)
            {
                index = i;
                break;
            }
        }
        if (index >= 0 && index < _inventory.Length)
        {
            if(index == SelectedItemIndex)
            {
                _selectedItemText.text = "";
                _quitPanelGO.SetActive(false);
            }
            Debug.Log("Inventory remove " + grabbable + " at index " + index);
            _inventory[index] = null;
            grabbable.InPlayerInventory = false;
            grabbable.transform.parent = null;
            var rbs = grabbable.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
            }

            Transform particles = grabbable.transform.Find("Particles");
            if(particles)
            {
                particles.gameObject.SetActive(true);
            }

            foreach (Transform trans in grabbable.transform.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = _cachedLayers[index];
            }

            _inventorySlots[index].enabled = false;
            _inventorySlots[index].sprite = null;

            Count -= 1;
            GameManager.Instance.RequestPlayDropSound();
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
        _quitPanelGO = _inventorySlotsGO.transform.Find("QUIT_PANEL").gameObject;
        _quitPanelGO.SetActive(false);
        _selectedItemText = _inventorySlotsGO.transform.Find("SELECTED_ITEM_WINDOW").GetComponentInChildren<Text>();

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
