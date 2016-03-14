using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ColossalFramework.UI;
using System.Collections;
using ICities;


namespace StreetDirectionViewer {
  class StreetDirectionViewerUI {

    private const String BUTTON_NAME = "StreetDirectionViewerButton";
    private const String SETTINGS_PANEL_NAME = "StreetDirectionViewerOptionsPanel";

    private readonly ArrowManager arrowManager;
    public AppMode appMode { get; set; }
    private UIMultiStateButton showStreetDirectionButton;
    private bool showStreetDirectionButtonActive = false;

    private GameObject optionsPanelGameObject;
    private OptionsPanel optionsPanel;

    public StreetDirectionViewerUI(ArrowManager arrowManager) {
      this.arrowManager = arrowManager;
    }

    public void OptionsFileChanged() {
      SetButtonPositionAndSize();
      optionsPanel.UpdateValues(OptionsLoader.CurrentOptions);
    }

    public void CreateUI() {
      // For development, delete any existing panels.
      var oldSettingsPanels = UIUtils.FindAll<UIPanel>(SETTINGS_PANEL_NAME);
      if (oldSettingsPanels.Count > 0) {
        CitiesConsole.Log("destroying " + oldSettingsPanels.Count + " panels");
        foreach(var p in oldSettingsPanels) {
          GameObject.Destroy(p);
        }
      }

      this.optionsPanelGameObject = new GameObject(SETTINGS_PANEL_NAME);
      this.optionsPanel = this.optionsPanelGameObject.AddComponent<OptionsPanel>();
      this.optionsPanel.eventOptionsApplied += () => {
        arrowManager.Update();
        SetButtonPositionAndSize();
      };
      optionsPanel.transform.parent = UIView.GetAView().transform;

      // Delete the old button if it's there.
      UIButton oldButton = UIUtils.Find<UIButton>(BUTTON_NAME);
      if (oldButton != null) {
        GameObject.Destroy(oldButton);
      }

      // There are 3 (?) panels with the name "RoadsOptionPanel" in it.
      // "RoadsOptionPanel(RoadsPanel)" appears to be the real one.
      UIComponent roadsOptionPanel = UIUtils.Find<UIComponent>("RoadsOptionPanel(RoadsPanel)");
      if (roadsOptionPanel == null) {
        // The roads panel doesn't appear to be added to the UI when the game first starts,
        // so listen for the roads tab button in the main tool strip.
        UITabstrip mainToolStrip = UIUtils.Find<UITabstrip>("MainToolstrip");
        if (mainToolStrip == null) {
          CitiesConsole.Warning("Could not find MainToolstrip");
          return;
        }
        mainToolStrip.eventSelectedIndexChanged += mainToolStrip_eventSelectedIndexChanged;
      } else {
        CreateButton(roadsOptionPanel);
      }
    }

    private void mainToolStrip_eventSelectedIndexChanged(UIComponent component, int value) {
      // 0th tab is the roads tab
      if (value == 0) {
        // The roads panel doesn't apper to be added yet when eventSelectedIndexChanged
        // is called, so delay creating the button a bit.
        component.StartCoroutine(CreateButtonCoroutine((UITabstrip)component));
      }
    }

    private IEnumerator CreateButtonCoroutine(UITabstrip tabstrip) {
      // This will delay the execution of this coroutine.
      yield return null;

      if (CreateButton(null)) {
        tabstrip.eventSelectedIndexChanged -= mainToolStrip_eventSelectedIndexChanged;
      }
    }

    private bool CreateButton(UIComponent roadsOptionPanel) {

      if (roadsOptionPanel == null) {
       roadsOptionPanel = UIUtils.Find<UIComponent>("RoadsOptionPanel(RoadsPanel)");
      }

      if (roadsOptionPanel == null) {
        CitiesConsole.Warning("Could not find RoadsOptionPanel");
        return false;
      }

      GameObject showStreetDirectionButtonGameObject = new GameObject(BUTTON_NAME);
      showStreetDirectionButton = showStreetDirectionButtonGameObject.AddComponent<UIMultiStateButton>();
      roadsOptionPanel.AttachUIComponent(showStreetDirectionButtonGameObject);

      SetButtonPositionAndSize();

      showStreetDirectionButton.playAudioEvents = true;
      showStreetDirectionButton.name = BUTTON_NAME;
      showStreetDirectionButton.tooltip = "Show Directions of One-Way Roads. Right-click for options.";
      showStreetDirectionButton.isTooltipLocalized = false;
      showStreetDirectionButton.spritePadding = new RectOffset();
      // The user could activate the arrows through the keyboard shortcut,
      // so if the arrows are active, turn the button on.
      if (showStreetDirectionButtonActive) {
        setShowStreetDirectionButtonState(true);
      }

      // The sprite for the button can't be added to the InGame atlas, since the sprite data
      // seems to come from the atlas's texture, instead of the texture supplied by the SpriteData.
      // So a whole new atlas with the toggle button base images duplicated is needed.

      String[] iconNames = {
        "RoadArrowIcon",
        "Base",
        "BaseFocused",
        "BaseHovered",
        "BasePressed",
        "BaseDisabled",
      };
      showStreetDirectionButton.atlas = CreateTextureAtlas("icons.png", BUTTON_NAME + "Atlas", ((UIPanel)roadsOptionPanel).atlas.material, 36, 36, iconNames);

      // Background sprites

      // Disabled state
      UIMultiStateButton.SpriteSet backgroundSpriteSet0 = showStreetDirectionButton.backgroundSprites[0];
      backgroundSpriteSet0.normal = "Base";
      backgroundSpriteSet0.disabled = "Base";
      backgroundSpriteSet0.hovered = "BaseHovered";
      backgroundSpriteSet0.pressed = "Base";
      backgroundSpriteSet0.focused = "Base";

      // Enabled state
      showStreetDirectionButton.backgroundSprites.AddState();
      UIMultiStateButton.SpriteSet backgroundSpriteSet1 = showStreetDirectionButton.backgroundSprites[1];
      backgroundSpriteSet1.normal = "BaseFocused";
      backgroundSpriteSet1.disabled = "BaseFocused";
      backgroundSpriteSet1.hovered = "BaseFocused";
      backgroundSpriteSet1.pressed = "BaseFocused";
      backgroundSpriteSet1.focused = "BaseFocused";

      // Forground sprites

      // Disabled state
      UIMultiStateButton.SpriteSet foregroundSpriteSet0 = showStreetDirectionButton.foregroundSprites[0];
      foregroundSpriteSet0.normal = "RoadArrowIcon";
      foregroundSpriteSet0.disabled = "RoadArrowIcon";
      foregroundSpriteSet0.hovered = "RoadArrowIcon";
      foregroundSpriteSet0.pressed = "RoadArrowIcon";
      foregroundSpriteSet0.focused = "RoadArrowIcon";

      // Enabled state
      showStreetDirectionButton.foregroundSprites.AddState();
      UIMultiStateButton.SpriteSet foregroundSpriteSet1 = showStreetDirectionButton.foregroundSprites[1];
      foregroundSpriteSet1.normal = "RoadArrowIcon";
      foregroundSpriteSet1.disabled = "RoadArrowIcon";
      foregroundSpriteSet1.hovered = "RoadArrowIcon";
      foregroundSpriteSet1.pressed = "RoadArrowIcon";
      foregroundSpriteSet1.focused = "RoadArrowIcon";

      showStreetDirectionButton.eventActiveStateIndexChanged += showStreetDirectionButton_eventActiveStateIndexChanged;
      showStreetDirectionButton.eventVisibilityChanged += showStreetDirectionButton_eventVisibilityChanged;
      // For some reason, right clicks get eaten with eventClick, but eventMouseUp works.
      showStreetDirectionButton.eventMouseUp += showStreetDirectionButton_eventMouseUp; 

      return true;
    }

