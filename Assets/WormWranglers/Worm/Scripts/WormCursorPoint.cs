using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormWranglers.Worm
{
    public class WormCursorPoint : MonoBehaviour
    {
        public float uCoord;

        // ====================================================================================================================
        // Displays this point in the editor for easy editing.

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.Lerp(Color.magenta, Color.cyan, uCoord);
            Gizmos.DrawSphere(transform.position, 0.4f);
        }
    }
}