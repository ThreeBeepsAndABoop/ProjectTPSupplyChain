using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{

    public List<Task> tasks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Task {

    public BoxLabel label;

    public List<ItemType> requiresItems = new List<ItemType>();

    public List<ItemType> packedItems = new List<ItemType>();

    public bool IsPacked() {
        return label.task.requiresItems.Count == label.task.packedItems.Count && !label.Box.open;
    }

    public bool _adulated;
    public bool Adulated
    {
        get
        {
            return _adulated;
        }
        set
        {
            _adulated = value;
            if(_adulated)
            {
                label.Box.UpdateAdulatedVisuals();
            }
        }
    }

    public bool Complete
    {
        get { return IsPacked() && Adulated; }
    }

    public bool sorted = false;

    public ConveyorSectorColor sectorColor;

    static Dictionary<ItemType, Material> itemMaterials = new Dictionary<ItemType, Material>();

    public Material GetImageForItem(ItemType item) {
        if (itemMaterials.ContainsKey(item)) {
            return itemMaterials[item];
        }

        string name = "";
        switch (item) {
            case ItemType.baseball:
                name = "baseball"; break;
            case ItemType.basketball:
                name = "basketball"; break;
            case ItemType.coins:
                name = "coins"; break;
            case ItemType.fish:
                name = "fish"; break;
            case ItemType.meat:
                name = "meat"; break;
            case ItemType.pens:
                name = "pens"; break;
            case ItemType.portraits:
                name = "portraits"; break;
            case ItemType.truck:
                name = "truck"; break;
            case ItemType.videoGame:
                name = "videoGame"; break;
        }

        string url = "Stickers/Items/" + name;
        var resource =  Resources.Load<Material>(url);
        itemMaterials[item] = resource;
        return resource;
    }


    static Dictionary<ConveyorSectorColor, Material> lineMaterials = new Dictionary<ConveyorSectorColor, Material>();

    public Material GetImageForLine(ConveyorSectorColor color) {
        if (lineMaterials.ContainsKey(color)) {
            return lineMaterials[color];
        }

        string name = "";
        switch (color) {
            case ConveyorSectorColor.black:
                return null;
            case ConveyorSectorColor.red:
                name = "red"; break;
            case ConveyorSectorColor.green:
                name = "green"; break;
            case ConveyorSectorColor.blue:
                name = "blue"; break;
        }

        string url = "Stickers/Lines/" + name;
        var resource =  Resources.Load<Material>(url);
        lineMaterials[color] = resource;
        return resource;
    }


}

public enum ItemType {
    fish,

    meat,

    baseball,

    basketball,

    truck,

    pens,

    coins,

    portraits,

    videoGame
}