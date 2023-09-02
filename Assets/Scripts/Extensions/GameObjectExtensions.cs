using UnityEngine;

public static class GameObjectExtensions
{
  public static T GetOrCreate<T>(this GameObject go) where T : Component
  {
    var comp = go.GetComponent<T>();

    if (comp != null)
      return comp;

    return go.AddComponent<T>();
  }

  public static void SetParent(this GameObject go, Transform parent, bool worldPosStays = false)
  {
    go.transform.SetParent(parent, worldPosStays);
  }

  public static bool CompareLayers(this GameObject obj, LayerMask layer)
  {
    return ((1 << obj.layer) & layer) != 0;
  }

  public static void Show(this GameObject obj)
  {
    obj.SetActive(true);
  }

  public static void Hide(this GameObject obj)
  {
    obj.SetActive(false);
  }
}
