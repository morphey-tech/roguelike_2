using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Zenject;

public class DungeonCreator
{
  private readonly List<GameObject> _roomsPrefabs;

  private readonly AssetsProviderService _assetsProvider;
  private readonly RandomnessService _randomnessService;
  private readonly Configs _config;

  [Inject]
  public DungeonCreator(AssetsProviderService assetsProvider, RandomnessService randomService, Configs configs)
  {
    _assetsProvider = assetsProvider;
    _randomnessService = randomService;
    _config = configs;

    _roomsPrefabs = new(5);
  }

  public async Task Create(string id)
  {
    await Load();

    var config = _config.GetConf<ConfDungeon>(id);
    var dungeonGo = new GameObject($"Dungeon: {config.ID}");

    var roomsCount = _randomnessService.RandomInt(0, _roomsPrefabs.Count);
    var firstRoom = _assetsProvider.Create(_roomsPrefabs[0], dungeonGo.transform);
    firstRoom.transform.localPosition = Vector3.zero;
  }

  private async Task Load()
  {
    var roomPrefab = await _assetsProvider.LoadAsync<GameObject>("stone_room");
    _roomsPrefabs.Add(roomPrefab);
  }
}