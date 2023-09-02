using Zenject;

namespace DI.Installers
{
  public class GameInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      BindServices();
      BindConfigs();
      BindDungeonCreator();
    }

    private void BindServices()
    {
      Container
        .Bind<AssetsProviderService>()
        .FromNew()
        .AsSingle();
    }

    private void BindConfigs()
    {
      Container
        .Bind<Configs>()
        .FromNew()
        .AsSingle()
        .NonLazy();
    }

    private void BindDungeonCreator()
    {
      Container
        .Bind<DungeonCreator>()
        .FromNew()
        .AsSingle();
    }
  }
}
