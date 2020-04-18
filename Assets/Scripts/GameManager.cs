using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lightbug.GrabIt;
using TMPro;

public class GameManager : MonoBehaviour
{
    private GameManager() {}
    public static GameManager Instance { get; private set; }

    public GameObject playerFPSController;

    public bool PlayBackgroundMusic = true;
    public bool PlayFactoryAmbience = true;

    public AudioClip ButtonClickSound;
    public AudioClip MachineSound;
    public AudioClip NewBoxSound;
    public AudioClip BackgroundMusic;
    public AudioClip BackgroundAmbience;

    private AudioSource loudAudioSource;
    private AudioSource musicAudioSource;
    private AudioSource ambienceAudioSource;

    public float totalGameTime = 5 * 60;
    public float timeleft = 5 * 60f;

    public AnimationCurve itemsPerBoxCurve = AnimationCurve.EaseInOut(0, 0, 1, 12);

    public GrabIt GrabIt;

    public void RequestPlayNewBoxSoundSound()
    {
        loudAudioSource.PlayOneShot(NewBoxSound);
    }

    public void RequestPlayButtonClickSound()
    {
        loudAudioSource.PlayOneShot(ButtonClickSound);
    }

    public void RequestPlayBackgroundMusic()
    {
        musicAudioSource.loop = true;
        musicAudioSource.clip = BackgroundMusic;
        musicAudioSource.volume = 0.17f;
        musicAudioSource.Play();
    }

    public void RequestPlayFactoryAmbience()
    {
        ambienceAudioSource.loop = true;
        ambienceAudioSource.clip = BackgroundAmbience;
        ambienceAudioSource.volume = 0.45f;
        ambienceAudioSource.Play();
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

    private TextMeshPro scoreLabel;
    private int score = 0;
    public void IncrementScore()
    {
        score += 1;
        scoreLabel.text = score + " boxes shipped";
    }

    private void LateUpdate() => Box.allBoxes.ForEach(b => b.onConveyor = false);


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        playerFPSController = GameObject.Find("FPSController").gameObject;
        timeleft = totalGameTime;
        scoreLabel = GameObject.Find("Score Label").GetComponent<TextMeshPro>();
        timerLabel = GameObject.Find("Canvas").transform.Find("Time Left").GetComponent<TextMeshProUGUI>();

        GameObject firstPersonCharacter = playerFPSController.transform.Find("FirstPersonCharacter").gameObject;
        GrabIt = firstPersonCharacter.GetComponent<GrabIt>();
        if (!GrabIt)
        {
            GrabIt = firstPersonCharacter.AddComponent<GrabIt>();
        }

        loudAudioSource = playerFPSController.AddComponent<AudioSource>();
        musicAudioSource = playerFPSController.AddComponent<AudioSource>();
        ambienceAudioSource = playerFPSController.AddComponent<AudioSource>();

        if (PlayBackgroundMusic)
        {
            RequestPlayBackgroundMusic();
        }

        if (PlayFactoryAmbience)
        {
            RequestPlayFactoryAmbience();
        }
    }

    private TextMeshProUGUI timerLabel;
    private bool gameOverComplete;
    // Update is called once per frame
    void Update()
    {
        timeleft -= Time.deltaTime;
        var itemsPerBox = Mathf.RoundToInt(itemsPerBoxCurve.Evaluate(1 - (timeleft / totalGameTime)));
        GameObject.FindObjectOfType<SpawnVentController>().numberOfItemsToPack = itemsPerBox;


        if (timeleft <= 0 && !gameOverComplete)
        {
            gameOverComplete = true;
            timerLabel.text = "Game Over! Your score: " + score;
        } else if(timeleft > 0)
        {
            float minutes = Mathf.Floor(timeleft / 60);
            float seconds = Mathf.RoundToInt(timeleft % 60);
            timerLabel.text = "Time Remaining: " + minutes + "m " + seconds + "s";
        }
    }
}
