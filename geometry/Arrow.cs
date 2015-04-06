using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreetDirectionViewer {
  class Arrow {

    private const float headLength = 8;
    private const float headBottomRadius = headLength / 2;
    private const float shaftRadius = 1.5f;
    private const float shaftLength = headLength * 2;

    private static readonly Mesh HEAD_MESH = ConeMesh.Create(0, headBottomRadius, headLength);
    private static readonly Mesh SHAFT_MESH = ConeMesh.Create(shaftRadius, shaftRadius, shaftLength);

    public static void Create(String name, Vector3 position, Vector3 direction, Material material, out GameObject headGameObject, out GameObject shaftGameObject) {
 
      Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);

      {
        headGameObject = new GameObject(name);

        headGameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = headGameObject.AddComponent<MeshFilter>();
        filter.mesh = HEAD_MESH;
        filter.name = name;

        headGameObject.transform.localPosition = position;
        headGameObject.transform.localRotation = rotation;
        headGameObject.GetComponent<Renderer>().material = material;
      }

      {
        shaftGameObject = new GameObject(name);

        shaftGameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = shaftGameObject.AddComponent<MeshFilter>();
        filter.mesh = SHAFT_MESH;
        filter.name = name;

        shaftGameObject.transform.localPosition = position - direction.normalized * shaftLength;
        shaftGameObject.transform.localRotation = rotation;
        shaftGameObject.GetComponent<Renderer>().material = material;
      }
    }


  }
}
