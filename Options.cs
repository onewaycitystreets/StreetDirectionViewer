using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace StreetDirectionViewer {

  public class Options {

    public class ArrowDimensions {
      public float headLength = 12;
      public float headRadius = 6;
      public float shaftLength = 20;
      public float shaftRadius = 2;
    }

    public class FlatArrowDimensions {
      public float arrowHeight = 4;
      public float headLength = 12;
      public float headWidth = 12;
      public float shaftLength = 20;
      public float shaftWidth = 4;
    }

    public enum ArrowType {
      Round, Flat
    }

    // This class really ought to be immutable,
    // but Vector3 and Color look like they're
    // mutable, so...

    public Color arrowColor = Color.green;
    public Color errorArrowColor = Color.magenta;
    public Vector3 arrowOffset = new Vector3(0, 6, 0);
    public Vector3 flatArrowOffset = new Vector3(0, 2, 0);
    public ArrowType arrowType = new ArrowType();
    public ArrowDimensions arrowDimensions = new ArrowDimensions();
    public FlatArrowDimensions flatArrowDimensions = new FlatArrowDimensions();
    public bool hideWithRoadsPanel = true;
  }

  public static class OptionsLoader {
    public static Options CurrentOptions {
      get;
      private set;
    }

    private static FileSystemWatcher fileSystemWatcher;

    private const String FILE_NAME = "one_way_street_arrows_options.xml";

    public delegate void OptionsChangedHandler();
    public static event OptionsChangedHandler eventOptionsChanged;

    public static void Load() {
      try {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
        using (StreamReader streamReader = new StreamReader(FILE_NAME)) {
          CurrentOptions = (Options)xmlSerializer.Deserialize(streamReader);
        }
      } catch (FileNotFoundException) {
        CurrentOptions = new Options();
        Save();
      } catch (Exception e) {
        CitiesConsole.Error(e);
        CurrentOptions = new Options();
      }

      if (fileSystemWatcher == null) {
        fileSystemWatcher = new FileSystemWatcher(".", FILE_NAME);
        fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
        fileSystemWatcher.Changed += fileSystemWatcher_Changed;
        fileSystemWatcher.EnableRaisingEvents = true;
      }
    }

    private static void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e) {
      eventOptionsChanged();
    }

    public static void Save() {
      try {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
        using (StreamWriter streamWriter = new StreamWriter(FILE_NAME)) {
          xmlSerializer.Serialize(streamWriter, CurrentOptions);
        }
      } catch (Exception e) {
        CitiesConsole.Error(e);
      }
    }

    public static void OnRelease() {
      if (fileSystemWatcher != null) {
        fileSystemWatcher.EnableRaisingEvents = false;
        fileSystemWatcher = null;
      }
    }
  }

}
