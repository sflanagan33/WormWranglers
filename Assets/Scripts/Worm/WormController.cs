using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WormController : MonoBehaviour {

    private Worm worm;
    private MeshFilter mF;

    public Transform point;
    public Transform cam;
    private Vector3 camOffset;

    public float wormSpeed = 3f;
    public float rotationScale = 5f;
    public int maxSegments = 100;
    public float segmentSize = .1f;

    private void Awake()
    {
        camOffset = cam.position;

        mF = GetComponent<MeshFilter>();
        worm = new Worm(mF.mesh);
    }

    private void Update()
    {
        if (Mathf.Abs((point.position - worm.GetHeadPosition()).magnitude) > segmentSize)
        {
            worm.AddToFront(point.position);
        }    
        if (worm.ExceedsSegmentCount(maxSegments))
        {
            worm.RemoveFromBack(5);
        }
        UpdateMesh();

        cam.position = worm.GetHeadPosition() + camOffset;
        cam.LookAt(worm.GetHeadPosition());


    }

    private void UpdateMesh()
    {
        mF.mesh = null;
        mF.mesh = worm.ToMesh();
    }
}
