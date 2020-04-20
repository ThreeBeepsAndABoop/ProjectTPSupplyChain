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
    Broken
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
        } else if (machineComponent == MachineComponentType.Broken)
        {
            return "Broken Component";
        }
        else
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
    public bool isBroken;

    public GameObject particals;
    public Material brokenMaterial;
    public Sprite brokenIcon;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isBroken && Condition < 0.005 || isBroken && Condition > 0)
        {
            isBroken = true;
            Condition = 0;
            Type = MachineComponentType.Broken;
            Instantiate(particals, transform);

            GetComponent<Grabbable>().name = Type.machineComponentName();
            GetComponent<Grabbable>().icon = brokenIcon;

            if (GetComponent<Renderer>() != null)
            {
                GetComponent<Renderer>().material = brokenMaterial;
            }
            
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.material = brokenMaterial;
            }
        }
    }
}
