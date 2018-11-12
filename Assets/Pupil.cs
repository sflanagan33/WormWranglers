using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pupil : MonoBehaviour {

    public Transform eye;

    private void FixedUpdate()
    {
        float x = Mathf.PerlinNoise(Time.time * 0.5f, 0) * 2f - 1f;
        float y = Mathf.PerlinNoise(0, Time.time * 0.5f) * 2f - 1f;

        transform.RotateAround(eye.position, Vector3.up, 30f * Time.deltaTime * Mathf.Cos(Time.time));
    }
}
