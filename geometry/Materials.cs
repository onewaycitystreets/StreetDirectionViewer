using UnityEngine;

namespace StreetDirectionViewer {
  class Materials {

    private const string shader = @"
Shader ""Arrow Shader"" {
  Properties { _Color (""Main Color"", Color) = (1,1,1,1) }
  SubShader {
    Pass {
      Lighting On
      ZTest Always
      ZWrite Off
      Material { Diffuse [_Color] Ambient [_Color] }
      SetTexture [_Dummy] { combine primary double, primary }
    }
  }
}";

    public static Material CreateAlwaysOnTop(Color color) {
      Material m = new Material(shader);
      m.color = color;
      return m;
    }

    public static Material CreateDiffuse(Color color) {
      Material m = new Material(Shader.Find("Diffuse"));
      m.color = color;
      return m;
    }
  }
}
