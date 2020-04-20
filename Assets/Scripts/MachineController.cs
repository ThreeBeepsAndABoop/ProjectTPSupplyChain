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
            if (machineStatus != MachineStatus.Broken) {
                machineStatus = AttemptToGetAndSendResources();
            }

            UpdateTerminalAndLight();
        }
    }

    public void AddComponent(MachineComponent component)
    {
        Debug.Log("Add Component " + component);
        components.Add(component);
    }

    public void RemoveComponent(MachineComponent component)
    {
        Debug.Log("Remove Component " + component);
        components.Remove(component);
    }

    MachineStatus UpdateResourceRequests()
    {

        // Create a summary of what we have
        Dictionary<MachineComponentType, MachineComponentSummaryRequest> componentCounts = new Dictionary<MachineComponentType, MachineComponentSummaryRequest>();
        foreach (MachineComponent component in components)
        {
            if (component == null || component.isBroken) { continue; }


            if (componentCounts.ContainsKey(component.Type))
            {
                componentCounts[component.Type].components.Add(component);
            } else
            {
                componentCounts[component.Type] = new MachineComponentSummaryRequest(component.Type, 0, new List<MachineComponent>(), 0);
                componentCounts[component.Type].components.Add(component);
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
                componentCounts[component.componentType] = new MachineComponentSummaryRequest(component.componentType, component.minCount, new List<MachineComponent>(), component.maxCount);
            }
        }

        return UpdateResourceRequestsFromCounts(componentCounts);
    }

    virtual public MachineStatus UpdateResourceRequestsFromCounts(Dictionary<MachineComponentType, MachineComponentSummaryRequest> componentCounts)
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
            if (res.currentAmount >= res.maxAmount)
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
        terminal.AppendLine("Status: " + machineStatus.statusString());
        terminal.AppendLine("");
        PrintInputsAndOutputs();
        terminal.AppendLine("");
        PrintComponents();

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

    void PrintInputsAndOutputs()
    {
        List<string> inputLines = new List<string>();
        inputLines.Add("Requires:");
        foreach (ResourceRequest req in requiredResources)
        {
            inputLines.Add("- " + req.amount + " " + req.resourceType.typeString());
        }

        List<string> outputsLines = new List<string>();
        outputsLines.Add("Supplies:");
        foreach (ResourceRequest req in suppliableResources)
        {
            outputsLines.Add("- " + req.amount + " " + req.resourceType.typeString());
        }

        for(int i = 0; i < Math.Max(inputLines.Count, outputsLines.Count); i++)
        {
            string input;
            if (inputLines.Count > i)
            {
                input = inputLines[i];
            }
            else
            {
                input = "";
            }

            string output;
            if (outputsLines.Count > i)
            {
                output = outputsLines[i];
            }
            else
            {
                output = "";
            }

            terminal.AppendLine(input.PadRight(terminal.charsWide / 2, ' ') + output);
        }
    }

    void PrintTableLine(string component, string effect, string min, string max, string slot1, string slot2, Char fillter)
    {
        PrintTableLine(component, effect, min, max, slot1, slot2, null, null, null, null, null, null, fillter);
    }

    void PrintTableLine(string component, string effect, string min, string max, string slot1, string slot2,
        string componentColor, string effectColor, string minColor, string maxColor, string slot1Color, string slot2Color,
        Char fillter)
    {
        var cmp = (fillter + component).PadRight(12, fillter);
        var eff = (fillter + effect).PadRight(17, fillter);
        var mi = (fillter + min).PadRight(5, fillter);
        var ma = (fillter + max).PadRight(5, fillter);
        var s1 = (fillter + slot1).PadRight(7, fillter);
        var s2 = (fillter + slot2).PadRight(7, fillter);

        ColorizeString(ref cmp, componentColor);
        ColorizeString(ref eff, effectColor);
        ColorizeString(ref mi, minColor);
        ColorizeString(ref ma, maxColor);
        ColorizeString(ref s1, slot1Color);
        ColorizeString(ref s2, slot2Color);

        terminal.AppendLine("|" + cmp + "|" + eff + "|" + mi + "|" + ma + "|" + s1 + "|" + s2 + "|");
    }

    void ColorizeString(ref string str, string color)
    {
        if (color != null) { str = "<color=" + color + ">" + str + "</color>"; }
    }

    string ColorForItemStatus(string itemStatus)
    {
        const string alertColor = "#FF0000";
        const string warningColor = "#FF4500";

        int number;
        itemStatus = itemStatus.Replace("%", "");
        bool success = int.TryParse(itemStatus, out number);
        if (success)
        {
            if (number < 20) { return alertColor; }
            else if (number < 60) { return warningColor; }
            else { return null; }
        }
        else
        {
            if (itemStatus == "REQ.") { return alertColor; }
            else if (itemStatus == "EMPTY") { return warningColor; }
        }

        return null;
    }

    void PrintComponents()
    {
        if (componentRequests.Count == 0)
        {
            return;
        }


        terminal.AppendLine("Component Health:");
        terminal.AppendLine("Components degrade over time. Consult your ships maintenance department if components are missing.");
        terminal.AppendLine(" ");
        PrintTableLine("Component", "Effect", "Min", "Max", "1", "2", ' ');
        PrintTableLine("", "", "", "", "", "", '-');
        foreach (MachineComponentRequest req in componentRequests)
        {
            List<string> itemStatuses = new List<string>();
            foreach (MachineComponent cmp in components)
            {
                if (cmp != null && cmp.Type == req.componentType && !cmp.isBroken)
                {
                    itemStatuses.Add(Math.Round(cmp.Condition * 100).ToString() + "%");
                }
            }

            while (itemStatuses.Count < 2)
            {
                if (itemStatuses.Count > req.maxCount)
                {
                    itemStatuses.Add("N/A");
                }
                else if (itemStatuses.Count < req.minCount)
                {
                    itemStatuses.Add("REQ.");
                }
                else
                {
                    itemStatuses.Add("EMPTY");
                }
            }

            PrintTableLine(req.componentType.machineComponentName(), req.effect, req.minCount.ToString(), req.maxCount.ToString(), itemStatuses[0], itemStatuses[1],
                null, null, null, null, ColorForItemStatus(itemStatuses[0]), ColorForItemStatus(itemStatuses[1]),
                ' ');
        }
    }
}

