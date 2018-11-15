using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormWranglers.Worm
{
    public class WormEye : MonoBehaviour
    {
        public float rotationClamp;
        public float rotationRate;
        private float value;
        private bool dir;
        private Quaternion initial;

        private void Awake()
        {
            initial = transform.rotation;
        }

        private void FixedUpdate()
        {
            Vector3 verticalaxis = transform.TransformDirection(Vector3.up);

            // update value
            if (dir)
                value += rotationRate * Time.deltaTime;
            else
                value -= rotationRate * Time.deltaTime;
            // update direction
            if (value > rotationClamp)
            {
                value = rotationClamp;
                dir = dir == true ? false : true;
            }
            if (value < -rotationClamp)
            {
                value = -rotationClamp;
                dir = dir == true ? false : true;
            }
            // reset rotation and apply value
            transform.localRotation = initial;
            transform.Rotate(verticalaxis, value);
        }
    }
}