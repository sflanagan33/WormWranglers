using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WormWranglers.Core
{
	public class TransitionManager : MonoBehaviour
	{
		private static TransitionManager instance;

		[SerializeField] private Transform grid;

		private List<TransitionTile> tiles;
		private bool isBusy;

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

				tiles = new List<TransitionTile>();
				foreach (Transform t in grid)
					tiles.Add(t.GetComponent<TransitionTile>());

				StartCoroutine(ShowTiles(false));
			}
		}

		public static void ToScene(int index)
		{
			if (!instance.isBusy)
				instance.StartCoroutine(instance.ToSceneRoutine(index));
		}

		private IEnumerator ToSceneRoutine(int index)
		{
			isBusy = true;

			yield return StartCoroutine(ShowTiles(true));

			AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(index);
			while (!sceneLoad.isDone)
				yield return null;

			yield return new WaitForSecondsRealtime(0.25f);
			yield return StartCoroutine(ShowTiles(false));

			isBusy = false;
		}

		private IEnumerator ShowTiles(bool show)
		{
			for (int i = 0; i < tiles.Count; i += 2)
			{
				tiles[i].Show(show);
				tiles[i + 1].Show(show);
				yield return new WaitForSecondsRealtime(0.01f);
			}

			yield return new WaitForSecondsRealtime(0.25f);
		}
	}
}