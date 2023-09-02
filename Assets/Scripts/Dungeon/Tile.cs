using System;
using UnityEngine;

[Serializable]
public struct Tile
{
  [NonSerialized] public GameObject Content;
  public Vector2Int Position;
}
