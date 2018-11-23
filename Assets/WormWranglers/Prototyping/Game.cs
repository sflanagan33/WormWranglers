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

        [HideInInspector]
        public bool beetleWon;

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
            End(winner, -1);
        }
		
		public void End(Player winner, int idx)
		{
			if (state == State.Play)
				StartCoroutine(EndRoutine(winner, idx));
		}

		private IEnumerator EndRoutine(Player winner, int idx)
		{
			state = State.End;
			Time.timeScale = 0f;
            if (beetleWon && idx > -1)
            {
                text.text = string.Format("Beetle {0} wins!", idx + 1);
            }
            else if (winner == Player.Worm)
            {
                text.text = "Worm wins!";
            }
            else
            {
                text.text = "Beetles win!";
            }
            yield return new WaitForSecondsRealtime(3f);

            Time.timeScale = 1f;
            SceneManager.LoadScene("Results");
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