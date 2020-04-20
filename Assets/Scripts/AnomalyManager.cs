using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum AnomalyType
{
    None,
    SolarFlare,
    ElectricalDischarge,
    Asteroid
}

public static class AnomalyTypeExtensions
{
    public static string displayString(this AnomalyType type)
    {
        switch (type)
        {
            case AnomalyType.SolarFlare:
                return "Solar Flare";
            case AnomalyType.ElectricalDischarge:
                return "Electrical Discharge";
            case AnomalyType.Asteroid:
                return "Asteroid";
            default:
                return "???";
        }
    }
}

public class AnomalyManager : MonoBehaviour
{
    [Range(0, 120)]
    public float minAnomalyCountdown = 30;
    [Range(0, 120)]        
    public float maxAnomalyCountdown = 60;

    [Range(0, 120)]
    public float minNextAnomalySpawnTime = 10;
    [Range(0, 120)]        
    public float maxNextAnomalySpawnTime = 30;

    private float _currentAnomalyStartCountdown;
    private float _currentAnomalyCurrentElapsedDuration;

    public AnomalyType NextAnomaly { get { return CurrentAnomaly; } }
    public AnomalyType CurrentAnomaly { get; private set; }
    public float NextAnomalyProgress
    {
        get
        {
            if (NextAnomaly == AnomalyType.None)
            {
                return 1.0f;
            } else
            {
                return Math.Max(0, _currentAnomalyCurrentElapsedDuration / _currentAnomalyStartCountdown);
            }
        }
    }

    public string NextAnomalyTimerString()
    {
        TimeSpan t = TimeSpan.FromSeconds(_currentAnomalyStartCountdown - _currentAnomalyCurrentElapsedDuration);
        string timeStr = t.ToString(@"hm\:ss\:fff");
        return timeStr;
    }

    private void Start()
    {
        ConfigureNextAnomalyAfterRandomDelay();
    }

    private void ConfigureNextAnomalyAfterRandomDelay()
    {
        float delay = UnityEngine.Random.Range(minNextAnomalySpawnTime, maxNextAnomalySpawnTime);
        Debug.Log("ConfigureNextAnomalyAfterRandomDelay = " + delay);
        CurrentAnomaly = AnomalyType.None;
        Invoke("ConfigureNextAnomaly", delay);
    }

    private void ConfigureNextAnomaly()
    {
        var rnd = new System.Random();
        while(CurrentAnomaly == AnomalyType.None)
        {
            CurrentAnomaly = (AnomalyType)rnd.Next(Enum.GetNames(typeof(AnomalyType)).Length);
        }
        _currentAnomalyStartCountdown = UnityEngine.Random.Range(minAnomalyCountdown, maxAnomalyCountdown);
        _currentAnomalyCurrentElapsedDuration = 0;

        GameManager.Instance.FlashStatusTool(5);

        Debug.Log("Next Anomaly = " + CurrentAnomaly.displayString() + ", triggering in " + _currentAnomalyStartCountdown + " seconds");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentAnomaly();
    }

    void UpdateCurrentAnomaly()
    {
        if (CurrentAnomaly == AnomalyType.None) { return; }

        _currentAnomalyCurrentElapsedDuration += Time.deltaTime;

        if (_currentAnomalyCurrentElapsedDuration >= _currentAnomalyStartCountdown)
        {
            TriggerAnomaly(CurrentAnomaly);
            ConfigureNextAnomalyAfterRandomDelay();
        }
    }


    void TriggerAnomaly(AnomalyType anomaly)
    {
        Debug.Log("Triggering Anomaly = " + anomaly.displayString());

        var hs = new HashSet<MachineComponentType>();
        var percentOfComponentsToDamage = 0.35f;
        var damageToInflict = 0.25f;
        switch(anomaly)
        {
            case AnomalyType.Asteroid:
                foreach (MachineComponentType type in Enum.GetValues(typeof(AnomalyType)))
                {
                    hs.Add(type);
                }
                break;
            case AnomalyType.ElectricalDischarge:
                hs.Add(MachineComponentType.Compressor);
                hs.Add(MachineComponentType.Motor);
                hs.Add(MachineComponentType.Computer);
                percentOfComponentsToDamage = 0.20f;
                damageToInflict = 0.15f;
                break;
            case AnomalyType.SolarFlare:
                hs.Add(MachineComponentType.Compressor);
                hs.Add(MachineComponentType.Motor);
                hs.Add(MachineComponentType.Computer);
                percentOfComponentsToDamage = 0.50f;
                damageToInflict = 0.35f;
                break;
            default:
                return;
        }


        GameManager.Instance.MachineComponentManager.InflictDamageToAllComponentsOfTypes(hs, damageToInflict, percentOfComponentsToDamage);
    }
}
