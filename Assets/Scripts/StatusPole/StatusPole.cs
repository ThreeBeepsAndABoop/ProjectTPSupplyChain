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

    public AudioClip badSound;
    public AudioClip okishSound;

    public AudioSource speaker;

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

        if (statusColor == StatusPoleLightColor.Good || statusColor == StatusPoleLightColor.Idle)
        {
            light.intensity = intensity;
        }  else
        {
            blinkTime += Time.deltaTime / blinkSpeed;
            if (blinkTime > 1)
            {
                blinkTime -= 1;
            }
            var prevIntensity = light.intensity;
            light.intensity = blinkCurve.Evaluate(blinkTime) * intensity;

            if (light.intensity >= 0.5 && prevIntensity < 0.5 ) {
                if (statusColor == StatusPoleLightColor.Error) {
                    speaker.PlayOneShot(badSound);
                } else {
                    speaker.PlayOneShot(okishSound);
                }
            }
        }

        if (light.intensity > 0.5) {
            if (statusColor == StatusPoleLightColor.Good)
            {
                greenLight.GetComponent<Renderer>().material = greenOn;
                light.color = new Color(0f, 1f, 0f);
            } else if (statusColor == StatusPoleLightColor.Warn)
            {
                amberLight.GetComponent<Renderer>().material = amberOn;
                light.color = new Color(1f, 1f, 0f);
            }
            else if (statusColor == StatusPoleLightColor.Error)
            {
                redLight.GetComponent<Renderer>().material = redOn;
                light.color = new Color(1f, 0f, 0f);
            }
            else if (statusColor == StatusPoleLightColor.Idle)
            {
                greenLight.GetComponent<Renderer>().material = greenOn;
                amberLight.GetComponent<Renderer>().material = amberOn;
                light.color = new Color(0.75f, 1f, 0f);
            }
        }
    }
}

public enum StatusPoleLightColor
{
    Good,
    Warn,
    Error,
    Idle
}