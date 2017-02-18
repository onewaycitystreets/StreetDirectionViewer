using ColossalFramework;
using ColossalFramework.Plugins;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace StreetDirectionViewer {
  class Materials {

    private static readonly Shader undergroundArrowShader;

    static Materials() {

      foreach (AssetBundle bundle in Resources.FindObjectsOfTypeAll <AssetBundle>()) {
        if (bundle.name == "undergroundonewaystreetarrowshader") {
          bundle.Unload(true);
          CitiesConsole.Log("unloaded bundle");
          break;
        }
      }

      PluginManager.PluginInfo pluginInfo = Singleton<PluginManager>.instance.FindPluginInfo(Assembly.GetAssembly(typeof (Materials)));
      AssetBundle shaders = AssetBundle.LoadFromFile(pluginInfo.modPath + "\\undergroundonewaystreetarrowshader");
      if (shaders == null) {
        CitiesConsole.Log("Could not load underground arrow shaders asset bundle");
        return;
      }

      undergroundArrowShader = shaders.LoadAsset<Shader>("UndergroundOnewayStreetArrowShader.shader");
      if (undergroundArrowShader == null || !undergroundArrowShader.isSupported) {
        CitiesConsole.Log("Could not load underground arrow shader");
      }
    }

    public static Material CreateAlwaysOnTop(Color color) {
      Material m = new Material(undergroundArrowShader == null ?  Shader.Find("Diffuse") : undergroundArrowShader);
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
