using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class Bootstrap : MonoBehaviour
{
  private DungeonCreator _creator;

  [Inject]
  private void Construct(DungeonCreator dungeonCreator)
  {
    _creator = dungeonCreator;
  }

  private void Awake()
  {
    CreateDungeon();
  }

  public async void CreateDungeon()
  {
    await _creator.Create("conf_dungeon_1");
  }
}
