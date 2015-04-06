using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ColossalFramework.UI;

namespace StreetDirectionViewer {
  public class UIUtils {

    public static T Find<T>(String name) where T : UIComponent {
      foreach (T go in UnityEngine.Object.FindObjectsOfType<T>()) {
        if (go.name.Contains(name)) {
          return go;
        }
      }
      return default (T);
    }
  }
}
