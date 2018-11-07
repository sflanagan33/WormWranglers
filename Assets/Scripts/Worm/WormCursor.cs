using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormCursor : MonoBehaviour {

    public float speed;
    public float rotation;
    public float segmentWidth;
    private float maxAngle;

    private void Awake()
    {
        //Figure out the max safe angle of rotation (lots of math)
        float w = segmentWidth;
        float l = FindObjectOfType<WormController>().segmentSize;
        float x = Mathf.Sqrt(Mathf.Pow(l, 2) + Mathf.Pow(w, 2)) - w;
        float alpha = Mathf.Atan(w / l);
        float a = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(l, 2) - (2 * x * l * Mathf.Cos(alpha)));

        maxAngle = 180 - Degrees(Mathf.Asin(x * a / (Mathf.Sin(alpha))));
    }

    private void FixedUpdate()
    {
        Vector3 forward = transform.forward;
        // move 
        transform.position += forward * speed * Time.deltaTime;
        // rotate
        transform.Rotate(Vector3.up, rotation * Time.deltaTime * Input.GetAxis("Horizontal"));
    }

    private float Degrees(float angle)
    {
        return angle * 180 / Mathf.PI;
    }
}
