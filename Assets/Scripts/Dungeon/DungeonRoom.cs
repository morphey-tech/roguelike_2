using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
  [Header("Room Parts")]
  [SerializeField] private GameObject _floor;
  [SerializeField] private List<GameObject> _doors;

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
      floorSize = new Vector3(collider.bounds.size.x, collider.bounds.size.z);
    else
      throw new InvalidOperationException("Floor must have collider for calculated size.");
  
    return floorSize;
  }
}
