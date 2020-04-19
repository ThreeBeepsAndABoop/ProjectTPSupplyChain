using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    public List<Resource> resources;

    public ResourceManager(List<Resource>  resources)
    {
        this.resources = resources;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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