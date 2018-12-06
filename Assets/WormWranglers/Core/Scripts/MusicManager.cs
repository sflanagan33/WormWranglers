using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace WormWranglers.Core
{
    public enum MusicTrack { None, Start, Race, Results }
    public enum Sound { Tick, Select, Crash }

    public class MusicManager : MonoBehaviour
    {
        private static MusicManager instance;

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip[] musicTracks;

        [SerializeField] private AudioSource soundSource;
        [SerializeField] private AudioClip[] soundClips;
        
		private bool isBusy;
        private float maxVolume = 0.6f;
        private float fadeInLength = 0.25f;
        private float fadeOutLength = 1f;

        private void Start()
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
            }

            else
            {
                instance = this;
                DontDestroyOnLoad(this);

                musicSource.ignoreListenerVolume = true;
                soundSource.ignoreListenerVolume = true;
            }
        }

        public static void Play(Sound sound)
        {
            instance.soundSource.PlayOneShot(instance.soundClips[(int) sound]);
        }

        public static void Play(MusicTrack track)
        {
            if (!instance.isBusy && instance.musicSource.clip != instance.musicTracks[(int) track])
                instance.StartCoroutine(instance.PlayRoutine(track));
        }

        private IEnumerator PlayRoutine(MusicTrack track)
        {
            isBusy = true;

            // If playing a track, fade out

            if (musicSource.isPlaying)
            {
                for (float f = 0; f < fadeOutLength; f += Time.deltaTime)
                {
                    musicSource.volume = (1 - f / fadeOutLength) * maxVolume;
                    yield return null;
                }

                musicSource.volume = 0f;
                musicSource.Stop();
            }

            // If there's a new track, play it

            if (track != MusicTrack.None)
            {
                musicSource.clip = musicTracks[(int) track];
                musicSource.Play();
                
                for (float f = 0; f < fadeInLength; f += Time.deltaTime)
                {
                    musicSource.volume = (f / fadeInLength) * maxVolume;
                    yield return null;
                }

                musicSource.volume = maxVolume;
            }

            isBusy = false;
        }
    }
}