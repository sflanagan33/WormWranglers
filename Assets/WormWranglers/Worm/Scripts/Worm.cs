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
        crossSection = OrganizeVertices(crossSection);
        crossSection = CloseHole(crossSection);

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
            float ratio = (i - vertCount) / (float)verticesPerSegment;
        }
        // attempt to link the triangles

        // first side
        t[triCount] = vertCount - verticesPerSegment; // first old point
        t[triCount + 1] = vertCount - verticesPerSegment + 1; //next old point
        t[triCount + 2] = vertCount; // first new point

        t[triCount + 4] = vertCount; // first new point
        t[triCount + 3] = vertCount + 1; // next new point
        t[triCount + 5] = vertCount - verticesPerSegment + 1; //next old point
        for (int i = 1; i < verticesPerSegment - 1; i ++)
        {
            t[triCount + 6 * i] = vertCount - verticesPerSegment + i;
            t[triCount + 6 * i + 1] = vertCount - verticesPerSegment + 1 + i;
            t[triCount + 6 * i + 2] = vertCount + i;

            t[triCount + 6 * i + 4] = vertCount + i;
            t[triCount + 6 * i + 3] = vertCount + 1 + i;
            t[triCount + 6 * i + 5] = vertCount - verticesPerSegment + 1 + i;
        }
        int lastIdx = triCount + (6 * (verticesPerSegment - 1));
        t[lastIdx] = vertCount - 1; // last old point
        t[lastIdx + 1] = vertCount - verticesPerSegment; //first old point
        t[lastIdx + 2] = vertCount + verticesPerSegment - 1; // last new point

        t[lastIdx + 4] = vertCount + verticesPerSegment - 1; // last new point
        t[lastIdx + 3] = vertCount; // first new point
        t[lastIdx + 5] = vertCount - verticesPerSegment; //last old point

        // assign normals

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
            v[i] = vertices[i + verticesPerSegment * amount];
            n[i] = normals[i + verticesPerSegment * amount];
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
            // rotate and offset to the correct position
            // Change first input to signedangle to match however the mesh is oriented
            points[i] = headPosition + Quaternion.Euler(0, Vector3.SignedAngle(Vector3.right, headOrientation, Vector3.up), 0) * csV[i];
        }
        return points;

    }

    private Mesh OrganizeVertices(Mesh m)
    {
        Vector3[] org = m.vertices;
        List<Vector3> verts = new List<Vector3>();
        foreach (Vector3 v in org)
        {
            verts.Add(v);
        }
        Vector3[] news = new Vector3[org.Length];
        news[0] = verts[org.Length - 1];
        verts.Remove(verts[org.Length - 1]);
        // find a closest point every time
        int newsIdx = 0;
        while (verts.Count != 0)
        {
            float min = Mathf.Infinity;
            int vertsIdx = -1;
            for (int i = 0; i < verts.Count; i++)
            {
                float dist = Mathf.Abs((news[newsIdx] - verts[i]).magnitude);
                if (dist < min)
                {
                    min = dist;
                    vertsIdx = i;
                }
            }
            // add to news and remove from list
            news[++newsIdx] = verts[vertsIdx];
            verts.Remove(verts[vertsIdx]);
        }
        m.vertices = news;
        return m;
    }

    private Mesh CloseHole(Mesh m)
    {
        int[] t = new int[3 * verticesPerSegment];
        for (int i = 1; i < verticesPerSegment - 1; i++)
        {
            t[3 * i] = 0;
            t[3 * i + 1] = i;
            t[3 * i + 2] = i + 1;
        }
        m.triangles = t;
        return m;
    }

    public Vector3 GetHeadPosition()
    {
        return headPosition;
    }

}
