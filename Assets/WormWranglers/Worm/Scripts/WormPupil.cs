using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormWranglers.Worm
{
    public class WormPupil : MonoBehaviour
    {
        public Transform eye;

        private void FixedUpdate()
        {
            Vector3 verticalaxis = transform.TransformDirection(Vector3.up);
            transform.RotateAround(eye.position, verticalaxis, 30f * Time.deltaTime * Mathf.Cos(Time.time));
        }
    }
}