using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LifeSupportMachine : MachineController
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    override public MachineStatus UpdateResourceRequestsFromCounts(Dictionary<MachineComponentType, MachineComponentSummaryRequest> componentCounts)
    {

        // Go through the requests and make sure that we have all the required parts.
        // We also need to damage the components here.
        var totalComponents = 0;
        var totalRequestedComponents = 0;
        foreach (var row in componentCounts)
        {
            if (row.Value.components.Count < row.Value.minCount)
            {
                return MachineStatus.Broken;
            }

            totalComponents += row.Value.components.Count;
            totalRequestedComponents += row.Value.maxCount;
        }

        var lowEffMode = totalComponents < totalRequestedComponents / 2;

        foreach (var row in componentCounts)
        {
            foreach (var cmp in row.Value.components)
            {
                if (lowEffMode)
                {
                    cmp.Condition -= 0.01;
                }
                else
                {
                    cmp.Condition -= 0.001;
                }

                cmp.Condition = Math.Max(cmp.Condition, 0);
            }
        }


        // Determine what we need
        var batt = (componentCounts[MachineComponentType.Battery].Percent() - 0.5) *2;
        var comp = (componentCounts[MachineComponentType.Compressor].Percent() - 0.5) * 2;
        var eff = 1900 * (1 - (batt * 0.8) - (comp * 0.2)) + 100;

        // Depending on Batteries (1-2) and Compressors (1-2) uses more or less power. 100 full eff, 2000 lowest eff.
        requiredResources.Clear();
        requiredResources.Add(new ResourceRequest(ResourceType.BatteryStorage, (int)Math.Round(eff)));

        // Life Support always generates 1K of support per second (ALOT)
        suppliableResources.Clear();
        suppliableResources.Add(new ResourceRequest(ResourceType.LifeSupport, 1000));



        // Go through the components and determine efficency...
        // Will need to be done for each client.
        if (lowEffMode)
        {
            return MachineStatus.LowEfficency;
        }
        else
        {
            return MachineStatus.Good;
        }

    }
}
