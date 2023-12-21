using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using VContainer;

namespace Services
{
  public sealed class AssetsProviderService
  {
    [Inject]
    public AssetsProviderService() { }

    public async Task<T> LoadAsync<T>(string alias) where T : Object
    {
      return await Addressables.LoadAssetAsync<T>(alias).Task;
    }

    public async Task<T> CreateAsync<T>(string alias, Transform? parent = null) where T : Object
    {
      var loaded = await Addressables.InstantiateAsync(alias, parent).Task;

      return loaded.TryGetComponent(out T instance)
        ? instance
        : throw new System.FormatException($"Can't get type {typeof(T)} from instance. Please check addressables.");
    }

    public T Load<T>(string alias) where T : Object
    {
      var handle = Addressables.LoadAsset<T>(alias);
      handle.WaitForCompletion();
      return handle.Result;
    }

    public T Create<T>(string alias, Transform parent = null) where T : Object
    {
      var handle = Addressables.Instantiate(alias, parent).Task;

      return handle.Result.TryGetComponent(out T instance)
        ? instance
        : throw new System.FormatException($"Can't get type {typeof(T)} from instance. Please check addressables.");
    }

    public void Release<TObject>(TObject asset)
    {
      Addressables.Release(asset);
    }

    public void ReleaseInstance(GameObject instance)
    {
      Addressables.ReleaseInstance(instance);
    }
  }
}