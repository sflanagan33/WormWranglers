using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBuildingScript : MonoBehaviour {

    //object that the road follows
    public Transform follow;

    //Components of the road's mesh
    private MeshFilter meshFilter;
    private Mesh mesh;

    //Components of the road's collider
    private MeshCollider meshCollider;

    //Runtime data about the road's head
    [HideInInspector]
    public Vector3 roadPosition;
    [HideInInspector]
    public Vector3 roadOrientation;

    //Public road data that can be manipulated in the editor
    public float roadWidth = 5f;
    public float roadSegmentLength = 1f;
    public int maximumSegments = 100;

	private void Start () {

        //Acquire the road's mesh components
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        meshCollider = GetComponent<MeshCollider>();
        meshCollider.inflateMesh = true;
        meshCollider.skinWidth = .1f;

        //Initialize the mesh and assign the head's data
        InitializeMesh();
        roadPosition = new Vector3(0, 0, roadSegmentLength);
        roadOrientation = new Vector3(0, 0, 1);

    }
	
	void FixedUpdate () {
        //Poll the followed object to see if it is an acceptable distance away
		if (DistanceCheck())
        {
            //If it is, append more road to follow it
            AddToMesh();
            //Rotate the cursor
            FindObjectOfType<CursorController>().RotateCursor();
        }

        /* Later change this to stay close behind last place */

        //Poll to see if the road is becoming too large
        if (mesh.vertexCount > maximumSegments * 2)
        {
            //Remove an amount of segments from the back of the road
            RemoveFromBack(5);
        }

	}


    private void InitializeMesh()
    {
        //Start by creating arrays for all the mesh's data
        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];
        Vector3[] normals = new Vector3[4];
        Vector2[] uv = new Vector2[4];

        //Populate the data with preset values
        vertices[0] = transform.InverseTransformPoint(new Vector3(-roadWidth / 2f, 0, 0));
        vertices[1] = transform.InverseTransformPoint(new Vector3(roadWidth / 2f, 0, 0));
        vertices[2] = transform.InverseTransformPoint(new Vector3(-roadWidth / 2f, 0, roadSegmentLength));
        vertices[3] = transform.InverseTransformPoint(new Vector3(roadWidth / 2f, 0, roadSegmentLength));
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 1;
        normals[0] = Vector3.up;
        normals[1] = Vector3.up;
        normals[2] = Vector3.up;
        normals[3] = Vector3.up;
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        //Use that data to create a new mesh, then copy it over to the object
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        meshFilter.mesh = mesh;

        //tell the collider to use this mesh
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
        
    }
    private void AddToMesh()
    {
        //determine current size of the mesh
        int vertCount = mesh.vertices.Length;
        int triCount = mesh.triangles.Length;

        //Create arrays to hold the mesh's (now larger) data
        Vector3[] vertices = new Vector3[vertCount + 2];
        int[] triangles = new int[triCount + 6];
        Vector3[] normals = new Vector3[vertCount + 2];
        Vector2[] uv = new Vector2[vertCount + 2];

        //Copy in existing data
        for (int i = 0; i < vertCount; i++)
        {
            vertices[i] = mesh.vertices[i];
            normals[i] = mesh.normals[i];
            uv[i] = mesh.uv[i];
        }
        for (int i = 0; i < triCount; i++)
        {
            triangles[i] = mesh.triangles[i];
        }

        //Determine the positions of the vertices of the new segment
        Vector3[] newCoords = CalculateCoords();

        //Add these values to vertices
        vertices[vertCount] = newCoords[0];
        vertices[vertCount + 1] = newCoords[1];

        //Add the new triangle indices
        //first triangle connects the new left point
        triangles[triCount] = vertCount - 2;
        triangles[triCount + 1] = vertCount;
        triangles[triCount + 2] = vertCount - 1;
        //second triangle connects the new right
        triangles[triCount + 3] = vertCount;
        triangles[triCount + 4] = vertCount + 1;
        triangles[triCount + 5] = vertCount - 1;

        //Add new points' normal data
        normals[vertCount] = Vector3.up;
        normals[vertCount + 1] = Vector3.up;

        //Add UV data
        if (uv[vertCount - 1] == new Vector2(1, 1))
        {
            uv[vertCount] = new Vector2(0, 0);
            uv[vertCount + 1] = new Vector2(1, 0);
        }
        else
        {
            uv[vertCount] = new Vector2(0, 1);
            uv[vertCount + 1] = new Vector2(1, 1);
        }

        //Use all this data to create a mesh, then copy it over
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        meshFilter.mesh = mesh;

        //tell the collider to use this mesh
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
        

    }

    private Vector3[] CalculateCoords()
    {
        //Determine the current status of the road
        roadOrientation = (follow.transform.position - roadPosition).normalized;
        roadPosition = follow.transform.position;

        //Create an array to store the new points
        Vector3[] point = new Vector3[2];

        //Two directional vectors that are perpendicular to the road's orientation
        Vector3 orientation0 = new Vector3(-roadOrientation.z, 0, roadOrientation.x);
        Vector3 orientation1 = new Vector3(roadOrientation.z, 0, -roadOrientation.x);
        point[0] = transform.InverseTransformPoint(roadPosition + orientation0 * roadWidth / 2f);
        point[1] = transform.InverseTransformPoint(roadPosition + orientation1 * roadWidth / 2f);

        return point;

    }

    private bool DistanceCheck()
    {
        //Calculate how far away the road's head is from the follow object
        Vector3 distanceFromFollow = follow.transform.position - roadPosition;
        float distanceMagnitude = distanceFromFollow.magnitude;

        //Return true if it's past threshold
        if (distanceMagnitude > roadSegmentLength)
            return true;
        //Return false if it's not
        return false;
    }

    private void RemoveFromBack(int amount)
    {

        //Determine current vertex count
        int vertCount = mesh.vertices.Length;
        int triCount = mesh.triangles.Length;

        //Create properly sized arrays to hold new mesh data
        Vector3[] vertices = new Vector3[vertCount - (2 * amount)];
        int[] triangles = new int[triCount - (6 * amount)];
        Vector3[] normals = new Vector3[vertCount - (2 * amount)];
        Vector2[] uv = new Vector2[vertCount - (2 * amount)];

        //Copy over all data except for [amount] at the back
        for (int i = 0; i < vertCount - (2 * amount); i++)
        {
            vertices[i] = mesh.vertices[i + 2 * amount];
            normals[i] = mesh.normals[i + 2 * amount];
            uv[i] = mesh.uv[i + 2 * amount];
        }
        for (int i = 0; i < triCount - (6 * amount); i++)
        {
            triangles[i] = mesh.triangles[i];
        }

        //Create a mesh with the data and copy over
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        meshFilter.mesh = mesh;

        //tell the collider to use this mesh
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }
}
