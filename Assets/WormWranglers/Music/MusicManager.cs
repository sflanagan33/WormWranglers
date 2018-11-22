// manage the music so it doesn't stop playing between scenes!
// written by mckenzie so this probably is bad but I'm out here

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private static MusicManager _instance;

    public static MusicManager Instance { get { return _instance; } }

    public AudioClip raceMusicLoop;
    public AudioClip resultsScreen;
    public AudioClip startScreen;
    public AudioClip current;

    AudioSource audioSource;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = startScreen;
        audioSource.Play();
	}
	
	void Update () {
		current = audioSource.clip;

	}

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool newSong = false; // true if the song is changed
        if (scene.name == "MainMenu" || scene.name == "PlayerSelection")
        {
            if(audioSource.clip != startScreen)
                newSong = true;
            audioSource.clip = startScreen;
        }

        if (scene.name == "Results")
        {
            if (audioSource.clip != resultsScreen)
                newSong = true;
            audioSource.clip = resultsScreen;
        }

        if (scene.name == "Garden")
        {
            if (audioSource.clip != raceMusicLoop)
                newSong = true;
            audioSource.clip = raceMusicLoop;
        }
        if (newSong)
        {
            audioSource.Play();
        }
    }


}
