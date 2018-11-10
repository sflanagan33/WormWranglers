using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class WormController : MonoBehaviour
{
    private Worm worm;
    private MeshFilter mF;
    private MeshCollider mC;

    public Transform cursor;
    private Vector3 cursorStoredPos;

    public int maxSegments = 100;
    public float segmentSize = .1f;
    public int backRemovalSegments = 10;

    private void Awake()
    {
        mF = GetComponent<MeshFilter>();
        mC = GetComponent<MeshCollider>();
        mC.inflateMesh = true;
        mC.skinWidth = .1f;
        worm = new Worm(mF.mesh, maxSegments, segmentSize);
    }

    private void Update()
    {
        if ((cursor.position - cursorStoredPos).magnitude > segmentSize)
        {
            cursorStoredPos = cursor.position;
            worm.Update(cursor);
            UpdateMesh();
        }

        //if (worm.ExceedsSegmentCount(maxSegments))
        //{
        //    worm.RemoveFromBack(backRemovalSegments);
        //    UpdateMesh();
        //}
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