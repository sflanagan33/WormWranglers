using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomizationManager : MonoBehaviour {

    public GameObject prefab;
    public string nextScene;

    private List<Camera> cameras;

    // selected models
    private GameObject[] selectedBeetles;

    private void Awake()
    {
        selectedBeetles = new GameObject[Settings.beetleCount];

        cameras = new List<Camera>();

        bool gamepad1 = Input.GetJoystickNames().Length > 0;
        bool gamepad2 = Input.GetJoystickNames().Length > 1;


        if (Settings.beetleCount >= 1)
        {
            GameObject go = Instantiate(prefab);
            go.transform.position = new Vector3(0, 0);
            cameras.Add(go.GetComponent<Camera>());
            go.GetComponent<CustomizeCar>().index = 0;
            go.GetComponent<CustomizeCar>().AssignControls(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
        }
        if (Settings.beetleCount >= 2)
        {
            GameObject go = Instantiate(prefab);
            go.transform.position = new Vector3(50, 0);
            cameras.Add(go.GetComponent<Camera>());
            go.GetComponent<CustomizeCar>().index = 1;
            if (gamepad1)
                go.GetComponent<CustomizeCar>().AssignControls("Gamepad1Horizontal", "Gamepad1Vertical");
            else
                go.GetComponent<CustomizeCar>().AssignControls(KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K);
        }
        if (Settings.beetleCount >= 3)
        {
            GameObject go = Instantiate(prefab);
            go.transform.position = new Vector3(100, 0);
            cameras.Add(go.GetComponent<Camera>());
            go.GetComponent<CustomizeCar>().index = 2;
            if (gamepad2)
                go.GetComponent<CustomizeCar>().AssignControls("Gamepad2Horizontal", "Gamepad2Vertical");
            else if (gamepad1)
                go.GetComponent<CustomizeCar>().AssignControls(KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K);
            else
            {
                go.GetComponent<CustomizeCar>().AssignControls(KeyCode.Keypad4, KeyCode.Keypad6, KeyCode.Keypad8, KeyCode.Keypad5);
            }
        }

        // Hard coded aspect ratio

        if (Settings.beetleCount == 1)
        {
            cameras[0].rect = new Rect(0, 0, 1, 1);
        }
        else if (Settings.beetleCount == 2)
        {
            cameras[0].rect = new Rect(0, 0, 0.495f, 1);
            cameras[1].rect = new Rect(0.505f, 0, 0.495f, 1);
        }
        else if (Settings.beetleCount == 3)
        {
            cameras[0].rect = new Rect(0, 0.505f, 0.495f, 0.495f);
            cameras[1].rect = new Rect(0, 0, 0.495f, 0.495f);
            cameras[2].rect = new Rect(0.505f, 0.2525f, 0.495f, 0.495f);
        }
    }

    private void Update()
    {
        bool finished = true;
        foreach (GameObject go in selectedBeetles)
            if (go == null)
                finished = false;
        if (finished)
        {
            // create a container for DontDestroyOnLoad that can be used at beginning of game scene
            GameObject o = Instantiate(new GameObject());
            o.name = "Beetles";
            for (int i = 0; i < selectedBeetles.Length; i++)
            {
                selectedBeetles[i].name = string.Format("Player{0}", i);
                selectedBeetles[i].transform.SetParent(o.transform);
            }
            DontDestroyOnLoad(o);

            SceneManager.LoadScene(nextScene);
        }
    }

    public void AssignFinalBeetle(GameObject beetle, int idx)
    {
        selectedBeetles[idx] = beetle;
    }

    public void RemoveBeetleSelection(int idx)
    {
        selectedBeetles[idx] = null;
    }
}
