using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusPole : MonoBehaviour
{

    public Material greenOn, greenOff, amberOn, amberOff, redOn, redOff;

    public GameObject greenLight, amberLight, redLight;

    public Light light;

    public StatusPoleLightColor statusColor;

    public float blinkTime;

    [Range(0.1f, 1.0f)]
    public float blinkSpeed;

    [Range(0.1f, 2.0f)]
    public float intensity;

    public AnimationCurve blinkCurve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        greenLight.GetComponent<Renderer>().material = greenOff;
        amberLight.GetComponent<Renderer>().material = amberOff;
        redLight.GetComponent<Renderer>().material   = redOff;

        if (statusColor != StatusPoleLightColor.Green)
        {
            blinkTime += Time.deltaTime / blinkSpeed;
            if (blinkTime > 1)
            {
                blinkTime -= 1;
            }
            light.intensity = blinkCurve.Evaluate(blinkTime) * intensity;
        } else
        {
            light.intensity = intensity;
        }

        if (light.intensity > 0.5) {
            if (statusColor == StatusPoleLightColor.Green)
            {
                greenLight.GetComponent<Renderer>().material = greenOn;
                light.color = new Color(0f, 1f, 0f);
            } else if (statusColor == StatusPoleLightColor.Amber)
            {
                amberLight.GetComponent<Renderer>().material = amberOn;
                light.color = new Color(1f, 1f, 0f);
            } else if (statusColor == StatusPoleLightColor.Red)
            {
                redLight.GetComponent<Renderer>().material = redOn;
                light.color = new Color(1f, 0f, 0f);
            }
        }
    }
}

public enum StatusPoleLightColor
{
    Green,
    Amber,
    Red
}