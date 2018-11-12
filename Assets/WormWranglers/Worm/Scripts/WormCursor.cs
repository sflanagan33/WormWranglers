using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Util;

namespace WormWranglers.Worm
{
    public class WormCursor : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float rotation;

        [HideInInspector] public List<WormCursorPoint> points;
        [HideInInspector] public float distanceTraveled;

        private LerpFloat steer = new LerpFloat(0f, 0f, 0.1f, 2, -1f, 1f);

        private void Awake()
        {
            // Get all the points and add them to a public list

            points = new List<WormCursorPoint>();
            foreach (Transform p in transform)
                points.Add(p.GetComponent<WormCursorPoint>());

            AnimatedFloatManager.Add(this, steer, true);
        }

        private void Update()
        {
            // Rotate with player input (TODO: this is bad input management)

            steer.target = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0)
                         - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);

            // Mathf.PerlinNoise(Time.time * 0.5f, 0) * 2f - 1f; // brilliant AI

            transform.Rotate(Vector3.up, rotation * Time.deltaTime * steer);

            // Move in the forward direction, keeping track of the (lateral) distance traveled

            float distance = speed * Time.deltaTime;
            transform.position += transform.forward * distance;
            distanceTraveled += distance;

            // Place on top of terrain again

            SnapToGround();
        }

        public void SnapToGround()
        {
            Ray ray = new Ray(transform.position + Vector3.up * 100f, Vector3.down);
            RaycastHit hitInfo;
            float maxDistance = 200f;
            int layerMask = LayerMask.GetMask("Terrain");
            Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);

            Vector3 p = transform.position;
            p.y = hitInfo.point.y;
            transform.position = p;
        }

        // ====================================================================================================================
        // Displays the connections between this cursor's points in the editor.

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                Vector3 a = transform.GetChild(i).position;
                Vector3 b = transform.GetChild(i + 1).position;
                Gizmos.color = Color.Lerp(Color.magenta, Color.cyan, (float) i / transform.childCount);
                Gizmos.DrawLine(a, b);
            }
        }
    }
}