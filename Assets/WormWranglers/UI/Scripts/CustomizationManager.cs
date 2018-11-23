using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationManager : MonoBehaviour {

    public GameObject prefab;

    private List<Camera> cameras;

    private void Awake()
    {
        Settings.beetleCount = 3;
        cameras = new List<Camera>();

        bool gamepad1 = Input.GetJoystickNames().Length > 0;
        bool gamepad2 = Input.GetJoystickNames().Length > 1;


        if (Settings.beetleCount >= 1)
        {
            GameObject go = Instantiate(prefab);
            go.transform.position = new Vector3(0, 0);
            cameras.Add(go.GetComponent<Camera>());

            go.GetComponent<CustomizeCar>().AssignControls(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
        }
        if (Settings.beetleCount >= 2)
        {
            GameObject go = Instantiate(prefab);
            go.transform.position = new Vector3(0, 10);
            cameras.Add(go.GetComponent<Camera>());

            if (gamepad1)
                go.GetComponent<CustomizeCar>().AssignControls("Gamepad1Horizontal", "Gamepad1Vertical");
            else
                go.GetComponent<CustomizeCar>().AssignControls(KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K);
        }
        if (Settings.beetleCount >= 3)
        {
            GameObject go = Instantiate(prefab);
            go.transform.position = new Vector3(0, 20);
            cameras.Add(go.GetComponent<Camera>());

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
            cameras[1].rect = new Rect(0, 0, 0.495f, 1);
        }
        else if (Settings.beetleCount == 3)
        {
            cameras[0].rect = new Rect(0, 0.505f, 0.495f, 0.495f);
            cameras[1].rect = new Rect(0, 0, 0.495f, 0.495f);
            cameras[2].rect = new Rect(0.505f, 0, 0.495f, 0.495f);
        }

    }
}
