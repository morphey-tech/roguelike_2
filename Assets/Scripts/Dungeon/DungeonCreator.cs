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
  private readonly List<DungeonRoom> _createdRoomsCache;
  private readonly HashSet<Vector2Int> _possibleRoomPositions;
  private DungeonRoom[,] _busyRoomsCoordinates;

  [Inject]
  public DungeonCreator(AssetsProviderService assetsProvider, RandomnessService randomService, Configs configs)
  {
    _assetsProvider = assetsProvider;
    _randomnessService = randomService;
    _configs = configs;

    _roomPrefabsCache = new(10);
    _possibleRoomPositions = new(10);
    _createdRoomsCache = new(10);
  }

  public async Task<Dungeon> Create(string id)
  {
    _createdRoomsCache.Clear();

    var config = _configs.GetConf<ConfDungeon>(id);
    await LoadRoomsBy(config);

    var dungeonGo = new GameObject($"Dungeon: {config.ID}");
    var dungeonComp = dungeonGo.AddComponent<Dungeon>();
    var firstRoom = _assetsProvider.Create(GetRandomRoom(), parent: dungeonGo.transform);
    firstRoom.transform.localPosition = Vector3.zero;

    var roomsCount = _randomnessService.RandomInt(config.MinRoomsCount, config.MaxRoomsCount);
    _busyRoomsCoordinates = new DungeonRoom[roomsCount, roomsCount];
    _busyRoomsCoordinates[0, 0] = firstRoom;
    _createdRoomsCache.Add(firstRoom);

    for (int i = 0; i < roomsCount; i++)
      _createdRoomsCache.Add(CreateRandomRoom(dungeonGo));

    dungeonComp.Init(_createdRoomsCache);

    return dungeonComp;
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
    var randomIndex = _randomnessService.RandomInt(0, _roomPrefabsCache.Count - 1);
    return _roomPrefabsCache[randomIndex];
  }

  private DungeonRoom CreateRandomRoom(GameObject parent)
  {
    var randomRoom = GetRandomRoom();
    var room = _assetsProvider.Create(randomRoom);
    room.transform.SetParent(parent);

    var randomPosition = GetFreeRandomPosition();
    var targetPosition = new Vector3(randomPosition.x * room.Size.x, 0f, randomPosition.y * room.Size.y);
    room.transform.localPosition = targetPosition;
    _busyRoomsCoordinates[randomPosition.x, randomPosition.y] = room;

    return room;
  }

  private Vector2Int GetFreeRandomPosition()
  {
    _possibleRoomPositions.Clear();

    for (int y = 0; y < _busyRoomsCoordinates.GetLength(1); y++)
    {
      for (int x = 0; x < _busyRoomsCoordinates.GetLength(0); x++)
      {
        if (_busyRoomsCoordinates[x, y] == null)
          continue;

        var maxX = _busyRoomsCoordinates.GetLength(0) - 1;
        var maxY = _busyRoomsCoordinates.GetLength(1) - 1;

        var leftNeighbourPosition = new Vector2Int(x - 1, y);
        var rightNeighbourPosition = new Vector2Int(x + 1, y);
        var bottomNeighbourPosition = new Vector2Int(x, y - 1);
        var topNeighbourPosition = new Vector2Int(x, y + 1);

        if (x > 0 && RoomCoordinateIsFree(leftNeighbourPosition))
          _possibleRoomPositions.Add(leftNeighbourPosition);

        if (x < maxX && RoomCoordinateIsFree(rightNeighbourPosition))
          _possibleRoomPositions.Add(rightNeighbourPosition);

        if (y > 0 && RoomCoordinateIsFree(bottomNeighbourPosition))
          _possibleRoomPositions.Add(bottomNeighbourPosition);

        if (y < maxY && RoomCoordinateIsFree(topNeighbourPosition))
          _possibleRoomPositions.Add(topNeighbourPosition);
      }
    }

    var randomPositionIndex = _randomnessService.RandomInt(0, _possibleRoomPositions.Count - 1);
    return _possibleRoomPositions.ElementAt(randomPositionIndex);
  }

  private bool RoomCoordinateIsFree(Vector2Int target)
  {
    return _busyRoomsCoordinates[target.x, target.y] == null;
  }
}