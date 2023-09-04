using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;
using Zenject;
using System.Drawing;
using System;

public sealed class DungeonCreator
{
  private readonly AssetsProviderService _assetsProvider;
  private readonly RandomnessService _randomnessService;
  private readonly Configs _configs;

  private readonly List<DungeonRoom> _roomPrefabsCache;
  private readonly HashSet<Vector2Int> _possibleRoomPositions;
  private readonly HashSet<DungeonRoom> _createdRoomsCache;
  private DungeonRoom[,] _dungeonGrid;

  [Inject]
  public DungeonCreator(AssetsProviderService assetsProvider, RandomnessService randomService, Configs configs)
  {
    _assetsProvider = assetsProvider;
    _randomnessService = randomService;
    _configs = configs;

    _roomPrefabsCache = new(10);
    _possibleRoomPositions = new(10);
    _createdRoomsCache = new(20);
  }

  //1. Create - DONE
  //2. Find neighbourds - DONE
  //3. Make holes - WIP
  //4. Fill interactable - ???
  public async UniTask<Dungeon> Create(string id)
  {
    var config = _configs.GetConf<ConfDungeon>(id);
    var dungeonGo = new GameObject($"Dungeon: {config.ID}");
    var dungeonComp = dungeonGo.AddComponent<Dungeon>();

    await LoadRoomsViewBy(config);
    await CreateRooms(config, dungeonGo);
    FindRoomsNeighbours();
    
    //3
    MakeRoomsConnections();
    //4
    await FillRoomsInteractiveObject();

    return dungeonComp.Init(_createdRoomsCache);
  }

  private async UniTask LoadRoomsViewBy(ConfDungeon config)
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

    var randomAliasOfRoom = GetRandomRoomAlias(config);
    var firstRoom = await _assetsProvider.CreateAsync<DungeonRoom>(randomAliasOfRoom, parent.transform);
    firstRoom.transform.localPosition = Vector3.zero;

    var roomsCount = _randomnessService.RandomInt(config.MinRoomsCount, config.MaxRoomsCount);
    _dungeonGrid = new DungeonRoom[roomsCount, roomsCount];
    _dungeonGrid[0, 0] = firstRoom;

    _createdRoomsCache.Clear();
    _createdRoomsCache.Add(firstRoom);

    for (int i = 0; i < roomsCount; i++)
    {
      var room = await CreateRandomRoom(config, parent);
      _createdRoomsCache.Add(room);
    }
  }

  private string GetRandomRoomAlias(ConfDungeon config)
  {
    var randomIndex = _randomnessService.RandomInt(0, config.Rooms.Length - 1);
    return config.Rooms[randomIndex];
  }

  private async UniTask<DungeonRoom> CreateRandomRoom(ConfDungeon config, GameObject parent)
  {
    var randomRoom = GetRandomRoomAlias(config);
    var room = await _assetsProvider.CreateAsync<DungeonRoom>(randomRoom);
    room.transform.SetParent(parent);

    var randomPosition = GetFreeRandomPosition();
    var roomSize = room.CalculateSize();
    var targetX = randomPosition.x * roomSize.x;
    var targetY = randomPosition.y * roomSize.y;
    var targetPosition = new Vector3(targetX, 0f, targetY);
    room.transform.localPosition = targetPosition;
    room.SetDungeonPosition(randomPosition);

    _dungeonGrid[randomPosition.x, randomPosition.y] = room;

    return room;
  }

  private Vector2Int GetFreeRandomPosition()
  {
    _possibleRoomPositions.Clear();

    for (int y = 0; y < _dungeonGrid.GetLength(1); y++)
    {
      for (int x = 0; x < _dungeonGrid.GetLength(0); x++)
      {
        if (_dungeonGrid[x, y] == null)
          continue;

        var maxX = _dungeonGrid.GetLength(0) - 1;
        var maxY = _dungeonGrid.GetLength(1) - 1;

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
    return _dungeonGrid[target.x, target.y] == null;
  }

  private void FindRoomsNeighbours()
  {
    for (int i = 0; i < _createdRoomsCache.Count; i++)
      FindRoomNeighbours(_createdRoomsCache.ElementAt(i));
  }

  private void FindRoomNeighbours(DungeonRoom room)
  {
    var dungeonPoint = room.DungeonPoint;

    for (int x = -1; x <= 1; x++)
    {
      for (int y = -1; y <= 1; y++)
      {
        if (x == 0 && y == 0)
          continue;

        if (Math.Abs(x) + Math.Abs(y) > 1)
          continue;

        var neighbourX = dungeonPoint.x + x;
        var neighbourY = dungeonPoint.y + y;

        if (DungeonContainsPoint(neighbourX, neighbourY)) 
          room.AddNeighbour(_dungeonGrid[neighbourX, neighbourY]);
      }
    }
  }

  private bool DungeonContainsPoint(int x, int y)
  {
    return x >= 0 && y >= 0 && x < _dungeonGrid.GetLength(0) - 1 && y < _dungeonGrid.GetLength(1) - 1;
  }

  private void MakeRoomsConnections()
  {
  }

  private async UniTask FillRoomsInteractiveObject()
  {
    await UniTask.Yield();
  }
}