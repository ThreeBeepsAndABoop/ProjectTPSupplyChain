using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public enum MachineComponentType {
    Battery = 0,
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

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.MachineComponentManager.Register(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.MachineComponentManager.Unregister(this);
    }

    public void DamageCondition(float damage)
    {
        if(damage < 0) { return; }

        Condition = Math.Max(Condition - damage, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBroken && Condition < 0.005 || isBroken && Condition > 0)
        {
            isBroken = true;
            Condition = 0;
            Type = MachineComponentType.Broken;
            GameObject particles = Instantiate(particals, transform);
            particles.name = "Particles";


            GetComponent<Grabbable>().name = Type.machineComponentName();

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
