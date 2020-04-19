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

    public GameObject DebugLabelPrefab;

    private TextMeshPro _debugText;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = Instantiate(DebugLabelPrefab);
        go.transform.position = transform.position + new Vector3(0.0f, 0.3f, 0.0f);
        go.transform.SetParent(transform.Find("Model"));
        go.transform.name = "Debug Label";
        _debugText = go.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCondition();
    }

    void UpdateCondition() {
        Condition = Math.Max(Condition - BaseDeteriorationRate() * Time.deltaTime, 0);

        if (GameManager.Instance.Debug)
        {
            _debugText.text = string.Format("Condition: {0:0.00}%", Condition * 100);
        }
        else
        {
            _debugText.gameObject.SetActive(false);
        }
    }

    // measured in condition loss per second
    // could be coupled with machine types to have different deterioration rates 
    double BaseDeteriorationRate() {
        const Double percentPerSecond = (1.0 / 60.0);
        switch (Type) {
            case MachineComponentType.Battery:
                return 2 * percentPerSecond;
            default:
                return 1 * percentPerSecond;
        }
    }
}
