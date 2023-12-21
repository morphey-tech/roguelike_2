using Morphey.Services;
using Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Morphey.DI
{
  public class ApplicationScope : LifetimeScope
  {
    protected override void Configure(IContainerBuilder builder)
    {
      BindServices(builder);
      BindTemp(builder);
    }

    private void BindServices(IContainerBuilder builder)
    {
      var holder = new GameObject("Infrastructure");
      holder.transform.SetParent(transform, false);

      //THINK: Maybe REGISTER INSTANCE? (fire in the booth)
      builder.Register<ConfigsProviderService>(Lifetime.Singleton); 
      builder.Register<AssetsProviderService>(Lifetime.Singleton);
      builder.Register<RandomnessService>(Lifetime.Singleton);
    }

    private void BindTemp(IContainerBuilder builder)
    {
      builder.Register<DungeonCreator>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
    }
  }
}