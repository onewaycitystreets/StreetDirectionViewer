using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreetDirectionViewer {
  class Arrow {

    private const float headLength = 12;
    private const float headBottomRadius = headLength / 2;
    private const float shaftRadius = 2f;
    private const float shaftLength = 20;

    private static readonly Mesh HEAD_MESH = ConeMesh.Create(0, headBottomRadius, headLength);
    private static readonly Mesh SHAFT_MESH = ConeMesh.Create(shaftRadius, shaftRadius, shaftLength);

    public static void Create(String name, Vector3 position, Vector3 direction, Material material, out GameObject headGameObject, out GameObject shaftGameObject) {
      Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);
      headGameObject = GameObjectUtils.Create(name, HEAD_MESH, position, rotation, material);
      shaftGameObject = GameObjectUtils.Create(name, SHAFT_MESH, position - direction.normalized * shaftLength, rotation, material);
    }
  }
}
