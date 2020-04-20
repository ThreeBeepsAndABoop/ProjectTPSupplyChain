using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SystemStatusIndicator : MonoBehaviour
{

    public ResourceType type;

    [Range(0.0f, 1.0f)]
    public double current;

    [Range(0.0f, 1.0f)]
    public double lowWatermark;

    public GameObject systemNameText;
    public GameObject systemStatusText;
    public GameObject systemProgressText;
    public GameObject systemLowWatermarkText;

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

    void UpdateSreen() 
    {
        /// Compute
        Resource res = GameManager.Instance.ResourceManager.ResourceForType(type);
        current = Math.Max(Math.Min(res.currentAmount / res.maxAmount, 1), 0);
        lowWatermark = Math.Max(Math.Min(res.requiredAmount / res.maxAmount, 1), 0);

        /// Draw
        TextMeshProUGUI systemStatusTMP = systemStatusText.GetComponent<TextMeshProUGUI>();
        string statusPercent = ((int)Math.Round(current * 100)).ToString() + "%";
        string statusQualitative;
        Color color;
        if (current < lowWatermark)
        {
            statusQualitative = "FAILURE";
            color = new Color(1f, 0.25f, 0.25f);
        }
        else if (lowWatermark < 0.90 && current - 0.1 < lowWatermark)
        {
            statusQualitative = "Warning";
            color = new Color(1f, 1f, 0f);
        }
        else
        {
            statusQualitative = "Good";
            color = new Color(0f, 1f, 0f);
        }
        systemStatusTMP.text = statusQualitative + " - " + statusPercent;
        systemStatusTMP.color = color;

        TextMeshProUGUI systemNameTMP = systemNameText.GetComponent<TextMeshProUGUI>();
        systemNameTMP.text = res.name;
        systemNameTMP.color = color;

        TextMeshProUGUI systemProgressTMP = systemProgressText.GetComponent<TextMeshProUGUI>();
        int charsWide = (int)Math.Round(59 * current);
        systemProgressTMP.text = "[" + new String('=', charsWide) + new String(' ', 59 - charsWide) + "]";
        systemProgressTMP.color = color;

        
        if (lowWatermark > 0)
        {
            TextMeshProUGUI systemLowWatermarkTMP = systemLowWatermarkText.GetComponent<TextMeshProUGUI>();
            int lowWatermarkCharsWide = (int)Math.Round(59 * lowWatermark);
            systemLowWatermarkTMP.text = " " + new String(' ', lowWatermarkCharsWide) + "|";
            systemLowWatermarkTMP.color = color;
            systemLowWatermarkText.SetActive(true);
        } else
        {
            systemLowWatermarkText.SetActive(false);
        }
    }
}
