using Cysharp.Threading.Tasks;

public interface IDungeonCreator
{
  UniTask<Dungeon> Create(ConfDungeon config);
}
