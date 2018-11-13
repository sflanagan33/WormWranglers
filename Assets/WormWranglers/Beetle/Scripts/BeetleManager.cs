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

        // select number of beetles
        public int beetleCount;
        // I got left and right backwards
        public List<KeyCode> leftSteer;
        public List<KeyCode> rightSteer;
        public List<KeyCode> accelerate;
        public List<KeyCode> decelerate;

        private void Awake()
        {
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
                        pos = new Vector3(-1, 0, -25);
                        break;
                    case 1:
                        pos = new Vector3(1, 0, -27);
                        break;
                    case 2:
                        pos = new Vector3(-1, 0, -29);
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
                col = new Color(col.r / 3f, col.g / 3f, col.b / 3f);
                thisBeetle.transform.Find("Visuals").GetChild(1).GetComponent<Renderer>().material.color = col;
                thisBeetle.transform.Find("Visuals").GetChild(2).GetComponent<Renderer>().material.color = col;
                thisBeetle.transform.Find("Visuals").GetChild(3).GetComponent<Renderer>().material.color = col;
                thisBeetle.transform.Find("Visuals").GetChild(4).GetComponent<Renderer>().material.color = col;
                thisBeetle.transform.position = pos;
                thisBeetle.GetComponentInChildren<Beetle>().AssignControls(leftSteer[i], rightSteer[i], accelerate[i], decelerate[i]);
                thisBeetle.GetComponentInChildren<BeetleVisuals>().AssignControls(leftSteer[i], rightSteer[i]);


                // Camera work
                thisCamera.transform.position += thisBeetle.transform.position;
                thisCamera.GetComponent<BeetleCam>().SetFollow(thisBeetle.GetComponent<Rigidbody>());
                Camera cam = thisCamera.transform.Find("Camera").GetComponent<Camera>();
                // Hard coded aspects
                if (beetleCount == 1)
                {
                    cam.rect = new Rect(0, 0, 0.5f, 1);
                    wormCam.rect = new Rect(0.5f, 0, 0.5f, 1);
                }
                else if (beetleCount == 2)
                {
                    if (i == 0)
                    {
                        cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                    }
                    else
                    {
                        cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                    }
                    wormCam.rect = new Rect(0.5f, 0, 0.5f, 1);
                }
                else if (beetleCount == 3)
                {
                    if (i == 0)
                    {
                        cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                    }
                    else if (i == 1)
                    {
                        cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                    }
                    else
                    {
                        cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                    }
                    wormCam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                }

                // Control display
                if (i == 0)
                {
                    instruct1.SetActive(true);
                    instruct1.transform.GetChild(0).GetComponent<Text>().text = "BEETLE 1";
                    instruct1.transform.GetChild(1).GetComponent<Text>().text = string.Format(
                        "{0} / {1} to steer\n{2} to accelerate\n{3} to reverse", 
                        rightSteer[i].ToString(), leftSteer[i].ToString(), accelerate[i].ToString(), decelerate[i].ToString());
                }
                else if (i == 1)
                {
                    instruct2.SetActive(true);
                    instruct2.transform.GetChild(0).GetComponent<Text>().text = "BEETLE 2";
                    instruct2.transform.GetChild(1).GetComponent<Text>().text = string.Format(
                        "{0} / {1} to steer\n{2} to accelerate\n{3} to reverse",
                        rightSteer[i].ToString(), leftSteer[i].ToString(), accelerate[i].ToString(), decelerate[i].ToString());
                }
                else if (i == 2)
                {
                    instruct3.SetActive(true);
                    instruct3.transform.GetChild(0).GetComponent<Text>().text = "BEETLE 3";
                    instruct3.transform.GetChild(1).GetComponent<Text>().text = string.Format(
                        "{0} / {1} to steer\n{2} to accelerate\n{3} to reverse",
                        rightSteer[i].ToString(), leftSteer[i].ToString(), accelerate[i].ToString(), decelerate[i].ToString());
                }


                // parent them for easy access
                thisBeetle.transform.parent = transform;
                thisCamera.transform.parent = transform;

            }
        }

        private void DisableUIs()
        {
            instruct1.SetActive(false);
            instruct2.SetActive(false);
            instruct3.SetActive(false);
        }

        // this just makes it so you can't mess up in the editor
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

            while (rightSteer.Count < beetleCount)
            {
                if (rightSteer.Count == 0)
                    rightSteer.Add(KeyCode.A);
                if (rightSteer.Count == 1)
                    rightSteer.Add(KeyCode.G);
                if (rightSteer.Count == 2)
                    rightSteer.Add(KeyCode.K);
            }

            while (leftSteer.Count < beetleCount)
            {
                if (leftSteer.Count == 0)
                    leftSteer.Add(KeyCode.D);
                if (leftSteer.Count == 1)
                    leftSteer.Add(KeyCode.J);
                if (leftSteer.Count == 2)
                    leftSteer.Add(KeyCode.Semicolon);
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


