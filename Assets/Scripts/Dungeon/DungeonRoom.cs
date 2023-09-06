using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DungeonRoom : MonoBehaviour
{
  [Header("Room Parts")]
  [SerializeField] private GameObject _floor;
  [SerializeField] private List<DungeonRoomDoor> _doors;

  public Vector2Int DungeonPoint { get; private set; }
  public Vector3 WorldPoint => transform.position;
  public IReadOnlyCollection<DungeonRoom> Neighbors => _neighbors.Keys;

  private readonly Dictionary<DungeonRoom, Vector2> _neighbors = new(4);

  public Vector2 CalculateSize()
  {
    return _floor.TryGetComponent<Collider>(out var collider)
      ? (Vector2)new Vector3(collider.bounds.size.x, collider.bounds.size.z)
      : throw new InvalidOperationException("Floor must have collider for calculate room size.");
  }

  public void SetDungeonPosition(Vector2Int position)
  {
    DungeonPoint = position;
  }

  public void AddNeighbor(DungeonRoom target)
  {
    var directionToTarget = target.transform.position - transform.position;
    var normalized = new Vector2(directionToTarget.x, directionToTarget.z).normalized;
    _neighbors.Add(target, normalized);
  }

  public void MakeConnectionWith(DungeonRoom neighbour)
  {
    if (_neighbors.ContainsKey(neighbour) == false)
      throw new ArgumentException($"{transform.name} doesn't have this neighbour - {neighbour.name}.");

    var direction = _neighbors[neighbour];
    neighbour.OpenDoor(-direction);
    OpenDoor(direction);
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

    for (int i = 0; i < _doors.Count; i++)
    {
      var directionToDoor = _doors[i].transform.position - transform.position;
      var normalizedDirection = new Vector2(directionToDoor.x, directionToDoor.z).normalized;

      if (direction != normalizedDirection)
        continue;

      door = _doors[i];
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
    var directions = new[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    foreach (var direction in directions)
    {
      if (TryGetDoorIn(direction, out var door) == false)
        continue;

      if (room.TryGetDoorIn(-direction, out var door1) == false)
        continue;

      hasConnection = door.IsOpen && door1.IsOpen;
      break;
    }

    return hasConnection;
  }
}
