using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormWranglers.Worm
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class WormMesh : MonoBehaviour
    {
        [SerializeField] private float textureLength;

        private Mesh mesh;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        
        private List<Vector3> vertices;
        private List<Vector2> uvs;
        private List<int> triangles;

        private void Awake()
        {
            // Create an empty dynamic mesh and place it in the mesh filter

            mesh = new Mesh();
            mesh.MarkDynamic();
            mesh.name = "Dynamic Mesh";

            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            meshFilter.sharedMesh = mesh;

            // Create the data lists which will be used to update the mesh

            vertices = new List<Vector3>();
            uvs = new List<Vector2>();
            triangles = new List<int>();
        }

        // ====================================================================================================================
        // Adds one new segment of vertex / uv data using the given cursor.

        public void AddSegment(WormCursor cursor)
        {
            float vCoord = cursor.distanceTraveled / textureLength;

            foreach (var point in cursor.points)
            {
                vertices.Add(point.transform.position);
                uvs.Add(new Vector2(point.uCoord, vCoord));
            }
        }

        // ====================================================================================================================
        // "Shifts" on a new segment - making room in the vertex / uv data by removing one segment from the back.

        public void ShiftSegment(WormCursor cursor)
        {
            int vertsPerSeg = cursor.points.Count;
            vertices.RemoveRange(0, vertsPerSeg);
            uvs.RemoveRange(0, vertsPerSeg);
            
            AddSegment(cursor);
        }

        // ====================================================================================================================
        // Bakes the mesh with the current contents of the data lists.
        // The "retriangulate" parameter controls whether or not the triangle list is remade.
        // It needs to be remade after Add(), but not after Shift(), since the data is the same size.

        public void BakeMesh(WormCursor cursor, bool retriangulate)
        {
            if (retriangulate)
            {
                triangles.Clear();

                int vertsPerSeg = cursor.points.Count;
                for (int v = 0; v < vertsPerSeg - 1; v++)
                {
                    for (int s = 0; s < vertices.Count - vertsPerSeg; s += vertsPerSeg)
                    {
                        triangles.Add(s + v);
                        triangles.Add(s + v + vertsPerSeg);
                        triangles.Add(s + v + 1);
                        triangles.Add(s + v + vertsPerSeg + 1);
                        triangles.Add(s + v + 1);
                        triangles.Add(s + v + vertsPerSeg);
                    }
                }
            }

            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);

            // Unity is dumb and won't update the collision unless you do this here

            meshCollider.sharedMesh = mesh;
        }
    }
}