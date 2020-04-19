using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunExpansion : MonoBehaviour
{

    public float StartScale = 15.0f;
    public float EndScale = 300.0f;

    public float StartIntensity = 1.5f;
    public float EndIntensity = 4.0f;

    public Color StartColor = new Color(248, 177, 71);
    public Color EndColor = new Color(248, 217, 205);

    public bool Debug;
    [Range(0, 1)]
    public float DebugPercentage = 0;

    private Light _light;

    // Start is called before the first frame update
    void Start()
    {
        _light = transform.Find("Light").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        float pct = GameManager.Instance.GameCompletionPercentage();

        if (Debug)
        {
            pct = DebugPercentage;
        }

        float scale = pct * (EndScale - StartScale) + StartScale;
        float intensity = pct * (EndIntensity - StartIntensity) + StartIntensity;
        Color color = Color.Lerp(StartColor, EndColor, pct);

        transform.localScale = new Vector3(scale, scale, scale);
        _light.intensity = intensity;
        _light.color = color;
    }
}
