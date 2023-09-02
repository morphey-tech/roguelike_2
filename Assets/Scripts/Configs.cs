using System;
using System.Collections.Generic;

public class Configs
{
  private readonly List<ConfBase> _configs;

  public T GetConf<T>(string id) where T : ConfBase
  {
    foreach (var item in _configs)
    {
      if (item.ID != id)
        continue;

      return item as T;
    }

    throw new ArgumentException($"Can't find config with ID {id}");
  }

  public Configs()
  {
    _configs = new()
    {
      new ConfDungeon()
      {
        ID = "conf_location_1",
        Type = EnumDungeonType.DEFAULT,
        MaxRoomsCount = 10
      }
    };
  }
}