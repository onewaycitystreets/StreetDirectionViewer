using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace StreetDirectionViewer {
  public class Options {

    public static readonly Options DEFAULT = new Options() {
      arrowColor = Color.green,
      errorArrowColor = Color.magenta,
      arrowOffset = new Vector3(0, 6, 0),
      arrowDimensions = new ArrowDimensions() {
        headLength = 12,
        headRadius = 6,
        shaftRadius = 2,
        shaftLength = 20
      }
    };

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
        CurrentOptions = DEFAULT;
        Save();
      } catch (Exception e) {
        CitiesConsole.Error(e);
        CurrentOptions = DEFAULT;
      }

      if (fileSystemWatcher == null) {
        fileSystemWatcher = new FileSystemWatcher(".", FILE_NAME);
        fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
        fileSystemWatcher.Changed += fileSystemWatcher_Changed;
        fileSystemWatcher.EnableRaisingEvents = true;
      }
    }

    private static void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e) {
      CitiesConsole.Log("Reloading settings");
      Load();
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

    public struct ArrowDimensions {
      public float headRadius;
      public float headLength;
      public float shaftRadius;
      public float shaftLength;
    }

    // This class really ought to be immutable,
    // but Vector3 and Color look like they're
    // mutable, so...

    public Color arrowColor;
    public Color errorArrowColor;
    public Vector3 arrowOffset;
    public ArrowDimensions arrowDimensions;
  }
}
