using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Zenject;

public class DungeonCreator
{
  private readonly List<GameObject> _roomPrefabsCache;

  private readonly AssetsProviderService _assetsProvider;
  private readonly RandomnessService _randomnessService;
  private readonly Configs _config;

  [Inject]
  public DungeonCreator(AssetsProviderService assetsProvider, RandomnessService randomService, Configs configs)
  {
    _assetsProvider = assetsProvider;
    _randomnessService = randomService;
    _config = configs;

    _roomPrefabsCache = new(10);
  }

  public async Task Create(string id)
  {
    var config = _config.GetConf<ConfDungeon>(id);
    await LoadRoomsBy(config);
    
    var dungeonGo = new GameObject($"Dungeon: {config.ID}");
    var roomsCount = _randomnessService.RandomInt(0, _roomPrefabsCache.Count);
    var firstRoom = _assetsProvider.Create(_roomPrefabsCache[0], dungeonGo.transform);
    firstRoom.transform.localPosition = Vector3.zero;
  }

  private async Task LoadRoomsBy(ConfDungeon config)
  {
    _roomPrefabsCache.Clear();

    foreach (var roomAlias in config.Rooms) 
    {
      var roomPrefab = await _assetsProvider.LoadAsync<GameObject>(roomAlias);
      _roomPrefabsCache.Add(roomPrefab);
    }
  }
}