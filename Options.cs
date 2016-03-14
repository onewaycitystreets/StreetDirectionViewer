using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace StreetDirectionViewer {

  public class Option : System.Attribute {
    public String uiLabel;
  }

  public class Options {

    public class ArrowDimensions {
      public float headLength = 12;
      public float headRadius = 6;
      public float shaftLength = 20;
      public float shaftRadius = 2;
    }

    public class FlatArrowDimensions {
      public float arrowHeight = 2;
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
    public Color undergroundArrowColor = Color.green;
    public Color undergroundErrorArrowColor = Color.magenta;

    public ArrowType arrowType = ArrowType.Round;

    [Option(uiLabel = "Round arrow offset")]
    public Vector3 arrowOffset = new Vector3(0, 6, 0);
    [Option(uiLabel = "Round arrow dimensions")]
    public ArrowDimensions arrowDimensions = new ArrowDimensions();

    public Vector3 flatArrowOffset = new Vector3(0, 2, 0);
    public FlatArrowDimensions flatArrowDimensions = new FlatArrowDimensions();

    [Option(uiLabel = "Hide arrow toggle button with roads panel")]
    public bool hideWithRoadsPanel = true;

    public Vector2 arrowToggleButtonPositionInGame = new Vector2(-38, 0);
    public Vector2 arrowToggleButtonPositionInEditor = new Vector2(38, 0);
    public Vector2 arrowToggleButtonSizeInGame = new Vector2(36, 36);
    public Vector2 arrowToggleButtonSizeEditor = new Vector2(36, 36);
    public String arrowToggleShortcutKey = ".";
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
      // Don't do a Load() here. Loading should happen on the main thread to avoid any
      // concurrency issues.
      eventOptionsChanged();
    }

    public static void Save() {
      if (fileSystemWatcher != null) {
        fileSystemWatcher.EnableRaisingEvents = false;
      }
      try {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
        using (StreamWriter streamWriter = new StreamWriter(FILE_NAME)) {
          xmlSerializer.Serialize(streamWriter, CurrentOptions);
          streamWriter.Flush();
        }
      } catch (Exception e) {
        CitiesConsole.Error(e);
      }
      if (fileSystemWatcher != null) {
        fileSystemWatcher.EnableRaisingEvents = true;
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
