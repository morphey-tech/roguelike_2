using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;
using Zenject;

public sealed class DungeonCreator
{
  private readonly AssetsProviderService _assetsProvider;
  private readonly RandomnessService _randomnessService;

  private DungeonRoom[,] _dungeonGrid;
  private readonly List<DungeonRoom> _roomPrefabsCache;
  private readonly HashSet<Vector2Int> _possibleRoomPositions;
  private readonly HashSet<DungeonRoom> _createdRoomsCache;

  private readonly Vector2Int[] _neighborsPattern =
  {
    new Vector2Int(0, 1),
    new Vector2Int(-1, 0),
    new Vector2Int(1, 0),
    new Vector2Int(0, -1)
  };

  [Inject]
  public DungeonCreator(AssetsProviderService assetsProvider, RandomnessService randomService)
  {
    _assetsProvider = assetsProvider;
    _randomnessService = randomService;

    _roomPrefabsCache = new(10);
    _possibleRoomPositions = new(10);
    _createdRoomsCache = new(20);
  }

  public async UniTask<Dungeon> Create(ConfDungeon config)
  {
    var dungeonGo = new GameObject($"Dungeon: {config.ID}");
    var dungeonComp = dungeonGo.AddComponent<Dungeon>();

    await LoadRoomsPrefabsBy(config);
    await CreateRooms(config, dungeonGo);
    FindNeighborsForAllRooms();
    MakeRoomsConnections();

    //In feature
    //await FillRoomsInteractiveObject();
    return dungeonComp.Init(_createdRoomsCache);
  }

  private async UniTask LoadRoomsPrefabsBy(ConfDungeon config)
  {
    _roomPrefabsCache.Clear();

    foreach (var roomAlias in config.Rooms)
    {
      var roomPrefab = await _assetsProvider.LoadAsync<GameObject>(roomAlias);
      var roomComponent = roomPrefab.GetComponent<DungeonRoom>();
      _roomPrefabsCache.Add(roomComponent);
    }
  }

  private async UniTask CreateRooms(ConfDungeon config, GameObject parent)
  {
    var totalRoomsCount = _randomnessService.RandomInt(config.MinRoomsCount, config.MaxRoomsCount);
    _dungeonGrid = new DungeonRoom[totalRoomsCount, totalRoomsCount];

    var randomAliasOfRoom = GetRandomRoomAlias(config);
    var firstRoom = await _assetsProvider.CreateAsync<DungeonRoom>(randomAliasOfRoom);
    firstRoom.transform.SetParent(parent);
    firstRoom.transform.localPosition = Vector3.zero;
    firstRoom.Init(Vector2Int.zero); //TODO: from default zero vector to random dungeon 
    _dungeonGrid[0, 0] = firstRoom; //TODO: random fisrt room pos

    _createdRoomsCache.Clear();
    _createdRoomsCache.Add(firstRoom);

    for (int i = 1; i < totalRoomsCount; i++)
    {
      var createdRoom = await CreateRandomRoom(config, parent);
      _createdRoomsCache.Add(createdRoom);
    }
  }

  private string GetRandomRoomAlias(ConfDungeon config)
  {
    var randomIndex = _randomnessService.RandomInt(0, config.Rooms.Length - 1);
    return config.Rooms[randomIndex];
  }

  private async UniTask<DungeonRoom> CreateRandomRoom(ConfDungeon config, GameObject parent)
  {
    var randomAliasOfRoom = GetRandomRoomAlias(config);
    var createdRoom = await _assetsProvider.CreateAsync<DungeonRoom>(randomAliasOfRoom);
    createdRoom.transform.SetParent(parent);

    var dungeonPoint = FindEmptyRandomPosition();
    var worldX = dungeonPoint.x * createdRoom.Size.x;
    var worldY = dungeonPoint.y * createdRoom.Size.y;
    var worldPosition = new Vector3(worldX, 0f, worldY);
    createdRoom.transform.localPosition = worldPosition;
    createdRoom.Init(dungeonPoint);

    _dungeonGrid[dungeonPoint.x, dungeonPoint.y] = createdRoom;

    return createdRoom;
  }

  private Vector2Int FindEmptyRandomPosition()
  {
    DungeonRoom randomRoom;
    Vector2Int dungeonPoint;

    do
    {
      var randomIndex = _randomnessService.RandomInt(0, _createdRoomsCache.Count - 1);
      randomRoom = _createdRoomsCache.ElementAt(randomIndex);
    }
    while (TryGetEmptyRandomPositionAround(randomRoom, out dungeonPoint) == false);

    return dungeonPoint;
  }


  private bool TryGetEmptyRandomPositionAround(DungeonRoom room, out Vector2Int emptyPosition)
  {
    _possibleRoomPositions.Clear();
    emptyPosition = default;

    foreach (var pattern in _neighborsPattern)
    {
      var xPos = room.DungeonPoint.x + pattern.x;
      var yPos = room.DungeonPoint.y + pattern.y;

      if (DungeonContainsPoint(xPos, yPos) == false)
        continue;

      if (RoomMissingAt(xPos, yPos) == false)
        continue;

      var possiblePosition = new Vector2Int(xPos, yPos);
      _possibleRoomPositions.Add(possiblePosition);
    }

    if (_possibleRoomPositions.Count <= 0)
      return false;

    var randomPositionIndex = _randomnessService.RandomInt(0, _possibleRoomPositions.Count - 1);
    emptyPosition = _possibleRoomPositions.ElementAt(randomPositionIndex);

    return true;
  }

  private bool RoomMissingAt(int x, int y)
  {
    return _dungeonGrid[x, y] == null;
  }

  private void FindNeighborsForAllRooms()
  {
    for (int i = 0; i < _createdRoomsCache.Count; i++)
      FindNeighborsFor(_createdRoomsCache.ElementAt(i));
  }

  private void FindNeighborsFor(DungeonRoom room)
  {
    foreach (var pattern in _neighborsPattern)
    {
      var xPos = room.DungeonPoint.x + pattern.x;
      var yPos = room.DungeonPoint.y + pattern.y;

      if (DungeonContainsPoint(xPos, yPos) == false)
        continue;

      if (RoomMissingAt(xPos, yPos))
        continue;

      room.AddNeighbor(_dungeonGrid[xPos, yPos]);
    }
  }

  private bool DungeonContainsPoint(int x, int y)
  {
    return x >= 0 && y >= 0 && x < _dungeonGrid.GetLength(0) && y < _dungeonGrid.GetLength(1);
  }

  private void MakeRoomsConnections()
  {
    foreach (var room in _createdRoomsCache)
    {
      var minimumConnectionsCount = room.Neighbors.Count <= 2 ? 1 : 2;
      var randomConnectionsCount = _randomnessService.RandomInt(minimumConnectionsCount, room.Neighbors.Count);
      var connectionsExist = room.GetNeighboursCountWithConnection();

      if (connectionsExist >= randomConnectionsCount)
        continue;

      var requiredConnections = randomConnectionsCount - connectionsExist;

      for (int i = 0; i < requiredConnections; i++)
      {
        DungeonRoom closedNeighbour = default;

        do
        {
          var randomNeighborIndex = _randomnessService.RandomInt(0, room.Neighbors.Count - 1);
          var randomNeighbor = room.Neighbors.ElementAt(randomNeighborIndex);

          if (room.HasConnectionWith(randomNeighbor))
            continue;

          closedNeighbour = randomNeighbor;
        }
        while (closedNeighbour == null);

        room.MakeConnectionWith(closedNeighbour);
      }
    }
  }

  private async UniTask FillRoomsInteractiveObject()
  {
    await UniTask.Yield();
  }
}