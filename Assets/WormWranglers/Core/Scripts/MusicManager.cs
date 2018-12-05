using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace WormWranglers.Core
{
    public class MusicManager : MonoBehaviour
    {
        private static MusicManager instance;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] musicTracks;
        
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
            }
        }

        public static void Play(MusicTrack track)
        {
            if (!instance.isBusy && instance.audioSource.clip != instance.musicTracks[(int) track])
                instance.StartCoroutine(instance.PlayRoutine(track));
        }

        private IEnumerator PlayRoutine(MusicTrack track)
        {
            isBusy = true;

            // If playing a track, fade out

            if (audioSource.isPlaying)
            {
                for (float f = 0; f < fadeOutLength; f += Time.deltaTime)
                {
                    audioSource.volume = (1 - f / fadeOutLength) * maxVolume;
                    yield return null;
                }

                audioSource.volume = 0f;
                audioSource.Stop();
            }

            // If there's a new track, play it

            if (track != MusicTrack.None)
            {
                audioSource.clip = musicTracks[(int) track];
                audioSource.Play();
                
                for (float f = 0; f < fadeInLength; f += Time.deltaTime)
                {
                    audioSource.volume = (f / fadeInLength) * maxVolume;
                    yield return null;
                }

                audioSource.volume = maxVolume;
            }

            isBusy = false;
        }
    }

    public enum MusicTrack
    {
        None, Start, Race, Results
    }
}