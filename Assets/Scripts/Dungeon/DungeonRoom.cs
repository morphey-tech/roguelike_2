using System;
using UnityEngine;
using System.Collections.Generic;

public class DungeonRoom : MonoBehaviour
{
  [Header("Fields")]
  [SerializeField] private Collider _floorCollider;

  public Vector2Int DungeonPoint { get; private set; }
  public Vector3 WorldPoint => transform.position;
  public Vector2 Size => new(_floorCollider.bounds.size.x, _floorCollider.bounds.size.z);
  public IReadOnlyCollection<DungeonRoom> Neighbors => _neighbors.Keys;

  private readonly Dictionary<DungeonRoom, Vector2> _neighbors = new(4);
  private readonly Dictionary<DungeonRoomDoor, Vector2> _doors = new(4);

  public void Init(Vector2Int dungeonPoint)
  {
    DungeonPoint = dungeonPoint;
    FindDoors();
  }

  private void FindDoors()
  {
    var doors = gameObject.GetComponentsInChildren<DungeonRoomDoor>();

    foreach (var door in doors)
    {
      var directionToDoor = door.transform.position - WorldPoint;
      var flatDirection = new Vector2(directionToDoor.x, directionToDoor.z);
      _doors.Add(door, flatDirection.normalized);
    }
  }

  public void AddNeighbor(DungeonRoom target)
  {
    var directionToTarget = target.transform.position - transform.position;
    var normalized = new Vector2(directionToTarget.x, directionToTarget.z).normalized;
    _neighbors.Add(target, normalized);
  }

  public void MakeConnectionWith(DungeonRoom neighbor)
  {
    if (_neighbors.ContainsKey(neighbor) == false)
      throw new ArgumentException($"{transform.name} doesn't have this neighbour - {neighbor.name}.");

    var directionToNeighbor = _neighbors[neighbor];
    OpenDoor(directionToNeighbor);

    var directionFromNeighbor = directionToNeighbor * -1;
    neighbor.OpenDoor(directionFromNeighbor);
  }

  private void OpenDoor(Vector2 direction)
  {
    if (TryGetDoorIn(direction, out var door))
      door.Open();
    else
      throw new NullReferenceException($"Can't find door of {transform.name} in direction {direction}.");
  }
  

  private bool TryGetDoorIn(Vector2 direction, out DungeonRoomDoor door)
  {
    door = null;

    foreach (var item in _doors)
    {
      if (item.Value != direction)
        continue;

      door = item.Key;
      break;
    }

    return door != null;
  }

  public int GetNeighboursCountWithConnection()
  {
    var count = 0;

    foreach (var neighborDirection in _neighbors.Values)
    {
      if (TryGetDoorIn(neighborDirection, out var door))
        if (door.IsOpen)
          count++;
    }

    return count;
  }


  public bool HasConnectionWith(DungeonRoom room)
  {
    var hasConnection = false;

    foreach (var neighbor in _neighbors)
    {
      if (neighbor.Key != room)
        continue;

      var directionToNeighbor = neighbor.Value;

      foreach (var door in _doors)
      {
        if (door.Value != directionToNeighbor)
          continue;

        hasConnection = door.Key.IsOpen;
      }
    }

    return hasConnection;
  }
}
