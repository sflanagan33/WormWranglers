using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WormWranglers.Beetle
{
    public class BeetleManager : MonoBehaviour
    {
        // determine what prefab to use
        // later this can be a list, so beetles can be unique
        public GameObject beetlePrefab;
        public GameObject cameraPrefab;

        // Need the worm's camera to set its aspect ratio
        public Camera wormCam;

        // Dirty instructions
        public GameObject instruct1;
        public GameObject instruct2;
        public GameObject instruct3;
        // Dirty Places
        public Text place1;
        public Text place2;
        public Text place3;

        // toggle for p2 and p3 to be controller enabled
        public bool player2Gamepad;
        public bool player3Gamepad;

        // select number of beetles
        public int beetleCount;

        public List<KeyCode> leftSteer;
        public List<KeyCode> rightSteer;
        public List<KeyCode> accelerate;
        public List<KeyCode> decelerate;

        // Used to determine beetles' lose states
        public float[] loseTimes;

        private void Awake()
        {
            beetleCount = Settings.beetleCount;
            Debug.Log(beetleCount);
            // see if controllers are plugged in
            if (Input.GetJoystickNames().Length > 0)
                player2Gamepad = true;
            else
                player2Gamepad = false;
            if (Input.GetJoystickNames().Length > 1)
                player3Gamepad = true;
            else
                player3Gamepad = false;

            // -1 means a beetle has not lost
            loseTimes = new float[beetleCount];
            for (int i = 0; i < beetleCount; i++)
            {
                loseTimes[i] = -1f;
            }

            DisableUIs();

            // create beetles, place them, and assign them controls
            for (int i = 0; i < beetleCount; i++)
            {
                GameObject thisBeetle = Instantiate(beetlePrefab);
                GameObject thisCamera = Instantiate(cameraPrefab);
                // assign position
                Vector3 pos = Vector3.zero;
                switch (i)
                {
                    case 0:
                        pos = new Vector3(-2, 0, -10);
                        break;
                    case 1:
                        pos = new Vector3(2, 0, -15);
                        break;
                    case 2:
                        pos = new Vector3(-2, 0, -20);
                        break;
                }
                // assign color (TEMPORARY)
                Color col = Color.white;
                switch (i)
                {
                    case 0:
                        col = Color.red;
                        break;
                    case 1:
                        col = Color.blue;
                        break;
                    case 2:
                        col = Color.yellow;
                        break;
                }
                // this is hard coded to a specific prefab structure
                thisBeetle.transform.Find("Visuals").GetComponent<Renderer>().material.color = col;
                col = new Color(col.r / 4f, col.g / 4f, col.b / 4f);
                thisBeetle.transform.Find("Visuals").GetChild(1).GetComponent<Renderer>().material.color = col;
                thisBeetle.transform.Find("Visuals").GetChild(2).GetComponent<Renderer>().material.color = col;
                thisBeetle.transform.Find("Visuals").GetChild(3).GetComponent<Renderer>().material.color = col;
                thisBeetle.transform.Find("Visuals").GetChild(4).GetComponent<Renderer>().material.color = col;
                thisBeetle.transform.position = pos;

                // assign inputs
                thisBeetle.GetComponentInChildren<Beetle>().AssignControls(leftSteer[i], rightSteer[i], accelerate[i], decelerate[i]);
                thisBeetle.GetComponentInChildren<BeetleVisuals>().AssignControls(leftSteer[i], rightSteer[i]);
                // assign index
                thisBeetle.GetComponentInChildren<Beetle>().AssignIndex(i);

                // Update controls based on controller settings
                if (player2Gamepad && i == 1)
                {
                    thisBeetle.GetComponentInChildren<Beetle>().AssignControls("Gamepad1Horizontal", "Gamepad1Vertical");
                    thisBeetle.GetComponentInChildren<BeetleVisuals>().AssignControls("Gamepad1Horizontal");
                }
                if (player3Gamepad && i == 2)
                {
                    thisBeetle.GetComponentInChildren<Beetle>().AssignControls("Gamepad2Horizontal", "Gamepad2Vertical");
                    thisBeetle.GetComponentInChildren<BeetleVisuals>().AssignControls("Gamepad2Horizontal");
                }


                // Camera work

                thisCamera.transform.position += thisBeetle.transform.position;
                thisCamera.GetComponent<BeetleCam>().SetFollow(thisBeetle.GetComponent<Rigidbody>());
                Camera cam = thisCamera.transform.Find("Camera").GetComponent<Camera>();

                // Hard coded aspect ratio

                if (beetleCount == 1)
                {
                    cam.rect = new Rect(0, 0, 0.5f, 1);
                    wormCam.rect = new Rect(0.5f, 0, 0.5f, 1);
                }
                else if (beetleCount == 2)
                {
                    if (i == 0)
                        cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                    else
                        cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                    wormCam.rect = new Rect(0.5f, 0, 0.5f, 1);
                }
                else if (beetleCount == 3)
                {
                    if (i == 0)
                        cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                    else if (i == 1)
                        cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                    else
                        cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                    wormCam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                }
                // Generate a UI
                if (i == 0)
                {
                    instruct1.SetActive(true);
                    instruct1.transform.GetChild(0).GetComponent<Text>().text = "BEETLE 1";
                    instruct1.transform.GetChild(1).GetComponent<Text>().text = string.Format(
                        "{0} {1} to steer\n{2} to accelerate\n{3} to reverse", 
                        rightSteer[i].ToString(), leftSteer[i].ToString(), accelerate[i].ToString(), decelerate[i].ToString());
                }
                else if (i == 1)
                {
                    instruct2.SetActive(true);
                    instruct2.transform.GetChild(0).GetComponent<Text>().text = "BEETLE 2";
                    instruct2.transform.GetChild(1).GetComponent<Text>().text = string.Format(
                        "{0} {1} to steer\n{2} to accelerate\n{3} to reverse",
                        rightSteer[i].ToString(), leftSteer[i].ToString(), accelerate[i].ToString(), decelerate[i].ToString());

                    if (player2Gamepad)
                        instruct2.transform.GetChild(1).GetComponent<Text>().text = "Gamepad\nControls";
                }
                else if (i == 2)
                {
                    instruct3.SetActive(true);
                    instruct3.transform.GetChild(0).GetComponent<Text>().text = "BEETLE 3";
                    instruct3.transform.GetChild(1).GetComponent<Text>().text = string.Format(
                        "{0} {1} to steer\n{2} to accelerate\n{3} to reverse",
                        rightSteer[i].ToString(), leftSteer[i].ToString(), accelerate[i].ToString(), decelerate[i].ToString());

                    if (player3Gamepad)
                        instruct3.transform.GetChild(1).GetComponent<Text>().text = "Gamepad\nControls";
                }
                // parent the game objects for easy access
                thisBeetle.transform.parent = transform;
                thisCamera.transform.parent = transform;
            }
        }

        public void Loser(int index)
        {
            loseTimes[index] = Time.time - 3f; // accounting for the countdown
            AssignPlace(index, DeterminePlace());

            // determine if there's a winner
            int remaining = 0;
            int remIdx = -1;
            for(int i = 0; i < beetleCount; i++)
            {
                if (loseTimes[i] == -1)
                {
                    remIdx = i;
                    remaining++;
                }
            }
            if (remaining == 1)
            {
                AssignWinner(remIdx);
                FindObjectOfType<Game>().beetleWon = true;
                // let them drive around for a few seconds more
                StartCoroutine(EndGame(remIdx));
            }
            else if (!FindObjectOfType<Game>().beetleWon && remaining == 0)
            {
                FindObjectOfType<Game>().End(Player.Worm);
            }
        }

        private IEnumerator EndGame(int remIdx)
        {
            yield return new WaitForSeconds(3f);

            FindObjectOfType<Game>().End(Player.Beetle, remIdx);

            yield return null;

        }

        private int DeterminePlace()
        {
            int place = beetleCount + 1;
            foreach (float f in loseTimes)
            {
                if (f > -1)
                    place--;
            }
            return place;
        }

        private void AssignPlace(int index, int place)
        {
            if (beetleCount == 1)
                return;
            string t = "";
            switch (place)
            {
                case 1:
                    t = "Winner!";
                    break;
                case 2:
                    t = "2nd";
                    break;
                case 3:
                    t = "3rd";
                    break;
            }
            if (index == 0)
                place1.text = t;
            else if (index == 1)
                place2.text = t;
            else
                place3.text = t;
        }

        private void AssignWinner(int index)
        {
            string t = "Winner!";
            if (index == 0)
                place1.text = t;
            else if (index == 1)
                place2.text = t;
            else
                place3.text = t;
        }

        private void DisableUIs()
        {
            // assign placements to their area of the screen
            float w = Screen.width;
            float h = Screen.height;

            place1.rectTransform.anchoredPosition = new Vector3(-w / 2f, h / 2f);
            place2.rectTransform.anchoredPosition = new Vector3(-w / 2f, -h / 2f);
            place3.rectTransform.anchoredPosition = new Vector3(w / 2f, -h / 2f);

            instruct1.SetActive(false);
            instruct2.SetActive(false);
            instruct3.SetActive(false);
        }

        // this just makes it so you can't mess up in the editor
        // and can easily return to preset controls
        private void OnValidate()
        {
            if (beetleCount > 3)
                beetleCount = 3;
            if (beetleCount < 0)
                beetleCount = 0;

            List<KeyCode> l = new List<KeyCode>();
            List<KeyCode> r = new List<KeyCode>();
            List<KeyCode> a = new List<KeyCode>();
            List<KeyCode> d = new List<KeyCode>();

            while (leftSteer.Count < beetleCount)
            {
                if (leftSteer.Count == 0)
                    leftSteer.Add(KeyCode.A);
                if (leftSteer.Count == 1)
                    leftSteer.Add(KeyCode.G);
                if (leftSteer.Count == 2)
                    leftSteer.Add(KeyCode.K);
            }

            while (rightSteer.Count < beetleCount)
            {
                if (rightSteer.Count == 0)
                    rightSteer.Add(KeyCode.D);
                if (rightSteer.Count == 1)
                    rightSteer.Add(KeyCode.J);
                if (rightSteer.Count == 2)
                    rightSteer.Add(KeyCode.Semicolon);
            }

            while (accelerate.Count < beetleCount)
            {
                if (accelerate.Count == 0)
                    accelerate.Add(KeyCode.W);
                if (accelerate.Count == 1)
                    accelerate.Add(KeyCode.Y);
                if (accelerate.Count == 2)
                    accelerate.Add(KeyCode.O);
            }

            while (decelerate.Count < beetleCount)
            {
                if (decelerate.Count == 0)
                    decelerate.Add(KeyCode.S);
                if (decelerate.Count == 1)
                    decelerate.Add(KeyCode.H);
                if (decelerate.Count == 2)
                    decelerate.Add(KeyCode.L);
            }

            for (int i = 0; i < beetleCount; i++)
            {
                l[i] = leftSteer[i];
                r[i] = rightSteer[i];
                a[i] = accelerate[i];
                d[i] = decelerate[i];
            }

            leftSteer = l;
            rightSteer = r;
            accelerate = a;
            decelerate = d;
        }
    }

}


