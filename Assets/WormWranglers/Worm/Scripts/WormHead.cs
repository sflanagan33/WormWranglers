using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Core;

namespace WormWranglers.Worm
{
    public class WormHead : MonoBehaviour
    {
        public bool Crashed { get { return crashed; } }
        
        [SerializeField] private Transform cursor;
        [SerializeField] private Transform collisionBox;

        private bool crashed;

        private void LateUpdate()
        {
            transform.position = cursor.position;
            transform.rotation = cursor.rotation;

            if (!crashed)
            {
                // Check for obstacle collisions (the worm mesh is itself an obstacle)

                int layerMask = LayerMask.GetMask("Obstacle");
                var result = Physics.OverlapBox(collisionBox.position, collisionBox.localScale,
                                                collisionBox.rotation, layerMask);
                if (result.Length > 0)
                {
                    crashed = true;
                    FindObjectOfType<RaceManager>().WormCrashed();
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = collisionBox.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 2);
        }
    }
}