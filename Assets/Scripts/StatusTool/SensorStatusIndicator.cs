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

    private float currentRefreshTime;

    [Range(0.0f, 1.0f)]
    public float refreshTime;

    // Start is called before the first frame update
    void Start()
    {
        currentRefreshTime = refreshTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentRefreshTime += Time.deltaTime;
        if (currentRefreshTime >= refreshTime)
        {
            currentRefreshTime -= refreshTime;
            UpdateSreen();
        }
    }

    // Update is called once per frame
    void UpdateSreen()
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
                statusNameTMP.text = "CRITICAL ALERT - SUPERNOVA IMMINENT";
                current = GameManager.Instance.GameCompletionPercentage();
                color = new Color(1f, 0f, 0f);
                break;
            case SensorType.Anomaly:

                if (GameManager.Instance.AnomalyManager.NextAnomaly == AnomalyType.None)
                {
                    sensorStatusTMP.text = "";
                    statusNameTMP.text = "No anomalous activity detected";
                    color = new Color(0f, 1f, 0f);
                    current = 1;
                } else
                {
                    current = GameManager.Instance.AnomalyManager.NextAnomalyProgress;
                    sensorStatusTMP.text = GameManager.Instance.AnomalyManager.NextAnomalyTimerString();
                    statusNameTMP.text = "ANOMALY DETECTED - INCOMING " + GameManager.Instance.AnomalyManager.NextAnomaly.displayString();
                    color = new Color(1f, 1f, 0f);
                }
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
