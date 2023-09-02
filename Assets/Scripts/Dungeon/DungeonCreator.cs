using Random = System.Random;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Threading.Tasks;

public class DungeonCreator
{
  private readonly Random _random = new();
  private readonly List<GameObject> _roomsPrefabs;
  private readonly AssetsProviderService _assetsProvider;
  private readonly ConfDungeon _config;

  [Inject]
  public DungeonCreator(AssetsProviderService assetsProvider, Configs configs)
  {
    _assetsProvider = assetsProvider;
    _config = configs.GetConf<ConfDungeon>("conf_location_1");
    _roomsPrefabs = new(5);
  }

  public async Task Create()
  {
    await Load();

    var dungeonGo = new GameObject($"Dungeon: {_config.ID}");

    var roomsCount = _random.Next(0, _roomsPrefabs.Count);
    var firstRoom = _assetsProvider.Create(_roomsPrefabs[0], dungeonGo.transform);
    firstRoom.transform.localPosition = Vector3.zero;
  }

  private async Task Load()
  {
    var roomPrefab = await _assetsProvider.LoadAsync<GameObject>("stone_room");
    _roomsPrefabs.Add(roomPrefab);
  }
}