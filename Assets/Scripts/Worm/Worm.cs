using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Worm {
 
    // mesh stuff
	private Vector3[] vertices;
    private int[] triangles;
    private Vector3[] normals;
    private int verticesPerSegment;
    //position stuff
    private Vector3 headPosition;
    private Vector3 headOrientation;
    // stores the shape the worm will take
    private Mesh crossSection;

    public Worm(Mesh m)
    {
        crossSection = m;

        vertices = crossSection.vertices;
        triangles = crossSection.triangles;
        normals = crossSection.normals;
        verticesPerSegment = vertices.Length;

        headPosition = Vector3.zero;
        headOrientation = Vector3.zero;
    }

    public Mesh ToMesh()
    {
        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.normals = normals;
        m.RecalculateNormals();
        string x = "";
        foreach (Vector3 v in vertices)
        {
            x += v.ToString() + " ";
        }
        Debug.Log(x);

        return m;
    }

    public void AddToFront(Vector3 following)
    {
        int vertCount = vertices.Length;
        int triCount = triangles.Length;

        Vector3[] v = new Vector3[vertCount + verticesPerSegment];
        int[] t = new int[triCount + (6 * verticesPerSegment)];
        Vector3[] n = new Vector3[vertCount + verticesPerSegment];

        // Copy in existing data
        for (int i = 0; i < vertCount; i++)
        {
            v[i] = vertices[i];
            n[i] = normals[i];
        }
        for (int i = 0; i < triCount; i++)
        {
            t[i] = triangles[i];
        }

        // Determine the transposed positions of new vertices
        Vector3[] newCoords = CalculateCoords(following);
        // assign them to the end of vertices
        for (int i = vertCount; i < vertCount + verticesPerSegment; i++)
        {
            v[i] = newCoords[i - vertCount];
        }
        // attempt to link the triangles
        for (int i = 0; i < verticesPerSegment; i++)
        {
            // FIX //


            // left triangle
            t[triCount + 3 * i] = vertCount - verticesPerSegment + i;
            t[triCount + 1 + 3 * i] = vertCount + i;
            t[triCount + 2 + 3 * i] = vertCount - 1 + i;
            // right triangle
            t[triCount + 3 + 3 * i] = vertCount + i;
            t[triCount + 4 + 3 * i] = vertCount - 1 + i;
            t[triCount + 5 + 3 * i] = vertCount - verticesPerSegment + i + 1;


        }
        // add normal data /* DO LATER */
        for (int i = vertCount; i < vertCount + verticesPerSegment; i++)
        {
            n[i] = (crossSection.vertices[i - vertCount]).normalized;
        }

        // assign all this back to Worm
        vertices = v;
        triangles = t;
        normals = n;


    }

    public void RemoveFromBack(int amount)
    {
        //Determine current vertex count
        int vertCount = vertices.Length;
        int triCount = triangles.Length;

        //Create properly sized arrays to hold new mesh data
        Vector3[] v = new Vector3[vertCount - (verticesPerSegment * amount)];
        int[] t = new int[triCount - (6 * verticesPerSegment * amount)];
        Vector3[] n = new Vector3[vertCount - (verticesPerSegment * amount)];

        //Copy over all data except for [amount] at the back
        for (int i = 0; i < vertCount - (verticesPerSegment * amount); i++)
        {
            v[i] = vertices[i + 2 * amount];
            n[i] = normals[i + 2 * amount];
        }
        for (int i = 0; i < triCount - (6 * verticesPerSegment * amount); i++)
        {
            t[i] = triangles[i];
        }

        vertices = v;
        triangles = t;
        normals = n;
    }

    public bool ExceedsSegmentCount(int x)
    {
        return vertices.Length > x * verticesPerSegment;
    }

   private Vector3[] CalculateCoords(Vector3 following)
    {
        headOrientation = (following - headPosition).normalized;
        headPosition = following;

        // Create an array to store new points
        Vector3[] points = new Vector3[verticesPerSegment];


        // iterate across all points in the cross section
        Vector3[] csV = crossSection.vertices;
        for (int i = 0; i < csV.Length; i++)
        {
            /* This is almost certainly wrong */
            points[i] = headPosition + csV[i];
        }
        return points;

    }

    public Vector3 GetHeadPosition()
    {
        return headPosition;
    }

}