public enum MachineStatus
{
    Good, DownstreamFull, LowEfficency, UpstreamEmpty, Broken
}

public static class MachineStatusExtensions
{
    public static string statusString(this MachineStatus machineStatus)
    {
        if (machineStatus == MachineStatus.Good)
        {
            return "Good";
        }
        else if (machineStatus == MachineStatus.DownstreamFull)
        {
            return "<color=#FF4500>Resource is at capacity</color>";
        } else if (machineStatus == MachineStatus.LowEfficency)
        {
            return "<color=#FF4500>Low Efficency - add more components or existing components will degrade faster</color>";
        }
        else if (machineStatus == MachineStatus.UpstreamEmpty)
        {
            return "<color=#FF0000>Missing required resources</color>";
        }
        else if (machineStatus == MachineStatus.Broken)
        {
            return "<color=#FF0000>Broken - missing required components</color>";
        } else
        {
            return "<color=#FF0000>Invalid</color>";
        }
    }
}

[System.Serializable]
public class MachineComponentRequest
{
    public MachineComponentType componentType;
    public string effect;
    public int minCount;
    public int maxCount;
}

[System.Serializable]
public class MachineComponentSummaryRequest
{
    public MachineComponentType componentType;
    public int minCount;
    public List<MachineComponent> components;
    public int maxCount;

    public MachineComponentSummaryRequest(MachineComponentType componentType, int minCount, List<MachineComponent> components, int maxCount)
    {
        this.componentType = componentType;
        this.minCount = minCount;
        this.components = components;
        this.maxCount = maxCount;
    }

    public double Percent()
    {
        return (double)components.Count / (double)maxCount;
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