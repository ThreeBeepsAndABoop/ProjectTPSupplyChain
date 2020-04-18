using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LightPulse : MonoBehaviour
{
    Light light;
    float startingIntensity;
    float time;

    public float duration = 1.0f;
    public AnimationCurve translationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        startingIntensity = light.intensity;
        pulseIncreasing = true;
    }

    bool pulseIncreasing;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        float max = translationCurve.Evaluate(duration);
        float offset = pulseIncreasing ? translationCurve.Evaluate(time / duration) : (max - translationCurve.Evaluate(time / duration));
        light.intensity = startingIntensity + offset;

        if (time > duration)
        {
            pulseIncreasing = !pulseIncreasing;
            time = 0;
        }
    }
}
