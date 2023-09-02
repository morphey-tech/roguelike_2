using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Zenject;
using System.Linq;

public class DungeonCreator
{
  private readonly AssetsProviderService _assetsProvider;
  private readonly RandomnessService _randomnessService;
  private readonly Configs _configs;
  
  private readonly List<DungeonRoom> _roomPrefabsCache;
  private readonly HashSet<Vector2Int> _roomPlaces;
  private DungeonRoom[,] _spawnedRooms;

  [Inject]
  public DungeonCreator(AssetsProviderService assetsProvider, RandomnessService randomService, Configs configs)
  {
    _assetsProvider = assetsProvider;
    _randomnessService = randomService;
    _configs = configs;

    _roomPrefabsCache = new(10);
    _roomPlaces = new(10);
  }

  public async Task Create(string id)
  {
    var config = _configs.GetConf<ConfDungeon>(id);
    await LoadRoomsBy(config);
    
    var dungeonGo = new GameObject($"Dungeon: {config.ID}");
    var firstRoom = _assetsProvider.Create(GetRandomRoom(), dungeonGo.transform);
    firstRoom.transform.localPosition = Vector3.zero;

    var roomsCount = _randomnessService.RandomInt(config.MinRoomsCount, config.MaxRoomsCount);
    _spawnedRooms = new DungeonRoom[roomsCount, roomsCount];
    _spawnedRooms[roomsCount, roomsCount] = firstRoom;

    for (int i = 0; i < roomsCount; i++)
      CreateRandomRoom(dungeonGo.transform);
  }

  private async Task LoadRoomsBy(ConfDungeon config)
  {
    _roomPrefabsCache.Clear();

    foreach (var roomAlias in config.Rooms) 
    {
      var roomPrefab = await _assetsProvider.LoadAsync<GameObject>(roomAlias);
      var roomComponent = roomPrefab.GetComponent<DungeonRoom>();
      _roomPrefabsCache.Add(roomComponent);
    }
  }

  private DungeonRoom GetRandomRoom()
  {
    var randomIndex = _randomnessService.RandomInt(0, _roomPrefabsCache.Count);
    return _roomPrefabsCache[randomIndex];
  }

  private void CreateRandomRoom(Transform parent)
  {
    _roomPlaces.Clear();

    for (int y = 0; y < _spawnedRooms.GetLength(1); y++)
    {
      for (int x = 0; x < _spawnedRooms.GetLength(0); x++)
      {
        if (_spawnedRooms[x, y] == null)
          continue;

        var maxY = _spawnedRooms.GetLength(1) - 1;
        var maxX = _spawnedRooms.GetLength(0) - 1;

        var leftNeighbourPosition = new Vector2Int(x - 1, y);
        var rightNeighbourPosition = new Vector2Int(x + 1, y);
        var bottomNeighbourPosition = new Vector2Int(x, y - 1);
        var topNeighbourPosition = new Vector2Int(x, y + 1);

        if (x > 0 && _spawnedRooms[leftNeighbourPosition.x, leftNeighbourPosition.y] == null)
          _roomPlaces.Add(leftNeighbourPosition);

        if (x < maxX && _spawnedRooms[rightNeighbourPosition.x, rightNeighbourPosition.y] == null)
          _roomPlaces.Add(rightNeighbourPosition);

        if (y > 0 && _spawnedRooms[bottomNeighbourPosition.x, bottomNeighbourPosition.y] == null)
          _roomPlaces.Add(bottomNeighbourPosition);

        if (y < maxY && _spawnedRooms[topNeighbourPosition.x, topNeighbourPosition.y] == null)
          _roomPlaces.Add(topNeighbourPosition);
      }
    }

    var randomRoom = GetRandomRoom();
    var room = _assetsProvider.Create(randomRoom);
    room.transform.SetParent(parent);

    var randomPositionIndex = _randomnessService.RandomInt(0, _roomPlaces.Count);
    var randomPosition = _roomPlaces.ElementAt(randomPositionIndex);
    var targetPosition = new Vector3(randomPosition.x - room.Size.x, 0f, randomPosition.y - room.Size.y);
    room.transform.localPosition = targetPosition;

    _spawnedRooms[randomPosition.x, randomPosition.y] = room;
  }
}