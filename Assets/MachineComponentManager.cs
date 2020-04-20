using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MachineComponentManager : MonoBehaviour
{
    [HideInInspector]
    public HashSet<MachineComponent> MachineComponents;

    // Start is called before the first frame update
    void Awake()
    {
        MachineComponents = new HashSet<MachineComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Register(MachineComponent newComponent)
    {
        Debug.Log("Register Component " + newComponent);
        MachineComponents.Add(newComponent);
    }

    public void Unregister(MachineComponent newComponent)
    {
        Debug.Log("Unregister Component " + newComponent);
        MachineComponents.Remove(newComponent);
    }

    public void InflictDamageToAllComponentsOfType(MachineComponentType type, float conditionDamage, float percentageOfComponents)
    {
        var hs = new HashSet<MachineComponentType>();
        hs.Add(type);
        InflictDamageToAllComponentsOfTypes(hs, conditionDamage, percentageOfComponents);
    }

    public void InflictDamageToAllComponentsOfTypes(HashSet<MachineComponentType> types, float conditionDamage, float percentageOfComponents)
    {
        IEnumerable<MachineComponent> filteredComponents = from component in MachineComponents
                                                           where types.Contains(component.Type)
                                                           select component;

        foreach (MachineComponent c in filteredComponents)
        {
            if (Random.Range(0f, 1f) < percentageOfComponents)
            {
                double prevCondition = c.Condition;
                float dmg = conditionDamage * Random.Range(0.75f, 1.25f);
                c.DamageCondition(conditionDamage);
                Debug.Log("Applied " + dmg + " damage to " + c + " - prev=" + prevCondition + ", cur=" + c.Condition);
            }
        }
    }
}
