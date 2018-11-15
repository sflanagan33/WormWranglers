using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormWranglers.Beetle
{
    // spawns two beetles in the text arena (WASD and Arrow Keys)
    public class CollisionTestScript : MonoBehaviour
    {

        public GameObject beetlePrefab;
        public GameObject cameraPrefab;

        private void Awake()
        {
            // create beetles, place them, and assign them controls
            for (int i = 0; i < 2; i++)
            {
                GameObject thisBeetle = Instantiate(beetlePrefab);
                GameObject thisCamera = Instantiate(cameraPrefab);
                // assign position
                Vector3 pos = Vector3.zero;
                Vector3 rot = Vector3.zero;
                switch (i)
                {
                    case 0:
                        pos = new Vector3(-3, 5, -3);
                        rot = new Vector3(0, 45, 0);
                        break;
                    case 1:
                        pos = new Vector3(3, 5, 3);
                        rot = new Vector3(0, 180 + 45, 0);
                        break;
                }
                thisBeetle.transform.position = pos;
                thisBeetle.transform.rotation = Quaternion.Euler(rot);

                // assign inputs
                if (i == 0)
                {
                    thisBeetle.GetComponentInChildren<Beetle>().AssignControls(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
                    thisBeetle.GetComponentInChildren<BeetleVisuals>().AssignControls(KeyCode.A, KeyCode.D);
                }
                else
                {
                    thisBeetle.GetComponentInChildren<Beetle>().AssignControls(KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow);
                    thisBeetle.GetComponentInChildren<BeetleVisuals>().AssignControls(KeyCode.LeftArrow, KeyCode.RightArrow);
                }
                thisBeetle.GetComponentInChildren<Beetle>().AssignIndex(i);


                thisCamera.transform.position += thisBeetle.transform.position;
                thisCamera.GetComponent<BeetleCam>().SetFollow(thisBeetle.GetComponent<Rigidbody>());
                Camera cam = thisCamera.transform.Find("Camera").GetComponent<Camera>();

                // Hard coded aspect ratio

                if (i == 0)
                {
                    cam.rect = new Rect(0, 0, 0.5f, 1);
                }
                else
                {
                    cam.rect = new Rect(0.5f, 0, 0.5f, 1);
                }
            }
        }
    }
}
