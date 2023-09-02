using System;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
  [SerializeField] private GameObject _floor;

  public Vector2 Size => _size;

  private Vector2 _size;

  private void Awake()
  {
    _size = CalculateSize();
  }

  private Vector2 CalculateSize()
  {
    var floorSize = Vector3.zero;

    if (_floor.TryGetComponent<Collider>(out var collider))
      floorSize = collider.bounds.size;
    else
      throw new InvalidOperationException("Floor must have collider for calculated size.");
  
    return floorSize;
  }
}
