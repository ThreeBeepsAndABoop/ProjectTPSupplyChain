using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class GoToNextScene : MonoBehaviour
{
    // private PlayableDirector _director;

    public string GameSceneName = "JamesScene";

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(GameSceneName);
        // _director = this.GetComponent<PlayableDirector>();
        // _director.stopped += OnPlayableDirectorStopped;
    }

    // void OnPlayableDirectorStopped(PlayableDirector aDirector)
    // {
    //     if (_director == aDirector)
    //     {
    //         SceneManager.LoadScene(GameSceneName);
    //     }
    // }

    // void OnDisable()
    // {
    //     _director.stopped -= OnPlayableDirectorStopped;
    // }
}
