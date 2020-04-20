using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private Image _screenFlash;
    private Light _light;

    // Start is called before the first frame update
    void Start()
    {
        _light = transform.Find("Light").GetComponent<Light>();
        _screenFlash = GameObject.Find("SCREEN_FLASH").GetComponent<Image>();
    }

    bool _explosionBegun = false;
    float _explosionDuration = 12;

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.IsGameOver) { return; }

        if(GameManager.Instance.timeleft < _explosionDuration)
        {
            // play sun explosion
            ExplosiveSunGrowth();
        } else {
            NormalSunGrowth();
        }
    }

    void ExplosiveSunGrowth()
    {
        if(!_explosionBegun)
        {
            GameManager.Instance.RequestPlaySunExplosionSound();
        }

        _explosionBegun = true;

        float pct = (_explosionDuration - GameManager.Instance.timeleft) / _explosionDuration;

        const float finalScaleTarget = 800f;
        const float finalIntensityTarget = 6.0f;
        Color finalColorTarget = Color.white;

        float scale = pct * (finalScaleTarget - EndScale) + EndScale;
        float intensity = pct * (finalIntensityTarget - EndIntensity) + EndIntensity;
        Color color = Color.Lerp(EndColor, finalColorTarget, pct);

        _screenFlash.color = new Color(1, 1, 1, pct * pct * pct);

        transform.localScale = new Vector3(scale, scale, scale);
        _light.intensity = intensity;
        _light.color = color;
    }

    void NormalSunGrowth()
    {
        float pct = (GameManager.Instance.totalGameTime - GameManager.Instance.timeleft) / (GameManager.Instance.totalGameTime - _explosionDuration);

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
