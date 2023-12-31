using System;
using System.Collections.Generic;

namespace Morphey.Services
{
  public class ConfigsProviderService
  {
    private readonly List<ConfBase> _configs;

    public T GetConf<T>(string id) where T : ConfBase
    {
      foreach ( var item in _configs )
      {
        if ( item.ID != id )
          continue;

        return item as T;
      }

      throw new ArgumentException($"Can't find config with ID {id}");
    }

    public ConfigsProviderService()
    {
      _configs = new()
    {
      new ConfDungeon()
      {
        ID = "conf_dungeon_1",
        Type = EnumDungeonType.STONE,
        MinRoomsCount = 10,
        MaxRoomsCount = 20,
        Rooms = new[]
        {
          "stone_room_1"
        }
      }
    };
    }
  }
}