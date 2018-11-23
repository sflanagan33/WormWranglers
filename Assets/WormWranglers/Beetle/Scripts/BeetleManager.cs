using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WormWranglers.Beetle
{
    public class BeetleManager : MonoBehaviour
    {
        // prefab for the camera attached to beetles
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

        // Used to determine beetles' lose states
        public float[] loseTimes;
        private float startTime;

        private void Awake()
        {
            startTime = Time.time;
            beetleCount = Settings.beetleCount;
            GameObject beetles = GameObject.Find("Beetles");
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
                GameObject thisBeetle = beetles.transform.Find("Player" + i).gameObject;
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
                thisBeetle.transform.position = pos;
                thisBeetle.transform.rotation = Quaternion.Euler(Vector3.zero);

                // assign index
                thisBeetle.GetComponentInChildren<Beetle>().AssignIndex(i);

                // Camera work

                thisCamera.transform.position += thisBeetle.transform.position;
                thisCamera.GetComponent<BeetleCam>().SetFollow(thisBeetle.GetComponent<Rigidbody>());
                Camera cam = thisCamera.transform.Find("Camera").GetComponent<Camera>();

                // Hard coded aspect ratio

                if (beetleCount == 1)
                {
                    cam.rect = new Rect(0, 0, 0.495f, 1);
                    wormCam.rect = new Rect(0.505f, 0, 0.495f, 1);
                }
                else if (beetleCount == 2)
                {
                    if (i == 0)
                        cam.rect = new Rect(0, 0.505f, 0.495f, 0.495f);
                    else
                        cam.rect = new Rect(0, 0, 0.495f, 0.495f);
                    wormCam.rect = new Rect(0.505f, 0, 0.495f, 1);
                }
                else if (beetleCount == 3)
                {
                    if (i == 0)
                        cam.rect = new Rect(0, 0.505f, 0.495f, 0.495f);
                    else if (i == 1)
                        cam.rect = new Rect(0, 0, 0.495f, 0.495f);
                    else
                        cam.rect = new Rect(0.505f, 0, 0.495f, 0.495f);
                    wormCam.rect = new Rect(0.505f, 0.505f, 0.495f, 0.495f);
                }

                // parent the game objects for easy access
                thisBeetle.transform.parent = transform;
                thisCamera.transform.parent = transform;
            }

            Destroy(beetles);
        }

        private void Update()
        {
            // continuously store survival times
            Settings.loseTimes = loseTimes;
        }

        public void Loser(int index)
        {
            loseTimes[index] = Time.time - startTime - 3f; // accounting for the countdown
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

            place1.rectTransform.anchoredPosition = new Vector3(-w / 4f, h / 4f);
            place2.rectTransform.anchoredPosition = new Vector3(-w / 4f, -h / 4f);
            place3.rectTransform.anchoredPosition = new Vector3(w / 4f, -h / 4f);

            instruct1.SetActive(false);
            instruct2.SetActive(false);
            instruct3.SetActive(false);
        }
    }
}


