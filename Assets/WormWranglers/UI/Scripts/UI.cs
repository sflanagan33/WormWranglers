using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

	public void SceneTransition(string x)
    {
        SceneManager.LoadScene(x);
    }

    public void SceneTransition(int idx)
    {
        SceneManager.LoadScene(idx);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
