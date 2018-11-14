using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormWranglers.Worm
{
    public class WormHead : MonoBehaviour
    {
        [SerializeField] private Transform cursor;
        [SerializeField] private Transform collisionBox;

        private void LateUpdate()
        {
            transform.position = cursor.position;
            transform.rotation = cursor.rotation;

            // Check for obstacle collisions (the worm mesh is itself an obstacle)

            int layerMask = LayerMask.GetMask("Obstacle");
            var result = Physics.OverlapBox(collisionBox.position, collisionBox.localScale,
                                            collisionBox.rotation, layerMask);
            if (result.Length > 0)
                FindObjectOfType<Game>().End(Player.Beetle);
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = collisionBox.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 2);
        }
    }
}