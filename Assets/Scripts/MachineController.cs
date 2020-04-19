using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MachineController : MonoBehaviour
{
    public string machineName;

    public MachineTerminal terminal;
    public StatusPole statusPole;

    public List<MachineComponent> components;
    public List<MachineComponentRequest> componentRequests;

    public List<ResourceRequest> bufferedResources;
    public List<ResourceRequest> requiredResources;
    public List<ResourceRequest> suppliableResources;

    public MachineStatus machineStatus;

    private float second;
    public float requiredTime;

    // Start is called before the first frame update
    public void Start()
    {
       
    }

    // Update is called once per frame
    public void Update()
    {
        second += Time.deltaTime;
        if (second > requiredTime)
        {
            second -= requiredTime;

            machineStatus = UpdateResourceRequests();
            machineStatus = AttemptToGetAndSendResources();
            UpdateTerminalAndLight();
        }
    }

    public void AddComponent(MachineComponent component)
    {
        components.Add(component);
    }

    public void RemoveComponent(MachineComponent component)
    {
        components.Remove(component);
    }

    MachineStatus UpdateResourceRequests()
    {

        // Create a summary of what we have
        Dictionary<MachineComponentType, MachineComponentSummaryRequest> componentCounts = new Dictionary<MachineComponentType, MachineComponentSummaryRequest>();
        foreach (MachineComponent component in components)
        {
            if (component == null) { continue; }


            if (componentCounts.ContainsKey(component.Type))
            {
                componentCounts[component.Type].count += 1;
            } else
            {
                componentCounts[component.Type] = new MachineComponentSummaryRequest(component.Type, 0, 1, 0);
            }
        }

        // Create a summary of what the min/max is
        foreach (MachineComponentRequest component in componentRequests)
        {
            if (componentCounts.ContainsKey(component.componentType))
            {
                componentCounts[component.componentType].minCount += component.minCount;
                componentCounts[component.componentType].maxCount += component.maxCount;
            }
            else
            {
                componentCounts[component.componentType] = new MachineComponentSummaryRequest(component.componentType, component.minCount, 0, component.maxCount);
            }
        }

        return UpdateResourceRequestsFromCounts(componentCounts);
    }

    public MachineStatus UpdateResourceRequestsFromCounts(Dictionary<MachineComponentType, MachineComponentSummaryRequest> componentCounts)
    {

        // Go through the components and determine efficency...
        // Will need to be done for each client.
        return MachineStatus.Good;
    }

    MachineStatus AttemptToGetAndSendResources()
    {
        
        // Attempt to get all the resources.
        bool goodToGo = true;
        foreach (ResourceRequest req in requiredResources)
        {

            // Get our buffer (if we have one)
            ResourceRequest buffer = null;
            foreach (ResourceRequest buf in bufferedResources)
            {
                if (req.resourceType == buf.resourceType)
                {
                    buffer = buf;
                    break;
                }
            }
            if (buffer == null)
            {
                buffer = new ResourceRequest(req.resourceType, 0);
                bufferedResources.Add(buffer);
            }

            if (req.amount <= buffer.amount)
            {
                continue;
            }


            // Attempt to fufill our request. Take as much as we can!
            Resource res = GameManager.Instance.ResourceManager.ResourceForType(req.resourceType);
            double amountToTake = Math.Min(res.currentAmount, req.amount - buffer.amount);
            buffer.amount += amountToTake;
            res.currentAmount -= amountToTake;


            if (req.amount > buffer.amount)
            {
                goodToGo = false;
            }
        }

        // Exit if we failed to get all required resources
        if ( !goodToGo )
        {
            return MachineStatus.UpstreamEmpty;
        }

        // We have to check to see if the downstream resouces can accept it, in full...
        foreach (ResourceRequest sup in suppliableResources)
        {
            Resource res = GameManager.Instance.ResourceManager.ResourceForType(sup.resourceType);
            if (res.currentAmount + sup.amount > res.maxAmount)
            {
                return MachineStatus.DownstreamFull;
            }
        }

        // Supply this machines resources to the system and clear the buffer.
        bufferedResources.Clear();
        foreach (ResourceRequest sup in suppliableResources)
        {
            Resource res = GameManager.Instance.ResourceManager.ResourceForType(sup.resourceType);
            res.currentAmount += sup.amount;
        }

        // Return Good unless we are missing alot of components.
        return machineStatus;
    }

    void UpdateTerminalAndLight()
    {
        terminal.Clear();
        terminal.AppendLine(machineName);
        terminal.AppendLine("Status: " + machineStatus.ToString());


        if (machineStatus == MachineStatus.Good)
        {
            statusPole.statusColor = StatusPoleLightColor.Good;
        } else if (machineStatus == MachineStatus.LowEfficency)
        {
            statusPole.statusColor = StatusPoleLightColor.Warn;
        } else  if (machineStatus == MachineStatus.DownstreamFull)
        {
            statusPole.statusColor = StatusPoleLightColor.Idle;
        } else
        {
            statusPole.statusColor = StatusPoleLightColor.Error;
        }
    }
}

public enum MachineStatus
{
    Good, DownstreamFull, LowEfficency, UpstreamEmpty, Broken
}

[System.Serializable]
public class MachineComponentRequest
{
    public MachineComponentType componentType;
    public int minCount;
    public int maxCount;
}

[System.Serializable]
public class MachineComponentSummaryRequest
{
    public MachineComponentType componentType;
    public int minCount;
    public int count;
    public int maxCount;

    public MachineComponentSummaryRequest(MachineComponentType componentType, int minCount, int count, int maxCount)
    {
        this.componentType = componentType;
        this.minCount = minCount;
        this.count = count;
        this.maxCount = maxCount;
    }
}

[System.Serializable]
public class ResourceRequest
{
    public ResourceType resourceType;
    public double amount;

    public ResourceRequest(ResourceType resourceType, int amount)
    {
        this.resourceType = resourceType;
        this.amount = amount;
    }
}