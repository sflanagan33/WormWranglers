using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

    //Variables that constrain movement
    public float forwardVelocity = 2f;
    public float rotation = 5f;

    //References to the road's runtime attributes
    private Vector3 roadPosition;
    private Vector3 roadOrientation;

    //References to the road's predetermined attributes
    private float roadWidth;
    private float roadSegmentLength;

    //The maximum angle that one segment can rotate without clipping issues
    private float maxAngle;

    //The current stored angle of rotation
    private float currentAngle;

    private void Start()
    {
        //Assign road attributes from its script
        roadWidth = FindObjectOfType<RoadBuildingScript>().roadWidth;
        roadSegmentLength = FindObjectOfType<RoadBuildingScript>().roadSegmentLength;

        //Figure out the max safe angle of rotation (lots of math)
        float w = roadWidth;
        float l = roadSegmentLength;
        float x = Mathf.Sqrt(Mathf.Pow(l, 2) + Mathf.Pow(w, 2)) - w;
        float alpha = Mathf.Atan(w / l);
        float a = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(l, 2) - (2 * x * l * Mathf.Cos(alpha)));

        maxAngle = 180 - Degrees(Mathf.Asin(x * a / (Mathf.Sin(alpha))));

        
    }

    private void Update()
    {
        //Assign road attributes from its script
        roadPosition = FindObjectOfType<RoadBuildingScript>().roadPosition;
        roadOrientation = FindObjectOfType<RoadBuildingScript>().roadOrientation;
    }

    private void FixedUpdate()
    {
        //Add rotation to current
        currentAngle += Input.GetAxis("Horizontal1") * rotation * Time.deltaTime;


        

        transform.Translate(forwardVelocity * Vector3.forward * Time.deltaTime);
        //transform.Translate(new Vector3(0, Input.GetAxis("Vertical1") * 2 * Time.deltaTime, 0));
    }

    public void RotateCursor()
    {
        //Apply a clamped rotation when the distance check from road's script is called
        transform.Rotate(0, Mathf.Clamp(currentAngle, -maxAngle, maxAngle), 0);
        currentAngle = 0;

    }

    private float Degrees(float angle)
    {
        return angle * 180 / Mathf.PI;
    }
}
