using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormCursor : MonoBehaviour
{
    public float speed;
    public float rotation;

    private void Awake()
    {
        SnapToGround();
    }

    private void Update()
    {
        // Rotate with player input

        float h = Input.GetAxis("Horizontal");
        h = Mathf.PerlinNoise(Time.time * 0.5f, 0) * 2f - 1f; // brilliant AI

        transform.Rotate(Vector3.up, rotation * Time.deltaTime * h);

        // Move in the forward direction

        transform.position += transform.forward * speed * Time.deltaTime;

        // Place on top of terrain

        SnapToGround();
    }

    private void SnapToGround()
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
}
