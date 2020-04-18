using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{

    public List<ConveyorSector> sectors;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ConveyorSector? SectorForColor(ConveyorSectorColor color) {
        foreach (var sector in sectors) {
            if (sector.color == color) {
                return sector;
            }
        }

        return null;
    }
}

[System.Serializable]
public struct ConveyorSector {

    public ConveyorSectorColor color;

    public Material material;

    [Range(-1.0f, 1.0f)]
    public float speed;
}

public enum ConveyorSectorColor {
    red, green, blue, black
}