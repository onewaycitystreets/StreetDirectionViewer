using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StreetDirectionViewer {
  class ArrowManager {

    // For debugging.
    private const bool SHOW_ALL_LANES = false;

    private const String ARROW_OBJECT_NAME = "OneWayStreetArrow";
    private readonly Material ARROW_MATERIAL = Materials.GREEN_MATERIAL;
    private readonly Vector3 ARROW_OFFSET = new Vector3(0, 6, 0);

    private readonly List<GameObject> arrowGameObjects = new List<GameObject>();

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
      return meshesToDelete.Count > 0;
    }

    public void DestroyArrows() {
      foreach (GameObject arrowGameObject in arrowGameObjects) {
        GameObject.Destroy(arrowGameObject);
      }
      arrowGameObjects.Clear();
    }

    public void CreateArrows() {

      NetManager netManager = Singleton<NetManager>.instance;

      IEnumerable<NetSegment> segments;
      try {
        segments = ArrayUtils.Array16Enumerable(netManager.m_segments);
      } catch (Exception e) {
        CitiesConsole.Error(e);
        return;
      }

      foreach (NetSegment segment in segments) {
        if ((segment.m_flags & NetSegment.Flags.Deleted) != NetSegment.Flags.None) {
          continue;
        }

        bool oneway = segment.Info.m_hasBackwardVehicleLanes ^ segment.Info.m_hasForwardVehicleLanes;
        if (oneway) {
          NetNode startNode = netManager.m_nodes.m_buffer[segment.m_startNode];
          NetNode endNode = netManager.m_nodes.m_buffer[segment.m_endNode];

          Vector3 startPosition = startNode.m_position;
          Vector3 endPosition = endNode.m_position;

          Vector3 direction = endPosition - startPosition;

          bool inverted = ((segment.m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None);
          if (inverted)
            direction = -direction;

          if (SHOW_ALL_LANES) {
            for (uint i = segment.m_lanes, j = 0; i != 0; i = netManager.m_lanes.m_buffer[i].m_nextLane, j++) {
              Vector3 arrowPosition = netManager.m_lanes.m_buffer[i].CalculatePosition(0.5f);
              createArrow(arrowPosition + new Vector3(0, j * 2), direction, Materials.materials[j]);
            }
          } else {
            Vector3 arrowPosition;
            try {
              arrowPosition = GetMiddlePosition(netManager, segment);
            } catch (ArgumentException e) {
              CitiesConsole.Error("Could not determine middle lane: " + e);
              // Fallback to the midpoint between the start and end nodes.
              // This won't look right for certain curved roads, but
              // better than nothing.
              arrowPosition = (endPosition - startPosition) / 2;
            }

            createArrow(arrowPosition, direction, ARROW_MATERIAL);
          }
        }

      }
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

    private void createArrow(Vector3 position, Vector3 direction, Material material) {
      GameObject head, shaft;
      Arrow.Create(ARROW_OBJECT_NAME, position + ARROW_OFFSET, direction, material, out head, out shaft);
      arrowGameObjects.Add(head);
      arrowGameObjects.Add(shaft);
    }

    private class RoadCenter {
      public readonly int left, right, center;

      public RoadCenter(int left, int right) {
        this.left = left;
        this.right = right;
        this.center = -1;
      }

      public RoadCenter(int center) {
        this.center = center;
        this.left = -1;
        this.right = -1;
      }
    }

    // This maps the number of lanes (of all types) a road has to the
    // index in its linked list of lanes for its center lane.
    // This was determined empirically by drawing colored arrows
    // over every lane and matching the colors to the index, so
    // hopefully this remains stable.
    private static readonly RoadCenter[] ROAD_CENTER_MAP = {
      /*  0 */ null,
      /*  1 */ null,
      /*  2 */ null,
      /*  3 */ new RoadCenter(0),
      /*  4 */ new RoadCenter(2, 3),
      /*  5 */ new RoadCenter(1),
      /*  6 */ new RoadCenter(4, 5),
      /*  7 */ null,
      /*  8 */ new RoadCenter(2, 3),
      /*  9 */ null,
      /* 10 */ new RoadCenter(8, 9),
    };

    private static Vector3 GetMiddlePosition(NetManager netManager, NetSegment segment) {

      if (segment.Info.m_lanes.Length > ROAD_CENTER_MAP.Length) {
        throw new ArgumentException("Road has too many lanes: " + segment.Info.m_lanes.Length);
      }

      RoadCenter roadCenter = ROAD_CENTER_MAP[segment.Info.m_lanes.Length];
      if (roadCenter == null) {
        throw new ArgumentException("Road has an unexpected number of lanes: " + segment.Info.m_lanes.Length);
      }

      if (segment.Info.m_lanes.Length == 6) {
        // Bizarrely, 3 lane highways and two-lane one-way roads both have 6 lanes.
        // The 3 lane highways have 2 lanes in the middle lane.
        // The easiest way to differentiate these seems to be to check the first lane:
        // For two-lane one-way roads, the first lane is a sidewalk.
        if ((segment.Info.m_lanes[0].m_laneType & NetInfo.LaneType.Pedestrian) != NetInfo.LaneType.None) {
          roadCenter = ROAD_CENTER_MAP[6];
        } else {
          roadCenter = ROAD_CENTER_MAP[5];
        }
      }

      if (roadCenter.center == -1) {
        Vector3 left = getNthLane(netManager, segment, roadCenter.left).CalculatePosition(0.5f);
        Vector3 right = getNthLane(netManager, segment, roadCenter.right).CalculatePosition(0.5f);
        return (left + right) / 2;
      } else {
        return getNthLane(netManager, segment, roadCenter.center).CalculatePosition(0.5f);
      }
    }

    private static NetLane getNthLane(NetManager netManager, NetSegment segment, int n) {

      uint segmentNum = segment.m_lanes;
      for (int i = 0; i < n; i++) {
        segmentNum = netManager.m_lanes.m_buffer[segmentNum].m_nextLane;
      }
      return netManager.m_lanes.m_buffer[segmentNum];
    }
  }
}
