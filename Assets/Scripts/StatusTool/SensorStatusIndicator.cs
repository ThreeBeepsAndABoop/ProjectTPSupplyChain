using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum SensorType
{
    Supernova,
    Anomaly,
}

public class SensorStatusIndicator : MonoBehaviour
{

    public SensorType type;

    [Range(0.0f, 1.0f)]
    public double current;

    public GameObject sensorNameText;
    public GameObject sensorStatusText;
    public GameObject sensorProgressText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /// Compute


        TextMeshProUGUI sensorStatusTMP = sensorStatusText.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI statusNameTMP = sensorNameText.GetComponent<TextMeshProUGUI>();

        TextMeshProUGUI statusProgressTMP = sensorProgressText.GetComponent<TextMeshProUGUI>();
        int charsWide = (int)Math.Round(59 * current);
        statusProgressTMP.text = "[" + new String('=', charsWide) + new String(' ', 59 - charsWide) + "]";

        Color color;
        switch (type)
        {
            case SensorType.Supernova:
                sensorStatusTMP.text = GameManager.Instance.GameCompletionString();
                statusNameTMP.text = "CRITICAL ALERT - SUPERNOVA EXPLOSION";
                current = Math.Min(GameManager.Instance.GameCompletionPercentage(), 1);
                color = new Color(1f, 0f, 0f);
                break;
            case SensorType.Anomaly:
                sensorStatusTMP.text = "???";
                statusNameTMP.text = "ANOMALY DETECTED - INCOMING SOLAR FLARE";
                color = new Color(1f, 1f, 0f);
                current = 0;
                break;
            default:
                color = new Color(0f, 1f, 0f);
                return;
        }

        statusProgressTMP.color = color;
        statusNameTMP.color = color;
        sensorStatusTMP.color = color;
    }
}
