using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
  [Header("Room Parts")]
  [SerializeField] private GameObject _floor;
  [SerializeField] private List<GameObject> _doors;

  public Vector2Int DungeonPoint { get; private set; }
  public Vector3 WorldPoint => transform.position;

  private readonly HashSet<DungeonRoom> _neighbours = new(4);

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

  public void AddNeighbour(DungeonRoom target)
  {
    _neighbours.Add(target);
  }

  [CustomButton("Print Neighbours")]
  public void PrintNeighbours()
  {
    var neighbours = string.Empty;

    for (int i = 0; i < _neighbours.Count; i++)
      neighbours += $"{_neighbours.ElementAt(i).name}\n";

    Debug.LogError($"I'm {gameObject.name} has this neighbours: \n{neighbours}", transform);
  }
}
