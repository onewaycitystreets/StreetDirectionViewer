using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreetDirectionViewer {
  class Arrow {

    private readonly float shaftLength;
    private readonly Mesh headMesh, shaftMesh;

    public Arrow(Options options) {
      this.shaftLength = options.arrowDimensions.shaftLength;

      this.headMesh = ConeMesh.Create(
          0,
          options.arrowDimensions.headRadius,
          options.arrowDimensions.headLength);

      this.shaftMesh = ConeMesh.Create(
          options.arrowDimensions.shaftRadius,
          options.arrowDimensions.shaftRadius,
          options.arrowDimensions.shaftLength);
    }

    public void Create(String name, Vector3 position, Vector3 direction, Material material, out GameObject headGameObject, out GameObject shaftGameObject) {
      Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);
      headGameObject = GameObjectUtils.Create(name, headMesh, position, rotation, material);
      shaftGameObject = GameObjectUtils.Create(name, shaftMesh, position - direction.normalized * shaftLength, rotation, material);
    }
  }
}
