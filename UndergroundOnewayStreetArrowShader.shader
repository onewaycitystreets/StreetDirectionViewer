Shader "UndergroundOnewayStreetArrowShader" {
  Properties {
    _Color("Main Color", Color) = (1, 1, 1, 1)
  }

  SubShader{
    Pass {
      Lighting on
      ZTest Always
      ZWrite Off
      Material{ Diffuse[_Color] Ambient[_Color] }
    }
  }
}