using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum AnomalyType
{
    None,
    SolarFlare,
    ElectricalDischarge,
    Asteroid,
    SpaceDebris,
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
            case AnomalyType.SpaceDebris:
                return "Space Debris";
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
        if (GameManager.Instance.timeleft < 15.0f) { return; }
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
        if (GameManager.Instance.IsGameOver) { return; }
        UpdateCurrentAnomaly();
    }

    void UpdateCurrentAnomaly()
    {
        if (CurrentAnomaly == AnomalyType.None) { return; }
        if (GameManager.Instance.timeleft < 15.0f) { _currentAnomalyStartCountdown = 100000000; return; }

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
            case AnomalyType.SpaceDebris:
                foreach (MachineComponentType type in Enum.GetValues(typeof(AnomalyType)))
                {
                    hs.Add(type);
                }
                percentOfComponentsToDamage = 1.0f;
                damageToInflict = 0.05f;
                break;
            default:
                return;
        }

        TriggerAnomalyUIWarning(anomaly);
        GameManager.Instance.MachineComponentManager.InflictDamageToAllComponentsOfTypes(hs, damageToInflict, percentOfComponentsToDamage);
    }

    void TriggerAnomalyUIWarning(AnomalyType anomaly)
    {
        // TODO - play sound?
        GameManager.Instance.FlashScreen(1f);
        string alertText = null;
        switch (anomaly)
        {
            case AnomalyType.Asteroid:
                alertText = "An asteroid has struck the ship! Some of our components in use have sustained damage. Unused components were unharmed.";
                break;
            case AnomalyType.ElectricalDischarge:
                alertText = "An electrical discharge has occured! Some of our electric components (compressors, motors, and computers) in use have sustained mild damage. Unused components were unharmed";
                break;
            case AnomalyType.SolarFlare:
                alertText = "The dying star has put out an intense solar flare! Many of our electric components (compressors, motors, and computers) in use have been severely damaged. Unused components were unharmed";
                break;
            case AnomalyType.SpaceDebris:
                alertText = "The ship has been peppered by space debris! All of our components in use have been sustained very minor damage. Unused components were unharmed.";
                break;
            default:
                alertText = "";
                return;
        }

        GameObject go = GameObject.Find("ANOMALY_TEXT");
        Text anomalyOnScreenText = go.GetComponent<Text>();
        anomalyOnScreenText.text = alertText;
        GameManager.Instance.FadeTextInAndOutAfterDelay(anomalyOnScreenText, 0.5f, 0.25f, 5.0f);
    }
}
