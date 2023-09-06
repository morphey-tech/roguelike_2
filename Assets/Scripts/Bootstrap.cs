using UnityEngine;
using Zenject;

public class Bootstrap : MonoBehaviour
{
  private Dungeon _dungeon;
  private DungeonCreator _creator;
  private Configs _configs;

  [Inject]
  private void Construct(DungeonCreator dungeonCreator, Configs config)
  {
    _creator = dungeonCreator;
    _configs = config;
  }

  private void Awake()
  {
    CreateDungeon();
  }

  [CustomButton("Create Dungeon")]
  public async void CreateDungeon()
  {
    var dungeonConfig = _configs.GetConf<ConfDungeon>("conf_dungeon_1"); 
    _dungeon = await _creator.Create(dungeonConfig);
  }

  [CustomButton("Destroy Dungeon")]
  public void DestroyDungeon()
  {
    GameObject.Destroy(_dungeon.gameObject);
  }
}
