using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class WormController : MonoBehaviour {

    private Worm worm;
    private Vector3 follow;

    private MeshFilter mF;

    private void Awake()
    {
        mF = GetComponent<MeshFilter>();
        worm = new Worm(mF.mesh);
    }

    private void Update()
    {
        if (Mathf.Abs((follow - worm.GetHeadPosition()).magnitude) > 1f)
        {
            worm.AddToFront(follow);
        }

        follow += new Vector3(.5f * Time.deltaTime, 0, 0);
        UpdateMesh();


    }

    private void UpdateMesh()
    {
        mF.mesh = null;
        mF.mesh = worm.ToMesh();
    }
}
