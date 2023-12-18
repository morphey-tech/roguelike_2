using Morphey.Editor.Attributes;
using UnityEngine;
using Zenject;

namespace Boostrap
{
  public sealed class Bootstrap : MonoBehaviour
  {
    private Dungeon _dungeon;
    private IDungeonCreator _creator;
    private Configs _configs;

    [Inject]
    private void Construct(IDungeonCreator dungeonCreator, Configs config)
    {
      _creator = dungeonCreator;
      _configs = config;
    }

    private void Awake()
    {
      CreateDungeon();
    }

    [DebugButton("Create Dungeon")]
    public async void CreateDungeon()
    {
      var dungeonConfig = _configs.GetConf<ConfDungeon>("conf_dungeon_1");
      _dungeon = await _creator.Create(dungeonConfig);
    }

    [DebugButton("Destroy Dungeon")]
    public void DestroyDungeon()
    {
      GameObject.Destroy(_dungeon.gameObject); //TODO: move to another entity with control flow for create/destroy dungeons and use Addresables.Release viw service
    }
  }
}