using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreetDirectionViewer {
  public class TriangleMesh {

    /// <param name="x">Length in the X direction</param>
    /// <param name="y">Length in the Y direction</param>
    /// <param name="z">Length in the Z direction</param>
    public static Mesh Create(float x, float y, float z) {
      Mesh mesh = new Mesh();

      #region Vertices
      // Bottom verticies
      Vector3 p0 = new Vector3(       0, -y * .5f, z);
      Vector3 p1 = new Vector3( x * .5f, -y * .5f, 0);
      Vector3 p2 = new Vector3(-x * .5f, -y * .5f, 0);

      // Top verticies
      Vector3 p3 = new Vector3(       0, y * .5f, z);
      Vector3 p4 = new Vector3( x * .5f, y * .5f, 0);
      Vector3 p5 = new Vector3(-x * .5f, y * .5f, 0);

      Vector3[] vertices = new Vector3[] {
        p0, p1, p2, p3, p4, p5
      };
      #endregion

      #region Triangles
      int[] triangles = new int[] {
	      // Bottom
        2, 1, 0,

        // Left
        5, 2, 0,
        0, 3, 5,

        // Right
        0, 1, 4,
        4, 3, 0,

        // Back
        1, 2, 5,
        5, 4, 1,

        // Top
        3, 4, 5
      };
      #endregion

      mesh.vertices = vertices;
      //mesh.uv = uvs;
      mesh.triangles = triangles;

      mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      mesh.Optimize();

      return mesh;
    }
  }
}
