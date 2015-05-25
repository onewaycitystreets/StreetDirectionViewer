using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace StreetDirectionViewer {
  public class BasicUIPanel : UIPanel {

    private const float titleBarHeight = 40f;

    public override void Start() {
      base.Start();
      isVisible = false;
      backgroundSprite = "MenuPanel";
      canFocus = true;
      isInteractive = true;

      CreateDragHandle();
      CreateCloseButton();
      CreatePanelTitle();
    }

    protected void CreateDragHandle() {
      var dragHandleObject = new GameObject("DragHandler");
      dragHandleObject.transform.parent = this.transform;
      dragHandleObject.transform.localPosition = Vector3.zero;
      var dragHandle = dragHandleObject.AddComponent<UIDragHandle>();
      dragHandle.width = this.width;
      dragHandle.height = titleBarHeight;
      dragHandle.zOrder = 0;
    }

    protected void CreatePanelTitle() {
      var titleObject = new GameObject("Title");
      titleObject.transform.parent = this.transform;
      titleObject.transform.localPosition = Vector3.zero;
      var title = titleObject.AddComponent<UILabel>();
      title.text = "One-Way Street Arrows Settings";
      title.textAlignment = UIHorizontalAlignment.Center;
      title.position = new Vector3((this.width / 2f) - (title.width / 2f), -20f + (title.height / 2f));
    }

    protected void CreateCloseButton() {
      var closeButtonObject = new GameObject("CloseButton");
      closeButtonObject.transform.parent = this.transform;
      closeButtonObject.transform.localPosition = Vector3.zero;
      var closeButton = closeButtonObject.AddComponent<UIButton>();
      closeButton.width = 32f;
      closeButton.height = 32f;
      closeButton.normalBgSprite = "buttonclose";
      closeButton.hoveredBgSprite = "buttonclosehover";
      closeButton.pressedBgSprite = "buttonclosepressed";
      closeButton.relativePosition = new Vector3(this.width - closeButton.width, 2f);
      closeButton.eventClick += (UIComponent component, UIMouseEventParameter eventParam) => {
        this.isVisible = false;
      };
    }
  }
}
