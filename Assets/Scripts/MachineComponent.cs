using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public enum MachineComponentType {
    Battery,
    Coolant,
    Motor,
    Computer,
    Compressor,
}

public static class MachineComponentTypeExtensions
{
    public static string machineComponentName(this MachineComponentType machineComponent)
    {
        if (machineComponent == MachineComponentType.Battery)
        {
            return "Battery";
        } else if (machineComponent == MachineComponentType.Coolant)
        {
            return "Coolant";
        } else if (machineComponent == MachineComponentType.Motor)
        {
            return "Motor";
        } else if (machineComponent == MachineComponentType.Computer)
        {
            return "Computer";
        } else if (machineComponent == MachineComponentType.Compressor)
        {
            return "Compressor";
        } else
        {
            return "Invalid";
        }
    }
}

public class MachineComponent : MonoBehaviour
{
    [Range(0, 1)]
    public double Condition = 1;
    public MachineComponentType Type;
    public ComponentSlot Slot;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
