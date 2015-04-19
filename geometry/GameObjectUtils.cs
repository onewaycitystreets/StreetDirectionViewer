using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StreetDirectionViewer {
  class GameObjectUtils {
    public static GameObject Create(String name, Mesh mesh, Vector3 position, Quaternion rotation, Material material) {
      GameObject obj = new GameObject(name);

      MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
      meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

      MeshFilter filter = obj.AddComponent<MeshFilter>();
      filter.mesh = mesh;
      filter.name = name;

      obj.transform.localPosition = position;
      obj.transform.localRotation = rotation;
      obj.GetComponent<Renderer>().material = material;
      return obj;
    }
  }
}
