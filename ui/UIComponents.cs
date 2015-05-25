using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StreetDirectionViewer {

  class UIComponents {

    public static UILabel CreateLabel(Component parent, String text, float x, float y) {
      GameObject labelObject = new GameObject("Label");
      labelObject.transform.parent = parent.transform;
      labelObject.transform.localPosition = Vector3.zero;
      UILabel label = labelObject.AddComponent<UILabel>();
      label.text = text;
      label.relativePosition = new Vector3(x, y);
      label.autoSize = true;
      return label;
    }

    public static UICheckBox CreateCheckbox(Component parent, bool value, float x, float y) {
      GameObject checkBoxObject = new GameObject("CheckBox");
      checkBoxObject.transform.parent = parent.transform;
      UICheckBox checkBox = checkBoxObject.AddComponent<UICheckBox>();
      checkBox.autoSize = true;

      GameObject uncheckedObject = new GameObject("Unchecked");
      uncheckedObject.transform.parent = checkBox.transform;
      UISprite uncheckedSprite = uncheckedObject.AddComponent<UISprite>();
      uncheckedSprite.spriteName = "check-unchecked";
      uncheckedSprite.width = 16f;
      uncheckedSprite.height = 16f;
      uncheckedSprite.relativePosition = new Vector3(3f, 3f);

      GameObject checkedObject = new GameObject("Checked");
      checkedObject.transform.parent = uncheckedSprite.transform;
      UISprite checkedSprite = checkedObject.AddComponent<UISprite>();
      checkedSprite.spriteName = "check-checked";
      checkedSprite.width = 16f;
      checkedSprite.height = 16f;
      checkedSprite.relativePosition = Vector3.zero;
      checkBox.checkedBoxObject = checkedSprite;

      checkBox.isChecked = value;
      checkBox.relativePosition = new Vector3(x, y, 0);

      return checkBox;
    }

    public static UITextField CreateTextField(Component parent, float x, float y, float w, float h) {
      GameObject textFieldObject = new GameObject("TextField");
      textFieldObject.transform.parent = parent.transform;
      UITextField textField = textFieldObject.AddComponent<UITextField>();
      textField.relativePosition = new Vector3(x, y);
      textField.horizontalAlignment = UIHorizontalAlignment.Left;
      textField.color = Color.black;
      textField.cursorBlinkTime = 0.5f;
      textField.cursorWidth = 1;
      textField.selectionBackgroundColor = new Color(233, 201, 148, 255);
      textField.selectionSprite = "EmptySprite";
      textField.verticalAlignment = UIVerticalAlignment.Middle;
      textField.padding = new RectOffset(5, 0, 5, 0);
      textField.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
      textField.normalBgSprite = "TextFieldPanel";
      textField.hoveredBgSprite = "TextFieldPanelHovered";
      textField.focusedBgSprite = "TextFieldPanel";
      textField.size = new Vector3(w, h);
      textField.isInteractive = true;
      textField.enabled = true;
      textField.readOnly = false;
      textField.builtinKeyNavigation = true;
      return textField;
    }

    public static UIDropDown CreateDropDown(Component parent, float x, float y) {
      GameObject textFieldObject = new GameObject("DropDown");
      textFieldObject.transform.parent = parent.transform;
      UIDropDown dropDown = textFieldObject.AddComponent<UIDropDown>();
      dropDown.relativePosition = new Vector3(x, y);
      dropDown.listBackground = "GenericPanelLight";
      dropDown.itemHeight = 24;
      dropDown.itemHover = "ListItemHover";
      dropDown.itemHighlight = "ListItemHighlight";
      dropDown.normalBgSprite = "ButtonMenu";
      dropDown.listWidth = 150;
      dropDown.listHeight = 500;
      dropDown.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
      dropDown.popupColor = new Color32(45, 52, 61, 255);
      dropDown.popupTextColor = new Color32(170, 170, 170, 255);
      dropDown.verticalAlignment = UIVerticalAlignment.Middle;
      dropDown.horizontalAlignment = UIHorizontalAlignment.Center;
      dropDown.textFieldPadding = new RectOffset(8, 0, 4, 0);
      dropDown.itemPadding = new RectOffset(8, 0, 2, 0);
      dropDown.size = new Vector2(150, 24);

      var dropdownButton = dropDown.AddUIComponent<UIButton>();
      dropDown.triggerButton = dropdownButton;

      dropdownButton.text = "";
      dropdownButton.size = dropDown.size;
      dropdownButton.relativePosition = new Vector3(0.0f, 0.0f);
      dropdownButton.textVerticalAlignment = UIVerticalAlignment.Middle;
      dropdownButton.textHorizontalAlignment = UIHorizontalAlignment.Center;
      dropdownButton.normalFgSprite = "IconDownArrow";
      dropdownButton.hoveredFgSprite = "IconDownArrowHovered";
      dropdownButton.pressedFgSprite = "IconDownArrowPressed";
      dropdownButton.focusedFgSprite = "IconDownArrowFocused";
      dropdownButton.disabledFgSprite = "IconDownArrowDisabled";
      dropdownButton.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
      dropdownButton.horizontalAlignment = UIHorizontalAlignment.Right;
      dropdownButton.verticalAlignment = UIVerticalAlignment.Middle;
      dropdownButton.zOrder = 0;
      dropdownButton.textScale = 0.8f;

      return dropDown;
    }

    public static UIButton CreateButton(Component parent, float x, float y, float w, float h, String text) {

      GameObject buttonObject = new GameObject("Button");
      buttonObject.transform.parent = parent.transform;
      buttonObject.transform.localPosition = Vector3.zero;
      UIButton button = buttonObject.AddComponent<UIButton>();

      button.text = text;
      button.size = new Vector2(w, h);
      button.relativePosition = new Vector3(x, y);
      button.textVerticalAlignment = UIVerticalAlignment.Middle;
      button.textHorizontalAlignment = UIHorizontalAlignment.Center;
      button.normalFgSprite = "ButtonMenu";
      button.hoveredFgSprite = "ButtonMenuHovered";
      button.pressedFgSprite = "ButtonMenuPressed";
      button.disabledFgSprite = "ButtonMenuDisabled";
      button.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
      button.horizontalAlignment = UIHorizontalAlignment.Right;
      button.verticalAlignment = UIVerticalAlignment.Middle;

      return button;
    }

  }
}
