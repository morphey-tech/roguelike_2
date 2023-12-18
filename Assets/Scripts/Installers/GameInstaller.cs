using Services;
using Zenject;

public sealed class GameInstaller : MonoInstaller
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

    Container
      .Bind<RandomnessService>()
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
      .Bind<IDungeonCreator>()
      .To<DungeonCreator>()
      .AsSingle();
  }
}