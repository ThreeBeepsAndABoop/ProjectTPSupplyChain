using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI;
using Lightbug.GrabIt;
using TMPro;

public class GameManager : MonoBehaviour
{
    private GameManager() {}
    public static GameManager Instance { get; private set; }

    [HideInInspector]
    public GameObject Player;

    [HideInInspector]
    public PlayerInventory PlayerInventory;

    public ResourceManager ResourceManager;

    [HideInInspector]
    public AnomalyManager AnomalyManager;

    public GameObject BatteryPrefab;
    public GameObject CoolantPrefab;
    public GameObject MotorPrefab;
    public GameObject ComputerPrefab;
    public GameObject CompressorPrefab;

    public bool Debug = true;
    public bool PlayBackgroundMusic = true;
    public bool PlayBackgroundAmbience = true;

    public AudioClip ButtonClickSound;
    public AudioClip PickUpSound;
    public AudioClip DropSound;
    public AudioClip TickSound;
    public AudioClip SunExplosionSound;
    public AudioClip MachineSound;
    public AudioClip NewBoxSound;
    public AudioClip BackgroundMusic;
    public AudioClip BackgroundAmbience;

    private AudioSource loudAudioSource;
    private AudioSource musicAudioSource;

    public float totalGameTime = 5 * 60;
    public float timeleft = 5 * 60f;

    public AnimationCurve itemsPerBoxCurve = AnimationCurve.EaseInOut(0, 0, 1, 12);

    [HideInInspector]
    public GrabIt GrabIt;

    private TextMeshProUGUI _timeLeftLabel;

    public string GameCompletionString()
    {
        TimeSpan t = TimeSpan.FromSeconds(timeleft);
        string timeStr = t.ToString(@"hm\:ss\:fff");
        return timeStr;
    }

    public float GameCompletionPercentage()
    {
        return Math.Min(1, (totalGameTime - timeleft) / totalGameTime);
    }

    public void RequestPlayButtonClickSound()
    {
        loudAudioSource.PlayOneShot(ButtonClickSound);
    }

    public void RequestPlaySunExplosionSound()
    {
        loudAudioSource.PlayOneShot(SunExplosionSound);
    }

    public void RequestPlayTickSound()
    {
        loudAudioSource.PlayOneShot(TickSound);
    }

    public void RequestPlayPickUpSound()
    {
        loudAudioSource.PlayOneShot(PickUpSound);
    }

    public void RequestPlayDropSound()
    {
        loudAudioSource.PlayOneShot(DropSound);
    }

    public void RequestPlayBackgroundMusic()
    {
        musicAudioSource.loop = true;
        musicAudioSource.clip = BackgroundMusic;
        musicAudioSource.volume = 0.17f;
        musicAudioSource.Play();
    }

    public void RequestPlayBackgroundAmbience()
    {
        musicAudioSource.loop = true;
        musicAudioSource.clip = BackgroundAmbience;
        musicAudioSource.volume = 0.45f;
        musicAudioSource.Play();
    }

    public void RequestPlayFactoryProcessingSound(Machine machine, float duration)
    {
        AudioSource source = machine.gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.clip = MachineSound;
        source.volume = 0.5f;
        source.Play();

        StartCoroutine(StopAudioAfterDuration(source, duration));
    }

    IEnumerator StopAudioAfterDuration(AudioSource source, float duration)
    {
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            yield return null;
            while (Time.time < endTime)
            {
                yield return null;
            }
        }
        
        source.Stop();
        Destroy(source);
    }

    private bool _flashingStatusTool;
    IEnumerator FlashStatusToolEnumerator(int iterations)
    {
        if (_flashingStatusTool || iterations <= 0) {
            yield break;
        }
        // TODO: Genericize
        GameObject toolBoxWindowGO = GameObject.Find("TOOL_BOX_WINDOW");
        GameObject toolBoxGO = GameObject.Find("TOOL_BOX");

        if(!toolBoxGO || ! toolBoxWindowGO)
        {
            yield break;
        }

        Image toolBoxWindow = toolBoxWindowGO.GetComponent<Image>();
        Image toolBox = toolBoxWindowGO.GetComponent<Image>();

        Color toolBoxWindowColorStart = toolBoxWindow.color;
        Color toolBoxColorStart = toolBox.color;

        const float duration = 0.25f;
        float startTime = Time.time;
        float endTime = startTime + duration;

        yield return null;
        while (Time.time < endTime)
        {
            float pct = (endTime - Time.time) / (endTime - startTime);

            if(pct < 0.5)
            {
                pct *= 2;
                toolBoxWindow.color = Color.Lerp(toolBoxWindowColorStart, Color.white, pct);
                toolBox.color = Color.Lerp(toolBoxColorStart, Color.white, pct);
            } else
            {
                pct = (pct - .5f) * 2f;
                toolBoxWindow.color = Color.Lerp(Color.white, toolBoxWindowColorStart, pct);
                toolBox.color = Color.Lerp(Color.white, toolBoxColorStart, pct);
            }

            yield return null;
        }

        toolBoxWindow.color = toolBoxWindowColorStart;
        toolBox.color = toolBoxColorStart;
        _flashingStatusTool = false;

        if (iterations > 0)
        {
            StartCoroutine(FlashStatusToolEnumerator(iterations - 1));
        }
    }

    public void FlashStatusTool(int iterations)
    {
        if (_flashingStatusTool) { return; }
        StartCoroutine(FlashStatusToolEnumerator(iterations));
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        Player = GameObject.Find("Player").gameObject;
        PlayerInventory = Player.GetComponent<PlayerInventory>();
        timeleft = totalGameTime;

        AnomalyManager = GetComponent<AnomalyManager>();

        GameObject firstPersonCharacter = Player.transform.Find("FirstPersonCharacter").gameObject;
        GrabIt = firstPersonCharacter.GetComponent<GrabIt>();
        if (!GrabIt)
        {
            GrabIt = firstPersonCharacter.AddComponent<GrabIt>();
        }

        loudAudioSource = Player.AddComponent<AudioSource>();
        musicAudioSource = Player.AddComponent<AudioSource>();

        if (PlayBackgroundMusic)
        {
            RequestPlayBackgroundMusic();
        }

        if (PlayBackgroundAmbience)
        {
            RequestPlayBackgroundAmbience();
        }
    }

    private bool gameOverComplete;
    // Update is called once per frame
    void Update()
    {
        timeleft -= Time.deltaTime;

        if (timeleft <= 0 && !gameOverComplete)
        {
            gameOverComplete = true;
        } else if(timeleft > 0)
        {
            TimeSpan t = TimeSpan.FromSeconds(timeleft);
            string timeStr = t.ToString(@"hm\:ss\:fff");
        }
    }
}
