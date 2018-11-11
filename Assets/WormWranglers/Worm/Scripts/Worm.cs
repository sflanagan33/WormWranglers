using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormWranglers.Worm
{
    public class Worm : MonoBehaviour
    {
        public WormCursor cursor;
        public WormMesh mesh;

        [SerializeField] private int segmentCount = 100;
        [SerializeField] private float segmentSize = 1f;

        private Vector3 cursorStoredPos = Vector3.zero;

        private void Start()
        {
            // Give the worm an initial length of segmentCount by moving the cursor along the ground
            // from (0, 0, (-segmentCount + 1) * segmentSize) to (0, 0, 0) in increments of segmentSize
 
            for (int i = -segmentCount + 1; i <= 0; i++)
            {
                cursor.transform.position = new Vector3(0, 0, i * segmentSize);
                cursor.distanceTraveled += segmentSize;
                cursor.SnapToGround();
                mesh.AddSegment(cursor);
            }

            // Bake the worm mesh so it shows up (this is the only time the mesh is triangulated)

            cursorStoredPos = cursor.transform.position;
            mesh.BakeMesh(cursor, true);
        }

        private void Update()
        {
            // If the cursor has moved farther than the segment size, shift on a new segment

            if ((cursor.transform.position - cursorStoredPos).magnitude > segmentSize)
            {
                cursorStoredPos = cursor.transform.position;
                mesh.ShiftSegment(cursor);
                mesh.BakeMesh(cursor, false);
            }
        }
    }
}