using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace WormWranglers.Core
{
	public class RaceManager : MonoBehaviour
	{
		public bool AllowInput { get { return state != State.Start; } }

        [SerializeField] private GameObject beetlePrefab;
        [SerializeField] private Transform[] beetlePositions;
        [SerializeField] private Camera wormCamera;
		[SerializeField] private RaceOverlay wormOverlay;
        [SerializeField] private Text startText;
		[SerializeField] private AudioSource startAudio;
		[SerializeField] private AudioClip[] startClips;

        private List<Beetle.Beetle> beetles = new List<Beetle.Beetle>();
		private List<RaceOverlay> beetleOverlays = new List<RaceOverlay>();

		private enum State { Start, Play, End }
		private State state;

		private int placement;

		// ====================================================================================================================
		// Setting up the race (spawning prefabs, etc.)

		private void Awake()
		{
			// Create beetles for each beetle player, give them their indices, and put them in a list

			for (int i = 0; i < Game.BEETLE_COUNT; i++)
            {
				Vector3 p = beetlePositions[i].position;
                GameObject g = Instantiate(beetlePrefab, p, Quaternion.identity);

                var b = g.GetComponent<Beetle.Beetle>();
                b.Initialize(i);
                beetles.Add(b);

				var o = g.GetComponentInChildren<RaceOverlay>();
				beetleOverlays.Add(o);
			}

			// Arrange the cameras on the screen

            var beetleCameras = beetles.Select(b => b.Camera).ToList();
            Game.ArrangeCameras(beetleCameras, wormCamera);
			
			// Start the placement index - the first person to lose will be (BEETLE_COUNT + 1)th place

			placement = Game.BEETLE_COUNT + 1;
		}

		// ====================================================================================================================
		// Race start countdown

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(2f);
			
			startText.text = "3";
			startAudio.PlayOneShot(startClips[0]);
			yield return new WaitForSeconds(1f);

			startText.text = "2";
			startAudio.PlayOneShot(startClips[0]);
			yield return new WaitForSeconds(1f);

			startText.text = "1";
			startAudio.PlayOneShot(startClips[0]);
			yield return new WaitForSeconds(1f);

			state = State.Play;
			Time.timeScale = 1f;
			startText.text = "GO";
			startAudio.PlayOneShot(startClips[1]);
			MusicManager.Play(MusicTrack.Race);
			yield return new WaitForSecondsRealtime(1f);

			startText.text = "";
		}

		// ====================================================================================================================
		// Race logic / end conditions

		public void BeetleCrashed(int index)
		{
			if (state == State.Play)
			{
				MusicManager.Play(Sound.Crash);
				beetleOverlays[index].Show(placement);
				placement--;

				// If we've reached 1st place, the worm won

				if (placement == 1)
				{
					wormOverlay.Show(placement);

					state = State.End;
					StartCoroutine(EndRoutine());
				}
			}
		}

		public void WormCrashed()
		{
			if (state == State.Play)
			{
				MusicManager.Play(Sound.Crash);
				wormOverlay.Show(placement);

				placement = 1;
				foreach (RaceOverlay o in beetleOverlays)
					o.Show(placement);
				
				state = State.End;
				StartCoroutine(EndRoutine());
			}
		}

		private IEnumerator EndRoutine()
		{
			MusicManager.Play(MusicTrack.None);
			yield return new WaitForSeconds(1.75f);

			MusicManager.Play(MusicTrack.Results);
			TransitionManager.ToScene(3);
		}
	}
}