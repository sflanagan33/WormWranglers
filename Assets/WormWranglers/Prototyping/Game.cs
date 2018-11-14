using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace WormWranglers
{
	// a very bad game manager written very quickly
	// TODO: replace! replace!

	public class Game : MonoBehaviour
	{
		[SerializeField] private Text text;

		private State state = State.Start;

		private IEnumerator Start()
		{
			Time.timeScale = 0f;
			yield return new WaitForSecondsRealtime(1f);
			
			text.text = "3";
			yield return new WaitForSecondsRealtime(1f);

			text.text = "2";
			yield return new WaitForSecondsRealtime(1f);

			text.text = "1";
			yield return new WaitForSecondsRealtime(1f);

			state = State.Play;
			Time.timeScale = 1f;
			text.text = "GO!";
			yield return new WaitForSecondsRealtime(1f);

			text.text = "";
		}
		
		public void End(Player winner)
		{
			if (state == State.Play)
				StartCoroutine(EndRoutine(winner));
		}

		private IEnumerator EndRoutine(Player winner)
		{
			state = State.End;
			Time.timeScale = 0f;
			text.text = winner == Player.Beetle ? "Beetle wins!" : "Worm wins!";
			yield return new WaitForSecondsRealtime(3f);

			SceneManager.LoadScene(0);
		}
	}

	public enum State
	{
		Start, Play, End
	}

	public enum Player
	{
		Beetle, Worm
	}
}