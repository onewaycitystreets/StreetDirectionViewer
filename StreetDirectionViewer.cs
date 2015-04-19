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

    public override void OnCreated(IThreading threading) {
      this.threading = threading;
      OptionsLoader.Load();
      OptionsLoader.eventOptionsChanged += () => {
        threading.QueueMainThread(() => {
          CitiesConsole.Log("Reloading settings");
          OptionsLoader.Load();
          arrowManager.Update(); 
        });
      };
    }

    public override void OnReleased() {
      OptionsLoader.OnRelease();
    }

    public ThreadingExtension() {
      arrowManager = new ArrowManager();
      streetDirectionViewerUI = new StreetDirectionViewerUI(arrowManager);
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
          streetDirectionViewerUI.CreateUI();
        } catch (Exception e) {
          CitiesConsole.Error(e);
        }
        uiCreated = true;
      }
    }

    public override void OnBeforeSimulationTick() {
      if (Singleton<NetManager>.instance.m_nodesUpdated || Singleton<NetManager>.instance.m_segmentsUpdated) {
        // Don't block the simulation thread.
        threading.QueueMainThread(() => { arrowManager.Update(); });
      }
    }

    public void OnCreated(ILoading loading) {

    }

    public void OnLevelLoaded(LoadMode mode) {

    }

    public void OnLevelUnloading() {
      arrowManager.DestroyArrows();
    }
  }

}
