using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxLabel : MonoBehaviour
{

    public List<GameObject> requiresStickerSlots;

    public List<GameObject> requiresStickerBadgeSlots;

    public GameObject lineSticker;

    public Task task;

    public Box Box { get; set; }
    // Start is called before the first frame update
    void Awake()
    {
        task = new Task();
        task.label = this;
        UpdateLabel();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateLabel()
    {

        // Update the requires section.

        foreach (var obj in requiresStickerSlots) {
            obj.GetComponent<MeshRenderer>().enabled = false;
        }

        foreach (var obj in requiresStickerBadgeSlots) {
            obj.GetComponent<MeshRenderer>().enabled = false;
        }

        var remainingItems = new List<ItemType>(task.packedItems);

        for (var i = 0; i < task.requiresItems.Count && i < requiresStickerSlots.Count; i++) {
            var obj = requiresStickerSlots[i];
            obj.GetComponent<MeshRenderer>().enabled = true;
            obj.GetComponent<MeshRenderer>().material = task.GetImageForItem(task.requiresItems[i]);

            if (remainingItems.Contains(task.requiresItems[i])) {
                requiresStickerBadgeSlots[i].GetComponent<MeshRenderer>().enabled = true;
                remainingItems.Remove(task.requiresItems[i]);
            }
        }

        if (task.sectorColor != ConveyorSectorColor.black) {
            lineSticker.GetComponent<MeshRenderer>().enabled = true;
            lineSticker.GetComponent<MeshRenderer>().material = task.GetImageForLine(task.sectorColor);
        } else {
            lineSticker.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
