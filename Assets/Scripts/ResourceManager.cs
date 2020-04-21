using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    public List<Resource> resources;

    // what the fuck
    private Dictionary<Resource, LinkedList<double>> runningResourceAverages;
    private Dictionary<Resource, double> previousAmounts;
    private Dictionary<Resource, double> resourceChangeRates;

    public ResourceManager(List<Resource>  resources)
    {
        this.resources = resources;
    }

    // Start is called before the first frame update
    void Start()
    {
        runningResourceAverages = new Dictionary<Resource, LinkedList<double>>();
        previousAmounts = new Dictionary<Resource, double>();
        resourceChangeRates = new Dictionary<Resource, double>();

        foreach(Resource res in resources)
        {
            runningResourceAverages.Add(res, new LinkedList<double>());
        }
    }

    public double RunningAverageForResource(Resource res)
    {
        if (!resourceChangeRates.ContainsKey(res))
        {
            return 0.0;
        }

        return resourceChangeRates[res] / updateRate;
    }

    // Update is called once per frame
    private const double updateRate = 0.1f;
    private double updateTimer = updateRate;
    private const int maximumRunningValuesToTrack = (int)(5.0 / updateRate) + 1; // seconds / update rate
    void Update()
    {
        Debug.Log("maximumRunningValuesToTrack = " + maximumRunningValuesToTrack);
        updateTimer -= Time.deltaTime;
        if(updateTimer < 0)
        {
            updateTimer = updateRate;

            LinkedList<double> runningResourceAverage = null;
            foreach (Resource res in resources)
            {
                runningResourceAverage = runningResourceAverages[res];

                if(runningResourceAverage.Count >= maximumRunningValuesToTrack)
                {
                    runningResourceAverage.RemoveFirst();
                }

                if (runningResourceAverage.Count == 0)
                {
                    runningResourceAverage.AddLast(res.currentAmount);
                }
                else
                {
                    runningResourceAverage.AddLast(res.currentAmount - previousAmounts[res]);
                }
                previousAmounts[res] = res.currentAmount;

                double total = 0;
                foreach (double val in runningResourceAverage)
                {
                    total += val;
                }
                resourceChangeRates[res] = total / runningResourceAverage.Count;
            }
        }
    }

    public Resource ResourceForType(ResourceType type)
    {
        foreach (Resource res in resources)
        {
            if (res.type == type)
            {
                return res;
            }
        }

        return null;
    }
}

public enum ResourceType 
{
    BatteryStorage, LifeSupport, FTLJumpDriveCharge

}

public static class ResourceTypeExtensions
{
    public static string typeString(this ResourceType type)
    {
        if (type == ResourceType.BatteryStorage)
        {
            return "Power";
        }
        else if (type == ResourceType.FTLJumpDriveCharge)
        {
            return "FTL Jump Drive Charge";
        }
        else if (type == ResourceType.LifeSupport)
        {
            return "Life Support";
        }
        else
        {
            return "Invalid";
        }
    }
}

[System.Serializable]
public class Resource
{
    public ResourceType type;

    public string name;

    public double currentAmount, maxAmount, requiredAmount;

    public Resource(string name, double currentAmount, double maxAmount, double requiredAmount)
    {
        this.name = name;
        this.currentAmount = currentAmount;
        this.maxAmount = maxAmount;
        this.requiredAmount = requiredAmount;
    }
}