    void showStreetDirectionButton_eventMouseUp(UIComponent component, UIMouseEventParameter eventParam) {
      if (eventParam.buttons == UIMouseButton.Right) {
        optionsPanel.absolutePosition = showStreetDirectionButton.absolutePosition - new Vector3(0, optionsPanel.height + 10);
        optionsPanel.isVisible = !optionsPanel.isVisible;
        optionsPanel.BringToFront();
        
      }
    }

    private void SetButtonPositionAndSize() {
      // In editor mode, the Toggle Snapping button is all the way to the left on the screen,
      // so putting the arrow toggle button to the left of the Toggle Snapping button
      // will put the button off screen. So put the arrow toggle button to the right
      // of the Toggle Snapping button in editor mode.
      // The Toggle Zoning button from the Toggle Zoning mod is normally to the right
      // of Toggle Snapping, but that mod isn't active in editor mode.
      Vector3 buttonPosition, buttonSize;
      if (appMode == AppMode.Game) {
        buttonPosition = OptionsLoader.CurrentOptions.arrowToggleButtonPositionInGame;
        buttonSize = OptionsLoader.CurrentOptions.arrowToggleButtonSizeInGame;
      } else {
        buttonPosition = OptionsLoader.CurrentOptions.arrowToggleButtonPositionInEditor;
        buttonSize = OptionsLoader.CurrentOptions.arrowToggleButtonSizeEditor;
      }
      showStreetDirectionButton.relativePosition = buttonPosition;
      showStreetDirectionButton.size = buttonSize;
    }

    private void showStreetDirectionButton_eventActiveStateIndexChanged(UIComponent component, int value) {
      if (value == 0) {
        arrowManager.DestroyArrows();
      } else {
        arrowManager.CreateArrows();
      }
    }

    private void showStreetDirectionButton_eventVisibilityChanged(UIComponent component, bool visible) {
      if (OptionsLoader.CurrentOptions.hideWithRoadsPanel) {
        if (visible) {
          if (((UIMultiStateButton) component).activeStateIndex == 1) {
            arrowManager.CreateArrows();
          }
        } else {
          arrowManager.DestroyArrows();
        }
      }
    }

    public void setShowStreetDirectionButtonState(bool active) {
      if (showStreetDirectionButton != null) {
        showStreetDirectionButton.activeStateIndex = (active ? 1 : 0);
      }
      showStreetDirectionButtonActive = active;
    }

    private static UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames) {

      Texture2D texture = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false);
      texture.filterMode = FilterMode.Bilinear;

      { // LoadTexture
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        System.IO.Stream textureStream = assembly.GetManifestResourceStream("StreetDirectionViewer." + textureFile);

        byte[] buf = new byte[textureStream.Length];  //declare arraysize
        textureStream.Read(buf, 0, buf.Length); // read from stream to byte array

        texture.LoadImage(buf);

        texture.Apply(true, true);
      }

      UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();

      { // Setup atlas
        Material material = (Material)Material.Instantiate(baseMaterial);
        material.mainTexture = texture;

        atlas.material = material;
        atlas.name = atlasName;
      }

      // Add sprites
      for (int i = 0; i < spriteNames.Length; ++i) {
        float uw = 1.0f / spriteNames.Length;

        var spriteInfo = new UITextureAtlas.SpriteInfo() {
          name = spriteNames[i],
          texture = texture,
          region = new Rect(i * uw, 0, uw, 1),
        };

        atlas.AddSprite(spriteInfo);
      }

      return atlas;
    }
  }
}
