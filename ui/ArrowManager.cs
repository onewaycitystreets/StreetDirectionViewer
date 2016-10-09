using ColossalFramework;
using ColossalFramework.Math;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StreetDirectionViewer {
  class ArrowManager {

    private const String ARROW_OBJECT_NAME = "OneWayStreetArrow";

    private readonly List<GameObject> arrowGameObjects = new List<GameObject>();

    private bool arrowsActive;

    public bool DestroyArrowsByGameObjectName() {

      var meshesToDelete = new List<GameObject>();
      foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()) {
        if (gameObj.name == ARROW_OBJECT_NAME) {
          meshesToDelete.Add(gameObj);
        }
      }

      if (meshesToDelete.Count > 0) {
        CitiesConsole.Log("Destroying old meshes");
        foreach (GameObject oldMesh in meshesToDelete) {
          GameObject.Destroy(oldMesh);
        }
      }
      arrowGameObjects.Clear();
      arrowsActive = false;
      return meshesToDelete.Count > 0;
    }

    public void DestroyArrows() {
      foreach (GameObject arrowGameObject in arrowGameObjects) {
        GameObject.Destroy(arrowGameObject);
      }
      arrowGameObjects.Clear();
      arrowsActive = false;
    }

    public void CreateArrows() {

      NetManager netManager = Singleton<NetManager>.instance;
      
      InfoManager infoManager = Singleton<InfoManager>.instance;
      bool showUndergroundArrows = (infoManager.CurrentMode == InfoManager.InfoMode.Traffic);

      SimulationManager simManager = Singleton<SimulationManager>.instance;
      bool leftHandDrive = simManager.m_metaData.m_invertTraffic == SimulationMetaData.MetaBool.True;

      IEnumerable<Array16Item<NetSegment>> segments;
      try {
        segments = ArrayUtils.Array16Enumerable(netManager.m_segments);
      } catch (Exception e) {
        CitiesConsole.Error(e);
        return;
      }

      Options options = OptionsLoader.CurrentOptions;

      Arrow arrow = null;
      if (options.arrowType == Options.ArrowType.Round) {
        arrow = new RoundArrow(options);
      } else if (options.arrowType == Options.ArrowType.Flat) {
        arrow = new FlatArrow(options);
      }

      Material normalArrowMaterial = Materials.CreateDiffuse(options.arrowColor);
      Material errorArrowMaterial = Materials.CreateDiffuse(options.errorArrowColor);
      Material normalUndergroundArrowMaterial = Materials.CreateAlwaysOnTop(options.undergroundArrowColor);
      Material errorUndergroundArrowMaterial = Materials.CreateAlwaysOnTop(options.undergroundErrorArrowColor);

      Bezier3 bezier3 = new Bezier3();

      foreach (var item in segments) {
        NetSegment segment = item.item;
        ushort segmentId = item.index;

        if (segment.m_flags.IsFlagSet(NetSegment.Flags.Deleted)) {
          continue;
        }

        if (IsHarbor(segment)) {
          continue;
        }

        if (!IsOneWay(segment)) {
          continue;
        }

        NetNode startNode = netManager.m_nodes.m_buffer[segment.m_startNode];
        NetNode endNode = netManager.m_nodes.m_buffer[segment.m_endNode];

        bool underground = IsUnderground(startNode, endNode);
        if (!showUndergroundArrows && underground) {
          continue;
        }

        Vector3 startPosition = startNode.m_position;
        Vector3 endPosition = endNode.m_position;

        Vector3 direction = endPosition - startPosition;

        bool inverted = segment.m_flags.IsFlagSet(NetSegment.Flags.Invert);
        if (inverted) {
          direction = -direction;
        }

        if (leftHandDrive) {
          direction = -direction;
        }

        bezier3.a = startPosition;
        bezier3.d = endPosition;
        bool smoothStart = (startNode.m_flags & NetNode.Flags.Middle) != NetNode.Flags.None;
        bool smoothEnd = (endNode.m_flags & NetNode.Flags.Middle) != NetNode.Flags.None;
        NetSegment.CalculateMiddlePoints(bezier3.a, segment.m_startDirection, bezier3.d, segment.m_endDirection, smoothStart, smoothEnd, out bezier3.b, out bezier3.c);
        Vector3 arrowPosition = bezier3.Position(0.5f);

        bool suspicious = IsSuspicious(segmentId, startNode, endNode, netManager);

        Material arrowMaterial;
        if (underground) {
          arrowMaterial = suspicious ? errorUndergroundArrowMaterial : normalUndergroundArrowMaterial;
        } else {
          arrowMaterial = suspicious ? errorArrowMaterial : normalArrowMaterial;
        }

        createArrow(arrow, arrowPosition, direction, arrowMaterial);
      }
      arrowsActive = true;
    }

    public bool getArrowsActive() {
      return arrowsActive;
    }

    private static bool IsUnderground(NetNode startNode, NetNode endNode) {
      return startNode.m_flags.IsFlagSet(NetNode.Flags.Underground) && endNode.m_flags.IsFlagSet(NetNode.Flags.Underground);
    }

    /// <summary>
    /// Performs a heuristic on the segment to determine if it is "suspicious".
    /// A segment is suspicious if it is one way, but one of its ends is a
    /// two-way segment.
    /// </summary>
    private static bool IsSuspicious(ushort segmentId, NetNode startNode, NetNode endNode, NetManager netManager) {
      return IsSuspicious(segmentId, startNode, netManager) || IsSuspicious(segmentId, endNode, netManager);
    }

    private static bool IsSuspicious(ushort segmentId, NetNode node, NetManager netManager) {
      // A node with 2 segments just connects two roads. A node with more than 2 segments
      // is an intersection, so that's not suspicious.
      if (node.CountSegments() == 2) {
        ushort otherSegmentId = ushort.MaxValue;
        for (int i = 0; i < 8; i++) {
          ushort id = node.GetSegment(i);
          if (id != 0 && id != segmentId) {
            otherSegmentId = id;
            break;
          }
        }
        if (otherSegmentId == ushort.MaxValue) {
          // TODO: There was a logging statement here to log an error,
          // but users may have been seeing this too much.
          return false;
        }
        NetSegment otherSegment = netManager.m_segments.m_buffer[otherSegmentId];
        if (!IsOneWay(otherSegment)) {
          return true;
        }
      }
      return false;
    }

    private static bool IsOneWay(NetSegment segment) {
      return segment.Info.m_hasBackwardVehicleLanes ^ segment.Info.m_hasForwardVehicleLanes;
    }

    private static bool IsHarbor(NetSegment segment) {
      foreach (NetInfo.Lane lane in segment.Info.m_lanes) {
        if (lane.m_vehicleType.IsFlagSet(VehicleInfo.VehicleType.Ship)) {
          return true;
        }
      }
      return false;
    }

    public void Update() {
      if (arrowGameObjects.Count > 0) {
        // TODO: If destroying all the arrows and re-creating them
        // ends up being too slow, then a map of segment -> arrow can be
        // used to update only the arrows that have changed. This would
        // still involve iterating over all of the segments.
        DestroyArrows();
        CreateArrows();
      }
    }

    private void createArrow(Arrow arrow, Vector3 position, Vector3 direction, Material material) {
      GameObject head, shaft;
      arrow.Create(ARROW_OBJECT_NAME, position, direction, material, out head, out shaft);
      arrowGameObjects.Add(head);
      arrowGameObjects.Add(shaft);
    }
  }
}
