using UnityEngine;
using Zenject;

public class Bootstrap : MonoBehaviour
{
  private Dungeon _dungeon;
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
    _dungeon = await _creator.Create("conf_dungeon_1");
  }

  public void DestroyDungeon()
  {
    GameObject.Destroy(_dungeon.gameObject);
  }
}
