using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachineComponentType {
    Battery,
    Coolant,
    Motor,
    Computer,
    Compressor,
}

public class MachineComponent : MonoBehaviour
{
    [Range(0, 100)]
    public double Condition = 100;
    public MachineComponentType Type;

    public GameObject DebugLabelPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = Instantiate(DebugLabelPrefab);
        go.transform.position = transform.position + new Vector3(0.0f, 0.25f, 0.0f);
        go.transform.parent = transform;

        go.transform.name = "Debug Label";
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCondition();
    }

    void UpdateCondition() {
        Condition -= BaseDeteriorationRate() * Time.deltaTime;
    }

    // measured in condition loss per second
    double BaseDeteriorationRate() {
        switch (Type) {
            default:
                return 1.0 / 60.0 * 100.0;
        }
    }
}
