using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;

public class AssetsProviderService
{
  public async Task<T> LoadAsync<T>(string alias) where T: Object
  {
    return await Addressables.LoadAssetAsync<T>(alias).Task;
  }

  public async Task<T> CreateAsync<T>(string alias) where T: Object
  {
    var loaded = await Addressables.InstantiateAsync(alias).Task;

    return loaded.TryGetComponent<T>(out var instance) 
      ? instance 
      : throw new System.FormatException($"Can't get type {typeof(T)} from instance. Please check addressables.");
  }

  public T Load<T>(string alias) where T: Object
  {
    var handle = Addressables.LoadAsset<T>(alias);
    handle.WaitForCompletion();
    return handle.Result;
  }

  public T Create<T>(string alias) where T: Object
  {
    var handle = Addressables.Instantiate(alias).Task;

    return handle.Result.TryGetComponent<T>(out var instance)
      ? instance
      : throw new System.FormatException($"Can't get type {typeof(T)} from instance. Please check addressables.");
  }

  public T Create<T>(T prefab, Transform parent = null, Vector3 position = default) where T: Object
  {
    return Object.Instantiate(prefab, position, Quaternion.identity, parent);
  }
}
