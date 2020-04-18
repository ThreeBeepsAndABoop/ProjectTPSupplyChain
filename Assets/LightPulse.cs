using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LightPulse : MonoBehaviour
{
    Light light;
    float startingIntensity;
    float time;

    public AnimationCurve translationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        startingIntensity = light.intensity;
    }

    bool pulseIncreasing;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        float duration = translationCurve.keys[translationCurve.keys.Length - 1].time;
        light.intensity = startingIntensity + translationCurve.Evaluate(time / duration);
    }
}
