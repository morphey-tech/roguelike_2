using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TransformExtensions
{
  public static GameObject GetChildOrCreate(this Transform owner, string name, GameObject prefab)
  {
    GameObject finded = null;

    for (int i = 0; i < owner.childCount; i++)
    {
      GameObject go = owner.GetChild(i).gameObject;

      if (go.name != name)
        continue;

      finded = go;
      break;
    }

    if (finded == null)
    {
      finded = GameObject.Instantiate(prefab, owner);
      finded.name = name;
    }

    return finded;
  }

  public static void DestroyChilds(this Transform target)
  {
    if (target.childCount == 0)
      return;

    for (int i = 0; i < target.childCount; i++)
    {
      GameObject go = target.GetChild(i--).gameObject;
      GameObject.DestroyImmediate(go);
    }
  }

  public static Transform GetChildRecursive(this Transform target, string name)
  {
    if (target.childCount == 0)
      return null;

    int childCount = target.childCount;
    Queue<Transform> findQueue = new Queue<Transform>(childCount);
    findQueue.Enqueue(target);

    while (findQueue.Count > 0)
    {
      Transform transform = findQueue.Dequeue();

      if (transform.name == name)
        return transform;

      foreach (Transform child in transform)
        findQueue.Enqueue(child);
    }

    return null;
  }

  public static void SetPositionX(this Transform target, float value) =>
    target.position = new Vector3(value, target.position.y, target.position.z);

  public static void SetPositionY(this Transform target, float value) =>
    target.position = new Vector3(target.position.x, value, target.position.z);

  public static void SetPositionZ(this Transform target, float value) =>
    target.position = new Vector3(target.position.x, target.position.y, value);

  public static void SetLocalPositionX(this Transform target, float value) =>
    target.localPosition = new Vector3(value, target.localPosition.y, target.localPosition.z);

  public static void SetLocalPositionY(this Transform target, float value) =>
    target.localPosition = new Vector3(target.localPosition.x, value, target.localPosition.z);

  public static void SetLocalPositionZ(this Transform target, float value) =>
    target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, value);

  public static void SetScaleX(this Transform target, float value) =>
    target.localScale = new Vector3(value, target.localScale.y, target.localScale.z);

  public static void SetScaleY(this Transform target, float value) =>
    target.localScale = new Vector3(target.localScale.x, value, target.localScale.z);

  public static void SetScaleZ(this Transform target, float value) =>
    target.localScale = new Vector3(target.localScale.x, target.localScale.y, value);

  public static void ClearName(this Transform target)
  {
    target.name = Regex.Replace(target.name, "\\((.*)\\)", string.Empty, RegexOptions.IgnoreCase);

    StringBuilder builder = new StringBuilder(target.name);
    builder.Replace("(", string.Empty);
    builder.Replace(")", string.Empty);
    builder.Replace("Clone", string.Empty);
    builder.Replace(" ", string.Empty);

    target.name = builder.ToString();
  }

  public static void SetChildLayers(this Transform t, int layer)
  {
    for (int i = 0; i < t.childCount; i++)
    {
      Transform child = t.GetChild(i);
      child.gameObject.layer = layer;
      SetChildLayers(child, layer);
    }
  }

  public static bool CompareLayers(this Transform t, LayerMask layer)
  {
    return ((1 << t.gameObject.layer) & layer) != 0;
  }

  public static void SetParent(this Transform t, GameObject parent)
  {
    t.SetParent(parent.transform);
  }
}
