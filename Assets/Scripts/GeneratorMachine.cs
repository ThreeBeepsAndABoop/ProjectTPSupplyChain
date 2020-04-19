using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeneratorMachine : MachineController
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
            }
        }


        // Determine what we need
        //var speed = 0.5 + (componentCounts[MachineComponentType.Coolant].Percent() / 2);
        //var charge = componentCounts[MachineComponentType.Battery].Percent();

        //requiredResources.Clear();
        //requiredResources.Add(new ResourceRequest(ResourceType.BatteryStorage, (int)Math.Round(charge * 1000)));

        //suppliableResources.Clear();
        //suppliableResources.Add(new ResourceRequest(ResourceType.FTLJumpDriveCharge, (int)Math.Round(charge * speed * 200)));



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
