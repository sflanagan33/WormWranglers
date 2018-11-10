using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm
{
    // mesh stuff
	private Vector3[] vertices;
    private int[] triangles;
    private Vector3[] normals;
    private int verticesPerSegment;

    // stores the shape the worm will take
    private Mesh crossSection;

    public Worm(Mesh m)
    {
        crossSection = OrganizeVertices(m);
        verticesPerSegment = m.vertices.Length;

        vertices = crossSection.vertices;
        triangles = crossSection.triangles;
    }

    public Worm(Mesh m, int segments, float spacing)
    {
        crossSection = OrganizeVertices(m);
        verticesPerSegment = m.vertices.Length;

        vertices = crossSection.vertices;
        triangles = crossSection.triangles;

        CreateEntireMesh(segments, spacing);
    }

    public void Update(Transform cursor)
    {
        // reassign vertex locations from back to front
        for (int i = 0; i < (vertices.Length / verticesPerSegment) - 1; i++)
        {
            for (int j = 0; j < verticesPerSegment; j++)
            {
                vertices[i * verticesPerSegment + j] = vertices[(i + 1) * verticesPerSegment + j];
            }
        }
        // calculate the new front positions
        Vector3[] csV = crossSection.vertices;
        for (int i = vertices.Length - verticesPerSegment; i < vertices.Length; i++)
        {
            vertices[i] = cursor.TransformPoint(csV[i - vertices.Length + verticesPerSegment]);
        }
    }

    public Mesh ToMesh()
    {
        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();

        return m;
    }

    public void CreateEntireMesh(int segments, float spacing)
    {
        GameObject obj = new GameObject();
        Transform t = obj.transform;
        obj.transform.position = Vector3.zero;
        for (int i = 0; i < segments; i++)
        {
            // t.position += Vector3.back * spacing;
            t = SnapToGround(t);
            AddToFront(t);
        }
        // just to be safe
        GameObject.Destroy(obj);
        Transform.Destroy(t);
    }

    public Vector3[] AssignNormals()
    {
        // determine center point
        Vector3 center = Vector3.zero;
        foreach (Vector3 v in vertices)
            center += v;
        center /= vertices.Length;
        // assign each normal as a direction from center
        Vector3[] n = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            n[i] = (vertices[i] - center).normalized;
        }
        return n;
    }

    public void AddToFront(Transform cursor)
    {
        int vertCount = vertices.Length;
        int triCount = triangles.Length;

        Vector3[] v = new Vector3[vertCount + verticesPerSegment]; 
        int[] t = new int[triCount + (6 * verticesPerSegment)];

        // Copy in existing data
        
        for (int i = 0; i < vertCount; i++)
        {
            v[i] = vertices[i];
        }

        for (int i = 0; i < triCount; i++)
        {
            t[i] = triangles[i];
        }

        // Transform the cross section's vertices by the cursor, and assign them to the end of vertices
        
        Vector3[] csV = crossSection.vertices;
        for (int i = vertCount; i < vertCount + verticesPerSegment; i++)
        {
            v[i] = cursor.TransformPoint(csV[i - vertCount]);
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

        // Assign all this back to Worm

        vertices = v;
        triangles = t;
    }

    public void RemoveFromBack(int amount)
    {
        // Determine current vertex count

        int vertCount = vertices.Length;
        int triCount = triangles.Length;

        // Create properly sized arrays to hold new mesh data

        Vector3[] v = new Vector3[vertCount - (verticesPerSegment * amount)];
        int[] t = new int[triCount - (6 * verticesPerSegment * amount)];

        // Copy over all data except for [amount] at the back

        for (int i = 0; i < vertCount - (verticesPerSegment * amount); i++)
        {
            v[i] = vertices[i + verticesPerSegment * amount];
        }

        for (int i = 0; i < triCount - (6 * verticesPerSegment * amount); i++)
        {
            t[i] = triangles[i];
        }

        vertices = v;
        triangles = t;
    }

    public bool ExceedsSegmentCount(int x)
    {
        return vertices.Length > x * verticesPerSegment;
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

    private Transform SnapToGround(Transform t)
    {
        Ray ray = new Ray(t.position + Vector3.up * 100f, Vector3.down);
        RaycastHit hitInfo;
        float maxDistance = 200f;
        int layerMask = LayerMask.GetMask("Terrain");
        Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);

        Vector3 p = t.position;
        p.y = hitInfo.point.y;
        t.position = p;

        return t;
    }
}