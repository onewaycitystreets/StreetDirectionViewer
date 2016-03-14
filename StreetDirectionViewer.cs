using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using UnityEngine;
using ColossalFramework;
using System.Reflection;
using ColossalFramework.Threading;
using System.Collections;

namespace StreetDirectionViewer {

  public class StreetDirectionViewer : IUserMod {
    public string Description {
      get { return "One-Way Street Arrows"; }
    }

    public string Name {
      get { return "One-Way Street Arrows"; }
    }
  }

  public class ThreadingExtension : ThreadingExtensionBase, ILoadingExtension {

    private IThreading threading;
    private readonly ArrowManager arrowManager;
    private readonly StreetDirectionViewerUI streetDirectionViewerUI;

    private bool uiCreated = false;
    private InfoManager.InfoMode previousInfoMode;

    public ThreadingExtension() {
      arrowManager = new ArrowManager();
      streetDirectionViewerUI = new StreetDirectionViewerUI(arrowManager);
    }

    public override void OnCreated(IThreading threading) {
      this.threading = threading;
      OptionsLoader.Load();
      OptionsLoader.eventOptionsChanged += () => {
        threading.QueueMainThread(() => {
          CitiesConsole.Log("Reloading settings");
          OptionsLoader.Load();
          arrowManager.Update();
          streetDirectionViewerUI.OptionsFileChanged();
        });
      };
    }

    public override void OnReleased() {
      OptionsLoader.OnRelease();
    }

    public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
      // OnCreate appears to be called too early to add things to the UI, so add the button
      // in OnUpdate.
      if (!uiCreated) {
        try {
          // If this was recompiled and is replacing an existing instance of the
          // mod, delete any orphaned arrows and recreate them if they were deleted.
          bool deletedArrows = arrowManager.DestroyArrowsByGameObjectName();
          if (deletedArrows) {
            arrowManager.CreateArrows();
          }
          streetDirectionViewerUI.appMode = threading.managers.loading.currentMode;
          streetDirectionViewerUI.CreateUI();
        } catch (Exception e) {
          CitiesConsole.Error(e);
        }
        uiCreated = true;
      }

      if (Input.GetKeyDown(OptionsLoader.CurrentOptions.arrowToggleShortcutKey)) {
        if (arrowManager.getArrowsActive()) {
          arrowManager.DestroyArrows();
          streetDirectionViewerUI.setShowStreetDirectionButtonState(false);
        } else {
          arrowManager.CreateArrows();
          streetDirectionViewerUI.setShowStreetDirectionButtonState(true);
        }
      }

    }

    public override void OnBeforeSimulationTick() {
      NetManager netManager = Singleton<NetManager>.instance;
      bool roadsUpdated = netManager.m_nodesUpdated || netManager.m_segmentsUpdated;

      InfoManager.InfoMode currentInfoMode = Singleton<InfoManager>.instance.CurrentMode;
      // Update the arrows if the info mode has come out of or gone into traffic mode.
      // When in traffic mode, tunnels are visible, so the underground arrows might
      // need to be drawn or hidden.
      bool trafficModeChanged = this.previousInfoMode != currentInfoMode &&
          (previousInfoMode == InfoManager.InfoMode.Traffic ||
           currentInfoMode == InfoManager.InfoMode.Traffic);
      this.previousInfoMode = currentInfoMode;

      if (roadsUpdated || trafficModeChanged) {
        // Don't block the simulation thread.
        threading.QueueMainThread(() => { arrowManager.Update(); });
      }
    }

    public void OnCreated(ILoading loading) {
      // This doesn't seem to be called. Maybe the plugin manager
      // sees that it implements IThreadingExtension first and doesn't
      // look for other interfaces?
    }

    public void OnLevelLoaded(LoadMode mode) {

    }

    public void OnLevelUnloading() {
      arrowManager.DestroyArrows();
    }
  }

}
