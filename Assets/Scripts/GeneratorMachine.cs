﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeneratorMachine : MachineController
{
    private StatusPoleLightColor _previousStatus;

    public List<GameObject> runningObjects;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        _previousStatus = statusPole.statusColor;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (statusPole.statusColor != _previousStatus)
        {
            _previousStatus = statusPole.statusColor;
            foreach (GameObject obj in runningObjects)
            {
                obj.SetActive(statusPole.statusColor != StatusPoleLightColor.Error);
            }
        }
    }

    override public MachineStatus UpdateResourceRequestsFromCounts(Dictionary<MachineComponentType, MachineComponentSummaryRequest> componentCounts)
    {

        // Go through the requests and make sure that we have all the required parts.
        // We also need to damage the components here.
        var totalComponents = 0;
        var totalRequestedComponents = 0;
        foreach (var row in componentCounts)
        {
            if (row.Key == MachineComponentType.Broken)
            {
                continue; // Don't count broken components, but it does not make us broken either.
            }

            if (row.Value.maxCount <= 0)
            {
                continue; // Don't count components that happen to be attached, but not required. This is a fix for basically only the example machine.
            }

            if (row.Value.components.Count < row.Value.minCount)
            {
                return MachineStatus.Broken;
            }

            totalComponents += row.Value.components.Count;
            totalRequestedComponents += row.Value.maxCount;
        }

        var lowEffMode = totalComponents <= totalRequestedComponents / 2;

        foreach (var row in componentCounts)
        {
            foreach (var cmp in row.Value.components)
            {
                if (lowEffMode)
                {
                    cmp.Condition -= 0.005 * UnityEngine.Random.Range(0f, 1f);
                }
                else
                {
                    cmp.Condition -= 0.002 * UnityEngine.Random.Range(0f, 1f);
                }

                cmp.Condition = Math.Max(cmp.Condition, 0);
            }
        }


        // Determine what we need
        var coolent     = componentCounts[MachineComponentType.Coolant].Percent();
        var motors      = componentCounts[MachineComponentType.Motor].Percent();
        var computer  = componentCounts[MachineComponentType.Computer].Percent();
        var eff = (coolent * 5000) + (motors * 4000) + (computer * 2000) + 1000;

        // Generators Use Nothing.
        requiredResources.Clear();

        // Gernators produce 5k-10k Depending on coolent (1+), motors (1+), and compressor (0 - 2)
        suppliableResources.Clear();
        suppliableResources.Add(new ResourceRequest(ResourceType.BatteryStorage, (int)Math.Round(eff)));



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
