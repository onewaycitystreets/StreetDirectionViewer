using UnityEngine;

namespace StreetDirectionViewer {
  class Materials {

    public static Material Create(Color color) {
      Material m = new Material(Shader.Find("Diffuse"));
      m.color = color;
      return m;
    }

  }
}
