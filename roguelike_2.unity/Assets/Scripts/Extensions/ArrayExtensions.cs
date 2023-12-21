using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ArrayExtensions
{
  public static T First<T>(this IList<T> list)
  {
    if (list.IsEmpty())
      throw new ArgumentOutOfRangeException();

    return list[0];
  }

  public static bool Contains<T>([JetBrains.Annotations.NotNull] this IList<T> list, Func<T, bool> predicate)
  {
    if (list == null)
      throw new ArgumentNullException(nameof(list));

    for (int i = list.Count - 1; i >= 0; i--)
      if (predicate.Invoke(list[i]))
        return true;

    return false;
  }

  public static int GetNextLoopedIndex<T>(this IList<T> list, int current)
  {
    return list.GetLoopedIndex(current + 1);
  }

  public static int GetLoopedIndex<T>(this IList<T> list, int index)
  {
    return (int) Mathf.Repeat(index, list.Count);
  }

  public static bool HasIndex<T>(this IList<T> list, int index)
  {
    return index >= 0 && list.Count > index;
  }

  public static bool HasOnlyNulls<T>(this IList<T> list)
  {
    for (int i = list.Count - 1; i >= 0; i--)
      if (list[i] != null)
        return false;

    return true;
  }

  public static bool IsEmpty<T>(this IList<T> list)
  {
    return list.Count == 0;
  }

  public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
  {
    foreach (T element in list)
      action(element);
  }

  public static void ForEach<T>(this IList<T> list, Func<T> action)
  {
    for (int i = 0; i < list.Count(); i++)
    {
      list[i] = action();
    }
  }

  public static T Last<T>(this IList<T> list)
  {
    if (list.IsEmpty())
      throw new ArgumentOutOfRangeException();

    return list[list.Count - 1];
  }

  public static T RandomValue<T>(this IList<T> list)
  {
    if (list.IsEmpty())
      throw new ArgumentOutOfRangeException();

    return list[Random.Range(0, list.Count)];
  }


  public static IList<T> Swap<T>(this IList<T> list, int a, int b)
  {
    T tmp = list[a];
    list[a] = list[b];
    list[b] = tmp;
    return list;
  }

  public static void Shuffle<T>(this IList<T> list, int seed = -1)
  {
    var old = Random.state;

    if (seed >= 0)
      Random.InitState(seed);

    int n = list.Count;

    while (n > 1)
    {
      --n;
      int k = Random.Range(0, n);
      T value = list[k];
      list[k] = list[n];
      list[n] = value;
    }

    Random.state = old;
  }

  public static void AddRange<T>(this IList<T> self, IList<T> list)
  {
    for (int i = 0; i < list.Count; ++i)
      self.Add(list[i]);
  }

  public static void Assign<T>(ref List<T> dst, IEnumerable<T> src)
  {
    if (src == null)
      return;

    if (dst == null)
    {
      dst = new List<T>(src);
    }
    else
    {
      dst.Clear();
      dst.AddRange(src);
    }
  }


  public static bool AddUnique<T>(this IList<T> dst, T o)
  {
    if (dst.Contains(o))
      return false;

    dst.Add(o);
    return true;
  }

  public static void AddUnique<T>(this IList<T> dst, IList<T> src)
  {
    for (int i = 0; i < src.Count; ++i)
      dst.AddUnique(src[i]);
  }

  public static bool ContainsAnyOf<T>(this IList<T> list, IList<T> other)
  {
    for (int i = 0; i < other.Count; ++i)
    {
      if (list.Contains(other[i]))
        return true;
    }

    return false;
  }

  public static T GetOr<T>(this IList<T> list, int idx, T defaultValue)
  {
    return idx >= 0 && idx < list.Count ? list[idx] : defaultValue;
  }
  
  public static string GetString(this IList list)
  {
    var strings = new string[list.Count];
    for (var i = 0; i < list.Count; i++)
      strings[i] = list[i].ToString();

    return $"[{string.Join(",", strings)}]";
  }
}
