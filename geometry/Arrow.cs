using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreetDirectionViewer {
  
  abstract class Arrow {
    public abstract void Create(String name, Vector3 position, Vector3 direction, Material material, out GameObject headGameObject, out GameObject shaftGameObject);
  }

  class RoundArrow : Arrow {

    private readonly float shaftLength;
    private readonly Mesh headMesh, shaftMesh;
    private readonly Vector3 arrowOffset;

    public RoundArrow(Options options) {
      this.shaftLength = options.arrowDimensions.shaftLength;
      this.arrowOffset = options.arrowOffset;

      this.headMesh = ConeMesh.Create(
          0,
          options.arrowDimensions.headRadius,
          options.arrowDimensions.headLength);

      this.shaftMesh = ConeMesh.Create(
          options.arrowDimensions.shaftRadius,
          options.arrowDimensions.shaftRadius,
          options.arrowDimensions.shaftLength);
    }

    public override void Create(String name, Vector3 position, Vector3 direction, Material material, out GameObject headGameObject, out GameObject shaftGameObject) {
      position += arrowOffset;
      Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);
      headGameObject = GameObjectUtils.Create(name, headMesh, position, rotation, material);
      shaftGameObject = GameObjectUtils.Create(name, shaftMesh, position - direction.normalized * shaftLength, rotation, material);
    }
  }

  class FlatArrow : Arrow {

    private readonly float shaftLength, headLength;
    private readonly Mesh headMesh, shaftMesh;
    private readonly Vector3 arrowOffset;

    public FlatArrow(Options options) {
      this.shaftLength = options.flatArrowDimensions.shaftLength;
      this.headLength = options.flatArrowDimensions.headLength;
      this.arrowOffset = options.flatArrowOffset;

      this.headMesh = TriangleMesh.Create(
          options.flatArrowDimensions.headLength,
          options.flatArrowDimensions.arrowHeight,
          options.flatArrowDimensions.headWidth);

      this.shaftMesh = BoxMesh.Create(
          options.flatArrowDimensions.shaftWidth,
          options.flatArrowDimensions.arrowHeight,
          options.flatArrowDimensions.shaftLength);
    }

    public override void Create(String name, Vector3 position, Vector3 direction, Material material, out GameObject headGameObject, out GameObject shaftGameObject) {
      position += arrowOffset;
      Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
      headGameObject = GameObjectUtils.Create(name, headMesh, position, rotation, material);
      shaftGameObject = GameObjectUtils.Create(name, shaftMesh, position - direction.normalized * (shaftLength / 2), rotation, material);
    }
  }
}
