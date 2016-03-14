using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace StreetDirectionViewer {

  public class OptionsPanel : BasicUIPanel {

    public delegate void OptionsApplied();
    public event OptionsApplied eventOptionsApplied;

    private readonly Dictionary<String, UIComponent> optionComponents = new Dictionary<String,UIComponent>();

    public override void Start() {
      width = 370;
      base.Start();

      float y = CreateOptionsUI(4, 42, typeof(Options), "");
      UIButton defaultButton = UIComponents.CreateButton(this, 12, y + 8, 72, 32, "Default");
      UIButton applyButton = UIComponents.CreateButton(this, width - 72 - 8, y + 12, 72, 32, "Apply");

      UpdateValues(OptionsLoader.CurrentOptions);

      height = y + applyButton.height + 8 + 12;

      applyButton.eventClick += applyButton_eventClick;
      defaultButton.eventClick += defaultButton_eventClick;
    }

    void applyButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
      SaveValues(OptionsLoader.CurrentOptions);
      OptionsLoader.Save();
      eventOptionsApplied();
    }

    void defaultButton_eventClick(UIComponent component, UIMouseEventParameter eventParam) {
      UpdateValues(new Options());
    }

    private float CreateOptionsUI(float xOffset, float y, Type type, String keyPrefix) {
      
      const int deltaY = 26;

      foreach (FieldInfo field in type.GetFields()) {
        
        String labelString;
        System.Object[] optionAttrs = field.GetCustomAttributes(typeof(Option), true);
        if (optionAttrs.Length > 0) {
          labelString = ((Option) optionAttrs[0]).uiLabel;
        } else {
          labelString = CreateLabelName(field.Name);
        }

        UILabel label = UIComponents.CreateLabel(this, labelString + ":", xOffset, y);
        label.height = 24;
        label.verticalAlignment = UIVerticalAlignment.Bottom;
        label.textAlignment = UIHorizontalAlignment.Left;

        String key = keyPrefix + field.Name;
        float x = label.width + 4 + xOffset;
        Type fieldType = field.FieldType;
        if (fieldType == typeof(String) || fieldType == typeof(Single) || fieldType == typeof(Double)) {
          UIComponent component = UIComponents.CreateTextField(this, x, y, 32, 24);
          optionComponents.Add(key, component);
        } else if (fieldType == typeof(Color)) {
          UITextField textField = UIComponents.CreateTextField(this, x, y, 64, 24);
          textField.maxLength = 6;
          optionComponents.Add(key, textField);
        } else if (fieldType == typeof(Vector2)) {
          UITextField textFieldX = UIComponents.CreateTextField(this, x, y, 32, 24);
          UITextField textFieldY = UIComponents.CreateTextField(this, x + 32 + 2, y, 32, 24);
          optionComponents.Add(key + ".x", textFieldX);
          optionComponents.Add(key + ".y", textFieldY);
        } else if (fieldType == typeof(Vector3)) {
          UITextField textFieldX = UIComponents.CreateTextField(this, x, y, 32, 24);
          UITextField textFieldY = UIComponents.CreateTextField(this, x + 32 + 2, y, 32, 24);
          UITextField textFieldZ = UIComponents.CreateTextField(this, x + (32 + 2)*2, y, 32, 24);
          optionComponents.Add(key + ".x", textFieldX);
          optionComponents.Add(key + ".y", textFieldY);
          optionComponents.Add(key + ".z", textFieldZ);
        } else if (fieldType == typeof(bool)) {
          UICheckBox checkbox = UIComponents.CreateCheckbox(this, true, x, y);
          optionComponents.Add(key, checkbox);
        } else if (fieldType.IsEnum) {
          UIDropDown dropdown = UIComponents.CreateDropDown(this, x, y);
          foreach (String s in Enum.GetNames(fieldType)) {
            dropdown.AddItem(s);
          }
          optionComponents.Add(key, dropdown);
        } else if (fieldType.IsClass) {
          y = CreateOptionsUI(xOffset + 16, y + label.height, fieldType, field.Name + ".");
          y -= deltaY;
        } else {
          CitiesConsole.Error("Unknown option type: " + field + ", type: " + fieldType);
        }

        y += deltaY;
      }
      return y;
    }

    private static String CreateLabelName(String name) {
      StringBuilder sb = new StringBuilder();
      foreach (char c in name.ToCharArray()) {
        if (Char.IsUpper(c)) {
          sb.Append(' ');
          sb.Append(Char.ToLower(c));
        } else {
          sb.Append(Char.ToLower(c));
        }
      }
      sb[0] = sb[0].ToString().ToUpperInvariant()[0];
      return sb.ToString();
    }

    public void UpdateValues(System.Object obj) {
      UpdateValues(obj, "");
    }

    private void UpdateValues(System.Object obj, String keyPrefix) {
      foreach (FieldInfo field in obj.GetType().GetFields()) {

        Type fieldType = field.FieldType;
        if (fieldType == typeof(String) || fieldType == typeof(Single) || fieldType == typeof(Double)) {
          System.Object val = field.GetValue(obj);
          ((UITextField)optionComponents[keyPrefix + field.Name]).text = val.ToString();
        } else if (fieldType == typeof(Color)) {
          UITextField textField = (UITextField)optionComponents[keyPrefix + field.Name];
          // Force it to Color32 so that the ToString(String) works nicely.
          Color32 color = (Color)field.GetValue(obj);
          String hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
          textField.text = hex;
        } else if (fieldType == typeof(Vector2)) {
          Vector2 v = (Vector2)field.GetValue(obj);
          UITextField textFieldX = (UITextField)optionComponents[keyPrefix + field.Name + ".x"];
          UITextField textFieldY = (UITextField)optionComponents[keyPrefix + field.Name + ".y"];
          textFieldX.text = v.x.ToString();
          textFieldY.text = v.y.ToString();
        } else if (fieldType == typeof(Vector3)) {
          Vector3 v = (Vector3)field.GetValue(obj);
          UITextField textFieldX = (UITextField)optionComponents[keyPrefix + field.Name + ".x"];
          UITextField textFieldY = (UITextField)optionComponents[keyPrefix + field.Name + ".y"];
          UITextField textFieldZ = (UITextField)optionComponents[keyPrefix + field.Name + ".z"];
          textFieldX.text = v.x.ToString();
          textFieldY.text = v.y.ToString();
          textFieldZ.text = v.z.ToString();
        } else if (fieldType == typeof(bool)) {
          UICheckBox checkbox = (UICheckBox)optionComponents[keyPrefix + field.Name];
          checkbox.isChecked = (bool)field.GetValue(obj);
        } else if (fieldType.IsEnum) {
          UIDropDown dropdown = (UIDropDown)optionComponents[keyPrefix + field.Name];
          System.Object e = field.GetValue(obj);
          dropdown.selectedIndex = (int) e;
        } else if (fieldType.IsClass) {
          UpdateValues(field.GetValue(obj), keyPrefix + field.Name + ".");
        } else {
          CitiesConsole.Error("Unknown option type: " + field + ", type: " + fieldType);
        }
      }
    }

    public void SaveValues(System.Object obj) {
      SaveValues(obj, "");
    }

    private void SaveValues(System.Object obj, String keyPrefix) {
      foreach (FieldInfo field in obj.GetType().GetFields()) {

        Type fieldType = field.FieldType;
        if (fieldType == typeof(String)) {
          String text = ((UITextField)optionComponents[keyPrefix + field.Name]).text;
          field.SetValue(obj, text);
        } else if (fieldType == typeof(Single)) {
          String text = ((UITextField)optionComponents[keyPrefix + field.Name]).text;
          float f;
          try {
            f = Single.Parse(text);
          } catch {
            CitiesConsole.Error("Could not set {0} because '{1}' is not a number", field.Name, text);
            continue;
          }
          field.SetValue(obj, f);
        } else if (fieldType == typeof(Double)) {
          String text = ((UITextField)optionComponents[keyPrefix + field.Name]).text;
          double d;
          try {
            d = Single.Parse(text);
          } catch {
            CitiesConsole.Error("Could not set {0} because '{1}' is not a number", field.Name, text);
            continue;
          }
          field.SetValue(obj, d);
        } else if (fieldType == typeof(Color)) {
          UITextField textField = (UITextField)optionComponents[keyPrefix + field.Name];
          byte r, g, b;
          try {
            r = Convert.ToByte(textField.text.Substring(0, 2), 16);
            g = Convert.ToByte(textField.text.Substring(2, 2), 16);
            b = Convert.ToByte(textField.text.Substring(4, 2), 16);
          } catch {
            CitiesConsole.Error("Could not set {0} because '{1}' is not a number", field.Name, textField.text);
            continue;
          }
          Color c = new Color32(r, g, b, 0xFF);
          field.SetValue(obj, c);
        } else if (fieldType == typeof(Vector2)) {
          UITextField textFieldX = (UITextField)optionComponents[keyPrefix + field.Name + ".x"];
          UITextField textFieldY = (UITextField)optionComponents[keyPrefix + field.Name + ".y"];
          float x, y;
          try {
            x = Convert.ToSingle(textFieldX.text);
          } catch {
            CitiesConsole.Error("Could not set {0} because '{1}' is not a number", field.Name, textFieldX.text);
            continue;
          }
          try {
            y = Convert.ToSingle(textFieldY.text);
          } catch {
            CitiesConsole.Error("Could not set {0} because '{1}' is not a number", field.Name, textFieldY.text);
            continue;
          }
          Vector2 v = new Vector2(x, y);
          field.SetValue(obj, v);
        } else if (fieldType == typeof(Vector3)) {
          UITextField textFieldX = (UITextField)optionComponents[keyPrefix + field.Name + ".x"];
          UITextField textFieldY = (UITextField)optionComponents[keyPrefix + field.Name + ".y"];
          UITextField textFieldZ = (UITextField)optionComponents[keyPrefix + field.Name + ".z"];
          float x, y, z;
          try {
            x = Convert.ToSingle(textFieldX.text);
          } catch {
            CitiesConsole.Error("Could not set {0} because '{1}' is not a number", field.Name, textFieldX.text);
            continue;
          }
          try {
            y = Convert.ToSingle(textFieldY.text);
          } catch {
            CitiesConsole.Error("Could not set {0} because '{1}' is not a number", field.Name, textFieldY.text);
            continue;
          }
          try {
            z = Convert.ToSingle(textFieldZ.text);
          } catch {
            CitiesConsole.Error("Could not set {0} because '{1}' is not a number", field.Name, textFieldZ.text);
            continue;
          }
          Vector3 v = new Vector3(x, y, z);
          field.SetValue(obj, v);
        } else if (fieldType == typeof(bool)) {
          UICheckBox checkbox = (UICheckBox)optionComponents[keyPrefix + field.Name];
          CitiesConsole.Log("checked: " + checkbox.isChecked);
          field.SetValue(obj, checkbox.isChecked);
        } else if (fieldType.IsEnum) {
          UIDropDown dropdown = (UIDropDown)optionComponents[keyPrefix + field.Name];
          field.SetValue(obj, Enum.GetValues(fieldType).GetValue(dropdown.selectedIndex));
        } else if (fieldType.IsClass) {
          SaveValues(field.GetValue(obj), keyPrefix + field.Name + ".");
        } else {
          CitiesConsole.Error("Unknown option type: " + field + ", type: " + fieldType);
        }
      }
    }
  }
}
