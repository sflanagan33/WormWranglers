using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class WormController : MonoBehaviour {

    private Worm worm;
    private MeshFilter mF;
    private MeshCollider mC;
    // point that the head follows
    public Transform point;

    public int maxSegments = 100;
    public float segmentSize = .1f;
    public int backRemovalSegments = 10;

    private void Awake()
    {
        mF = GetComponent<MeshFilter>();
        mC = GetComponent<MeshCollider>();
        mC.inflateMesh = true;
        mC.skinWidth = .1f;
        worm = new Worm(mF.mesh);
        // for debugging
        worm.AddToFront(new Vector3(2, 0, 0));
        UpdateMesh();
        mF.mesh = worm.CloseHoleFront(mF.mesh);
        mF.mesh.RecalculateNormals();
        worm.DebugMesh(mF.mesh);
    }

    private void Update___()
    {
        if (Mathf.Abs((point.position - worm.GetHeadPosition()).magnitude) > segmentSize)
        {
            worm.AddToFront(point.position);
            UpdateMesh();
        }
        if (worm.ExceedsSegmentCount(maxSegments))
        {
            worm.RemoveFromBack(backRemovalSegments);
            UpdateMesh();
        }
    }

    private void UpdateMesh()
    {
        Mesh m = worm.ToMesh();
        mF.mesh = null;
        mF.mesh = m;
        mC.sharedMesh = null;
        mC.sharedMesh = m;
    }
}
