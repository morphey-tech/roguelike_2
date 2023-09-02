using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
  private readonly List<DungeonRoom> _rooms = new(20);

  public void Init(IEnumerable<DungeonRoom> rooms)
  {
    _rooms.AddRange(rooms);
  }
}
