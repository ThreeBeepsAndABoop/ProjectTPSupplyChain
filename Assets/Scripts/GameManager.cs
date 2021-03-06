﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI;
using Lightbug.GrabIt;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public enum LossReason
{
    None,
    Supernova,
    LifeSupport
}

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

    [HideInInspector]
    public MachineComponentManager MachineComponentManager;

    public GameObject BatteryPrefab;
    public GameObject CoolantPrefab;
    public GameObject MotorPrefab;
    public GameObject ComputerPrefab;
    public GameObject CompressorPrefab;

    public bool _DEBUG = false;

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

    public AudioClip StatusToolPing;
    public AudioClip StatusToolPingLong;

    private AudioSource loudAudioSource;
    private AudioSource musicAudioSource;

    [HideInInspector]
    public LossReason LossReason = LossReason.None;

    public float totalGameTime = 5 * 60;
    public float timeleft = 5 * 60f;

    public AnimationCurve itemsPerBoxCurve = AnimationCurve.EaseInOut(0, 0, 1, 12);

    [HideInInspector]
    public bool IsGameOver { get { return IsGameWon || IsGameLost; } }

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
        if (iterations <= 0) {
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
        loudAudioSource.PlayOneShot(StatusToolPing, 0.5f);
    }

    IEnumerator FlashScreenEnumerator(float duration)
    {
        // TODO: Genericize
        GameObject screenFlashGO = GameObject.Find("SCREEN_FLASH");

        if (!screenFlashGO)
        {
            yield break;
        }

        Image screenFlash = screenFlashGO.GetComponent<Image>();

        Color screenFlashColorStart = screenFlash.color;

        float startTime = Time.time;
        float endTime = startTime + duration;

        yield return null;
        while (Time.time < endTime)
        {
            float pct = (endTime - Time.time) / (endTime - startTime);

            if (pct < 0.5)
            {
                pct *= 2;
                screenFlash.color = Color.Lerp(screenFlashColorStart, Color.white, pct);
            }
            else
            {
                pct = (pct - .5f) * 2f;
                screenFlash.color = Color.Lerp(Color.white, screenFlashColorStart, pct);
            }

            yield return null;
        }

        screenFlash.color = screenFlashColorStart;
    }

    public void FlashScreen(float duration = 0.7f)
    {
        StartCoroutine(FlashScreenEnumerator(duration));
        loudAudioSource.PlayOneShot(StatusToolPingLong, 0.5f);
    }

    IEnumerator FadeInScreenEnumerator(float duration)
    {
        // TODO: Genericize
        GameObject screenFlashGO = GameObject.Find("SCREEN_FLASH");

        if (!screenFlashGO)
        {
            yield break;
        }

        Image screenFlash = screenFlashGO.GetComponent<Image>();

        Color screenFlashColorStart = screenFlash.color;

        float startTime = Time.time;
        float endTime = startTime + duration;

        yield return null;
        while (Time.time < endTime)
        {
            float pct = (endTime - Time.time) / (endTime - startTime);

            screenFlash.color = Color.Lerp(Color.white, screenFlashColorStart, pct);

            yield return null;
        }

        screenFlash.color = Color.white;
    }

    public void FadeInScreen(float duration = 1.2f)
    {
        StartCoroutine(FadeInScreenEnumerator(duration));
        loudAudioSource.PlayOneShot(StatusToolPingLong, 0.5f);
    }

    IEnumerator FadeTextInEnumerator(Text text, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        Color startColor = text.color;
        text.color = Color.clear;
        text.enabled = true;
        yield return null;
        while (Time.time < endTime)
        {
            float pct = (1 - (endTime - Time.time)) / (endTime - startTime);
            text.color = Color.Lerp(Color.clear, startColor, pct);

            yield return null;
        }
    }

    IEnumerator FadeTextInAndFadeOutAfterDelayEnumerator(Text text, float delay, float duration, float fadeOutAfter)
    {
        float waitDuration = delay;
        float startTime = Time.time;
        float endTime = startTime + waitDuration;

        yield return null;
        while (Time.time < endTime)
        {
            yield return null;
        }

        StartCoroutine(FadeTextInAndFadeOutEnumerator(text, duration, fadeOutAfter));
    }

    IEnumerator FadeTextInAndFadeOutEnumerator(Text text, float duration, float fadeOutAfter)
    {
        FadeTextIn(text, duration);

        float waitDuration = fadeOutAfter;
        float startTime = Time.time;
        float endTime = startTime + waitDuration;

        yield return null;
        while (Time.time < endTime)
        {
            yield return null;
        }

        FadeTextOut(text, duration);
    }

    public void FadeTextInAndOut(Text text, float durationToFade, float fadeOutAfter)
    {
        StartCoroutine(FadeTextInAndFadeOutEnumerator(text, durationToFade, fadeOutAfter));
    }

    public void FadeTextInAndOutAfterDelay(Text text, float delay, float durationToFade, float fadeOutAfter)
    {
        StartCoroutine(FadeTextInAndFadeOutAfterDelayEnumerator(text, delay, durationToFade, fadeOutAfter));
    }

    IEnumerator FadeTextOutEnumerator(Text text, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        Color startColor = text.color;
        text.color = Color.clear;
        yield return null;
        while (Time.time < endTime)
        {
            float pct = (1 - (endTime - Time.time)) / (endTime - startTime);
            text.color = Color.Lerp(startColor, Color.clear, pct);

            yield return null;
        }

        text.color = startColor;
        text.enabled = false;
    }

    public void FadeTextIn(Text text, float duration)
    {
        StartCoroutine(FadeTextInEnumerator(text, duration));
    }

    public void FadeTextOut(Text text, float duration)
    {
        StartCoroutine(FadeTextOutEnumerator(text, duration));
    }

    void DisplayFailure1()
    {

        GameObject failureTextGO = GameObject.Find("FAILURE_TEXT_1");
        Text failureText = failureTextGO.GetComponent<Text>();
        if(IsGameWon)
        {
            failureText.text = "With " + GameCompletionString() + " remaining,\nyour heroic efforts to start the FTL jump drive\nwere a success.";
        }
        else
        {
            if (LossReason == LossReason.LifeSupport)
            {
                failureText.text = "Despite your best efforts,\n the life support system failed.\n\nThough you could not keep the Starship Ludum alive\nand start the FTL Jump Drive, the warning signal your ship sent will be heard throughout the galaxy.";
            }
        }
        FadeTextIn(failureText, 1.0f);
    }

    void DisplayFailure2()
    {

        GameObject failureTextGO = GameObject.Find("FAILURE_TEXT_2");
        Text failureText = failureTextGO.GetComponent<Text>();
        if (IsGameWon)
        {
            failureText.text = "Your spacefaring expertise has kept\n the Starship Ludum, and yourself, alive.";
        }
        FadeTextIn(failureText, 1.0f);
    }

    void DisplayFailure3()
    {

        GameObject failureTextGO = GameObject.Find("FAILURE_TEXT_3");
        Text failureText = failureTextGO.GetComponent<Text>();
        FadeTextIn(failureText, 1.0f);
    }

    void DisplayGameOverMessage()
    {
        Invoke("DisplayFailure1", 1.5f);
        Invoke("DisplayFailure2", 6.0f);
        Invoke("DisplayFailure3", 10.0f);
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        Player = GameObject.Find("Player").gameObject;
        PlayerInventory = Player.GetComponent<PlayerInventory>();
        timeleft = totalGameTime;
        MachineComponentManager = GetComponent<MachineComponentManager>();
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

    private void GivePlayerStartingItems()
    {
        GameObject coolant = Instantiate(CoolantPrefab);
        PlayerInventory.PickUp(coolant.GetComponent<Grabbable>());
    }

    private void Start()
    {
        Invoke("GivePlayerStartingItems", 0.25f);

        Debug.Log("DisplayHintText");
        GameObject go = GameObject.Find("PLAYER_HINT_TEXT");
        Text hintText = go.GetComponent<Text>();
        FadeTextInAndOut(hintText, 1, 10);
    }

    public bool IsGameWon { get; private set; }
    public bool IsGameLost { get; private set; }

    void GameLost()
    {
        if (IsGameOver) { return; }

        IsGameLost = true;
        DisplayGameOverMessage();
        Player.GetComponent<FirstPersonController>().enabled = false;
    }

    void GameWon()
    {
        if(IsGameOver) { return; }

        IsGameWon = true;
        FadeInScreen();
        DisplayGameOverMessage();
        Player.GetComponent<FirstPersonController>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        timeleft -= Time.deltaTime;

        if(IsGameOver)
        {
            return;
        }

        if (timeleft <= 0)
        {
            LossReason = LossReason.Supernova;
            GameLost();
        }
        else if(timeleft > 0)
        {
            TimeSpan t = TimeSpan.FromSeconds(timeleft);
            string timeStr = t.ToString(@"hm\:ss\:fff");

            var r = ResourceManager.ResourceForType(ResourceType.FTLJumpDriveCharge);
            if (r.currentAmount >= r.requiredAmount)
            {
                GameWon();
            }

            r = ResourceManager.ResourceForType(ResourceType.LifeSupport);
            if (r.currentAmount < r.requiredAmount)
            {
                LossReason = LossReason.LifeSupport;
                FadeInScreen();
                GameLost();
            }
        }
    }
}
