using UnityEngine;

namespace StreetDirectionViewer {
  class Materials {

    public static readonly Material RED_MATERIAL = new Material(Shader.Find("Diffuse"));
    public static readonly Material ORANGE_MATERIAL = new Material(Shader.Find("Diffuse"));
    public static readonly Material YELLOW_MATERIAL = new Material(Shader.Find("Diffuse"));
    public static readonly Material GREEN_MATERIAL = new Material(Shader.Find("Diffuse"));
    public static readonly Material CYAN_MATERIAL = new Material(Shader.Find("Diffuse"));
    public static readonly Material BLUE_MATERIAL = new Material(Shader.Find("Diffuse"));
    public static readonly Material MAGENTA_MATERIAL = new Material(Shader.Find("Diffuse"));
    public static readonly Material BLACK_MATERIAL = new Material(Shader.Find("Diffuse"));
    public static readonly Material GRAY_MATERIAL = new Material(Shader.Find("Diffuse"));
    public static readonly Material WHITE_MATERIAL = new Material(Shader.Find("Diffuse"));

    public static readonly Material[] materials = {
      Materials.RED_MATERIAL,    // 0
      Materials.ORANGE_MATERIAL, // 1
      Materials.YELLOW_MATERIAL, // 2
      Materials.GREEN_MATERIAL,  // 3
      Materials.CYAN_MATERIAL,   // 4
      Materials.BLUE_MATERIAL,   // 5
      Materials.MAGENTA_MATERIAL,// 6
      Materials.BLACK_MATERIAL,  // 7
      Materials.GRAY_MATERIAL,   // 8
      Materials.WHITE_MATERIAL,  // 9
    };

    static Materials() {
      RED_MATERIAL.color = Color.red;
      ORANGE_MATERIAL.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
      YELLOW_MATERIAL.color = Color.yellow;
      GREEN_MATERIAL.color = Color.green;
      CYAN_MATERIAL.color = Color.cyan;
      BLUE_MATERIAL.color = Color.blue;
      MAGENTA_MATERIAL.color = Color.magenta;
      BLACK_MATERIAL.color = Color.black;
      GRAY_MATERIAL.color = Color.Lerp(Color.black, Color.white, 0.5f);
      WHITE_MATERIAL.color = Color.white;
    }

  }
}
