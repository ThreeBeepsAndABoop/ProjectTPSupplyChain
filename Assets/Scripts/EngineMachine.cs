using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineMachine : MachineController
{
    public Light reactionGlow;
    public LinearTranslation engineFlapTranslationOne;
    public LinearTranslation engineFlapTranslationTwo;

    public AudioSource audioSource;


    private StatusPoleLightColor _previousStatus;

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

        if(statusPole.statusColor != _previousStatus)
        {
            if (reactionGlow == null ) { return; }
            _previousStatus = statusPole.statusColor;

            reactionGlow.enabled = statusPole.statusColor != StatusPoleLightColor.Error;
            engineFlapTranslationOne.enabled = statusPole.statusColor != StatusPoleLightColor.Error;
            engineFlapTranslationTwo.enabled = statusPole.statusColor != StatusPoleLightColor.Error;
            audioSource.enabled = statusPole.statusColor != StatusPoleLightColor.Error;
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
                    cmp.Condition -= 0.02 * UnityEngine.Random.Range(0f, 1f);
                } else
                {
                    cmp.Condition -= 0.004 * UnityEngine.Random.Range(0f, 1f);
                }

                cmp.Condition = Math.Max(cmp.Condition, 0);
            }
        }

        // We won't get here if the required components are missing.
        // Determine what we need
        var speed = 0.5 + (componentCounts[MachineComponentType.Coolant].Percent() / 2);
        var charge = componentCounts[MachineComponentType.Battery].Percent();


        // The engine requires between 10k and 20k Battery --  Depends on # of Batteries
        requiredResources.Clear();
        requiredResources.Add(new ResourceRequest(ResourceType.BatteryStorage, (int)Math.Round(charge * 20000)));

        // Produces between 3k and 5k FTL Jump Charge -- Depends on coolant and batteries.
        suppliableResources.Clear();
        suppliableResources.Add(new ResourceRequest(ResourceType.FTLJumpDriveCharge, (int)Math.Round(charge * speed * 2000) + 3000));

        // Go through the components and determine efficency...
        // Will need to be done for each client.
        if (lowEffMode)
        {
            return MachineStatus.LowEfficency;
        } else
        {
            return MachineStatus.Good;
        }
        
    }
}
