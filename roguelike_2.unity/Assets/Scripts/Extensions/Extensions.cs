using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Web;
using Type = System.Type;
using UnityEngine.Assertions;

public static class Extensions
{
  public static bool IsEmpty(this Vector3 self)
  {
    return float.IsNegativeInfinity(self.x) &&
           float.IsNegativeInfinity(self.y) &&
           float.IsNegativeInfinity(self.z);
  }

  public static Vector2 FromXZ(this Vector3 self)
  {
    return new Vector2(self.x, self.z);
  }

  public static Vector3 FromXY(this Vector2 self)
  {
    return new Vector3(self.x, 0, self.y);
  }

  public static Vector3 ToXZ(this Vector3 self)
  {
    return new Vector3(self.x, 0, self.z);
  }

  public static Quaternion ToLook(this Vector3 self) => Quaternion.LookRotation(self);
  public static Quaternion ToLook(this Vector3 self, Vector3 up) => Quaternion.LookRotation(self, up);

  public static bool IsValid(this Vector3 v)
  {
    return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z);
  }

  public static bool BitIsSet(this uint self, int bit)
  {
    return (self & (uint)(1 << bit)) != 0;
  }

  public static uint BitSet(this uint self, int bit, bool flag)
  {
    if (flag)
    {
      self |= (uint)(1 << bit);
      return self;
    }
    else
    {
      self &= ~(uint)(1 << bit);
      return self;
    }
  }

  public static string Reverse(this string s)
  {
    char[] charArray = s.ToCharArray();
    Array.Reverse(charArray);
    return new string(charArray);
  }

  public static bool ExtractVersion(this string v, out int maj, out int min, out int pat)
  {
    maj = 0;
    min = 0;
    pat = 0;

    if (v == null || v.Length == 0)
      return false;

    string[] pieces = v.Split('.');

    try
    {
      if (pieces.Length == 2)
      {
        maj = int.Parse(pieces[0]);
        min = int.Parse(pieces[1]);
        return true;
      }
      else if (pieces.Length == 3)
      {
        maj = int.Parse(pieces[0]);
        min = int.Parse(pieces[1]);
        pat = int.Parse(pieces[2]);
        return true;
      }
      else
        return false;
    }
    catch (Exception)
    {
      return false;
    }
  }

  public static string RemoveQueryStringByKey(this string url, string key)
  {
    if (string.IsNullOrEmpty(url))
      return url;

    var uri = new Uri(url);

    // this gets all the query string key value pairs as a collection
    var newQueryString = HttpUtility.ParseQueryString(uri.Query);

    // this removes the key if exists
    newQueryString.Remove(key);

    return uri.ReplaceURIQueryString(newQueryString);
  }

  public static string ReplaceURIQueryString(this Uri uri, NameValueCollection newQueryString)
  {
    // this gets the page path from root without QueryString
    string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

    return newQueryString.Count > 0 ?
      $"{pagePathWithoutQueryString}?{newQueryString}" :
      pagePathWithoutQueryString;
  }

  public static GameObject GetChild(this GameObject o, string name)
  {
    Transform t = o.transform.Find(name);
    return t.gameObject;
  }

  public static T GetChild<T>(this GameObject o, string name) where T : Component
  {
    return o.GetChild(name).AsComponent<T>();
  }

  public static T GetChild<T>(this GameObject o) where T : Component
  {
    var res = o.GetComponentInChildren<T>();
    VerifyComponent(o, res);
    return res;
  }

  public static GameObject FindChild(this GameObject o, string name)
  {
    Transform t = o.transform.Find(name);
    if (t)
      return t.gameObject;
    return null;
  }

  public static GameObject FindChildRecursive(this GameObject o, string name)
  {
    Transform t = o.transform.FindRecursive(name);
    if (t == null)
      return null;
    return t.gameObject;
  }

  public static Transform GetChild(this Transform o, string name)
  {
    Transform t = o.transform.Find(name);
    return t;
  }

  public static void SetActiveAllChildren(this GameObject o, bool flag)
  {
    for (int i = 0; i < o.transform.childCount; ++i)
      o.transform.GetChild(i).gameObject.SetActive(flag);
  }

  public static string GetFullPath(this GameObject o)
  {
    return o.transform.GetFullPath();
  }

  public static void SetLayerRecursive(this GameObject obj, int layer, int except_layer = -1)
  {
    if (obj.layer == except_layer)
      return;

    obj.layer = layer;

    for (int i = 0; i < obj.transform.childCount; ++i)
      SetLayerRecursive(obj.transform.GetChild(i).gameObject, layer, except_layer);
  }

  //NOTE: using Unity's built-in non-allocating Find
  public static Transform FindRecursive(this Transform current, string name)
  {
    if (current.parent)
    {
      if (current.parent.Find(name) == current)
        return current;
    }
    //NOTE: switching to mem-allocating version only if there's no parent
    else if (current.name == name)
      return current;

    for (int i = 0; i < current.childCount; ++i)
    {
      var chld = current.GetChild(i);
      var tmp = chld.FindRecursive(name);
      if (tmp != null)
        return tmp;
    }
    return null;
  }

  public static GameObject GetParent(this GameObject o)
  {
    return o.transform.parent.gameObject;
  }

  public static string GetFullPath(this Transform t)
  {
    return t.GetPathWhile(current => current != null);
  }

  public delegate bool TransformPathBuilderCondition(Transform current);

  public static string GetPathWhile(this Transform t, TransformPathBuilderCondition condition)
  {
    string path = "";
    Transform tmp = t;
    while (condition(tmp))
    {
      path = tmp.gameObject.name + (path.Length > 0 ? ("/" + path) : "");
      tmp = tmp.parent;
    }
    return path;
  }

  public static bool BelongsTo(this GameObject o, GameObject other)
  {
    Transform current = o.transform;
    while (current != null)
    {
      if (current.gameObject == other)
        return true;
      current = current.parent;
    }
    return false;
  }

  public static void Shuffle<T>(this IList<T> list)
  {
    var count = list.Count;
    var last = count - 1;

    for (var i = 0; i < last; ++i)
    {
      var r = UnityEngine.Random.Range(i, count);
      (list[r], list[i]) = (list[i], list[r]);
    }
  }

  public static string EscapeSlash(this string src)
  {
    return src.Replace('/', '\u2215');
  }

  public struct ComponentsList
  {
    public bool busy;
    public object list;
  }

  //generic cache of component lists
  static List<ComponentsList> comp_lists = new List<ComponentsList>();

  public static List<T> RequestComponentsList<T>() where T : Component
  {
    //1. try to find non-busy existing one
    {
      for (int i = 0; i < comp_lists.Count; ++i)
      {
        var cl = comp_lists[i];
        if (cl.busy)
          continue;

        var lst = cl.list as List<T>;
        if (lst == null)
          continue;

        cl.busy = true;
        comp_lists[i] = cl;
        return lst;
      }
    }

    //2. otherwise create new one
    {
      var cl = new ComponentsList();
      var lst = new List<T>();
      cl.list = lst;
      cl.busy = true;
      comp_lists.Add(cl);
      return lst;
    }
  }

  public static void ReleaseComponentsList<T>(List<T> lst) where T : Component
  {
    for (int i = 0; i < comp_lists.Count; ++i)
    {
      var cl = comp_lists[i];
      if (cl.list == lst)
      {
        lst.Clear();
        cl.busy = false;
        comp_lists[i] = cl;
        return;
      }
    }
  }

  public static GameObject CloneTpl(this GameObject tpl)
  {
    GameObject o = GameObject.Instantiate(tpl) as GameObject;
    o.transform.SetParent(tpl.transform.parent);
    o.transform.localPosition = tpl.transform.localPosition;
    o.transform.localRotation = tpl.transform.localRotation;
    o.transform.localScale = tpl.transform.localScale;
    return o;
  }

  public static void GetComponentsRecursive<T>(this Component self, List<T> res) where T : Component
  {
    self.GetComponentsInChildren<T>(includeInactive: true, result: res);
  }

  public static T GetFirstComponent<T>(this Component self) where T : Component
  {
    T c = self.GetComponent<T>();
    if (c != null)
      return c;
    var tfm = self.transform;
    for (int i = 0; i < tfm.childCount; ++i)
    {
      var cc = tfm.GetChild(i).GetFirstComponent<T>();
      if (cc != null)
        return cc;
    }
    return c;
  }

  public static T AddComponentOnce<T>(this GameObject self) where T : Component
  {
    T c = self.GetComponent<T>();
    if (c == null)
      c = self.AddComponent<T>();
    return c;
  }

  public static T AsComponent<T>(this GameObject o) where T : Component
  {
    var res = o.GetComponent<T>();
    VerifyComponent(o, res);
    return res;
  }

  static void VerifyComponent<T>(GameObject o, T res) where T : Component
  {
    Assert.IsNull(res, $"Object {o.GetFullPath()} must have component {typeof(T).Name}\n");
  }

  public static void AddRendererSortingOrderRecursive(this Transform tfm, int add_order, string layer = null)
  {
    var r = tfm.GetComponent<Renderer>();
    if (r != null)
    {
      if (layer != null)
        r.sortingLayerName = layer;
      r.sortingOrder += add_order;
    }
    for (int i = 0; i < tfm.childCount; ++i)
      tfm.GetChild(i).AddRendererSortingOrderRecursive(add_order, layer);
  }

  public static void SetRendererSortingOrderRecursive(this Transform tfm, int order, string layer = null)
  {
    var r = tfm.GetComponent<Renderer>();
    if (r != null)
    {
      if (layer != null)
        r.sortingLayerName = layer;
      r.sortingOrder = order;
    }
    for (int i = 0; i < tfm.childCount; ++i)
      tfm.GetChild(i).SetRendererSortingOrderRecursive(order, layer);
  }

  public static void UpdateRendererSortingOrderRecursive(this Transform tfm, int base_order, string layer = null)
  {
    var r = tfm.GetComponent<Renderer>();
    if (r != null)
    {
      if (layer != null)
        r.sortingLayerName = layer;
      r.sortingOrder += base_order;
    }
    for (int i = 0; i < tfm.childCount; ++i)
      tfm.GetChild(i).UpdateRendererSortingOrderRecursive(base_order, layer);
  }

  static HashSet<object> mem_history = new HashSet<object>();

  public static int GetApproxMemSize(this object obj, int str_symb_size = 2, bool is_top = true)
  {
    if (is_top)
      mem_history.Clear();

    if (mem_history.Contains(obj))
      return 0;
    mem_history.Add(obj);

    int ptr_size = IntPtr.Size;
    if (obj == null)
      return ptr_size;

    int size = 0;
    Type type = obj.GetType();
    var info = type.GetFields(
    System.Reflection.BindingFlags.Instance |
    System.Reflection.BindingFlags.Public |
    System.Reflection.BindingFlags.NonPublic
    );
    foreach (var field in info)
    {
      if (field.FieldType.IsEnum)
        size += 4; //????
      else if (field.FieldType.IsValueType)
        size += System.Runtime.InteropServices.Marshal.SizeOf(field.FieldType);
      else
      {
        size += ptr_size;
        if (field.FieldType.IsArray)
        {
          var array = field.GetValue(obj) as Array;
          if (array != null)
          {
            var elementType = array.GetType().GetElementType();
            if (elementType.IsValueType)
              size += System.Runtime.InteropServices.Marshal.SizeOf(elementType) * array.Length;
            else
            {
              size += ptr_size * array.Length;
              if (elementType == typeof(string))
                size += str_symb_size * array.Length;
            }
          }
        }
        else if (field.FieldType == typeof(string) && field.GetValue(obj) != null)
          size += (field.GetValue(obj) as string).Length * str_symb_size;
        else
          size += field.GetValue(obj).GetApproxMemSize(str_symb_size, false);
      }
    }
    return size;
  }

  public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

  public static double ToStamp(this DateTime date)
  {
    return date.ToUniversalTime().Subtract(UnixEpoch).TotalSeconds;
  }

  public static DateTime ToDateTime(this double unix_stamp)
  {
    return UnixEpoch.AddSeconds(unix_stamp).ToLocalTime();
  }

  public static DateTime ToDateTimeUTC(this double unix_stamp)
  {
    return UnixEpoch.AddSeconds(unix_stamp).ToUniversalTime();
  }

  public static DateTime ToDateTime(this uint unix_stamp)
  {
    return ToDateTime((double)unix_stamp);
  }

  public static DateTime ToDateTimeUTC(this uint unix_stamp)
  {
    return ToDateTimeUTC((double)unix_stamp);
  }

  public static bool IsSameDay(this DateTime date_time, DateTime other)
  {
    return date_time.Year == other.Year && date_time.Month == other.Month && date_time.Day == other.Day;
  }

  //public static uint CalcHash(this ConfBase conf)
  //{
  //  //NOTE: we want to ignore id related changes
  //  uint backup_id = conf.id;
  //  string backup_strid = conf.strid;

  //  conf.id = 0;
  //  conf.strid = "";

  //  uint hash = MetaData.CalcHash(conf);

  //  conf.id = backup_id;
  //  conf.strid = backup_strid;

  //  return hash;
  //}

  public static DateTime Max(DateTime lhs, DateTime rhs)
  {
    return lhs > rhs ? lhs : rhs;
  }

  public static TimeSpan Max(TimeSpan lhs, TimeSpan rhs)
  {
    return lhs > rhs ? lhs : rhs;
  }

  public static bool IsValid(this DateTime date)
  {
    var reference = UnixEpoch;
    if (date.Kind == DateTimeKind.Local)
      reference = reference.ToLocalTime();
    return date.Ticks > reference.Ticks; //TODO?
  }

  public static bool TryCancel(this Animator anim)
  {
    //NOTE: if there's any layer in transition we wait for it
    //      to complete
    for (int i = 0; i < anim.layerCount; ++i)
    {
      if (anim.IsInTransition(i))
        return false;
    }

    anim.SetTrigger("Cancel");
    return true;
  }

  public static Color AsColor(this int n)
  {
    uint val = (uint)n;

    // scramble the bits up using Robert Jenkins' 32 bit integer hash function
    val = (val + 0x7ed55d16) + (val << 12);
    val = (val ^ 0xc761c23c) ^ (val >> 19);
    val = (val + 0x165667b1) + (val << 5);
    val = (val + 0xd3a2646c) ^ (val << 9);
    val = (val + 0xfd7046c5) + (val << 3);
    val = (val ^ 0xb55a4f09) ^ (val >> 16);

    float r = (float)((val >> 0) & 0xFF);
    float g = (float)((val >> 8) & 0xFF);
    float b = (float)((val >> 16) & 0xFF);

    float max = (float)Mathf.Max(Mathf.Max(r, g), b);
    float min = (float)Mathf.Min(Mathf.Min(r, g), b);
    float intensity = 0.75f;

    // Saturate and scale the color
    if (min == max)
    {
      return new Color(intensity, 0.0f, 0.0f, 1.0f);
    }
    else
    {
      float coef = (float)intensity / (max - min);
      return new Color(
      (r - min) * coef,
      (g - min) * coef,
      (b - min) * coef,
      1.0f
      );
    }
  }

  public static V GetOr<K, V>(this IDictionary<K, V> dict, K key, V default_val)
  {
    V res;
    if (!dict.TryGetValue(key, out res))
      res = default_val;
    return res;
  }

  //NOTE: T should be Enum. There is no 'official' Enum constraint in C#
  public static T ParseEnum<T>(this string enum_val) where T : struct, IConvertible
  {
    return (T)System.Enum.Parse(typeof(T), enum_val);
  }

  //NOTE: T should be Enum. There is no 'official' Enum constraint in C#
  public static T RotateEnum<T>(this T enum_val) where T : struct, IConvertible
  {
    int int_val = (int)(object)enum_val;
    int_val++;
    enum_val = (T)(object)int_val;
    if (!Enum.IsDefined(typeof(T), enum_val))
      enum_val = (T)(object)0;
    return enum_val;
  }


  //NOTE: this method contains boxing allocation, use with caution
  public static string ToEnumName<T>(this int raw_enum_val) where T : struct, IConvertible
  {
    return Enum.GetName(typeof(T), raw_enum_val);
  }

  //

  public static bool ToBool(this byte val)
  {
    return val > 0;
  }

  public static byte ToByte(this bool val)
  {
    return (byte)(val ? 1 : 0);
  }

  public static void SelectChild(this GameObject obj, string name)
  {
    foreach (Transform tfm in obj.transform)
    {
      GameObject child = tfm.gameObject;
      child.SetActive(child.name == name);
    }
  }

  public static void ChildSetActive(this GameObject obj, bool activate)
  {
    foreach (Transform tfm in obj.transform)
      tfm.gameObject.SetActive(activate);
  }

  public static string BaseName(this string stats_str)
  {
    return stats_str.Substring(stats_str.LastIndexOf("/") + 1); // name only without path to conf
  }

  public static string ReplaceFirst(this string text, string search, string replace)
  {
    int pos = text.IndexOf(search);
    if (pos < 0)
      return text;
    return $"{text.Substring(0, pos)}{replace}{text.Substring(pos + search.Length)}";
  }

  public static void RestoreLocalSettingsToDefault(this Transform transform)
  {
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;
    transform.localScale = Vector3.one;
  }

  public static void InvertVectors(ref Vector3 vector_a, ref Vector3 vector_b)
  {
    var temp_start_pos = vector_a;
    vector_a = vector_b;
    vector_b = temp_start_pos;
  }

  // public static T RandomValue<T>(this List<T> list) where T : class
  // {
  //   if(list.Count == 0)
  //     return null;
  //   return list[UnityEngine.Random.Range(0, list.Count)];
  // }
  //
  // public static uint RandomValue(this List<uint> list)
  // {
  //   if(list.Count == 0)
  //     return 0;
  //   return list[UnityEngine.Random.Range(0, list.Count)];
  // }

  public static int Max(this int[] array)
  {
    if (array.Length == 0)
      return 0;

    int max = array[0];
    for (int i = 0; i < array.Length; ++i)
      if (max < array[i])
        max = array[i];

    return max;
  }

  public static float GetMaxX(this List<Vector3> vectors_list)
  {
    var max_x = float.MinValue;
    foreach (var vector in vectors_list)
      if (vector.x > max_x)
        max_x = vector.x;

    return max_x;
  }

  public static float GetMaxY(this List<Vector3> vectors_list)
  {
    var max_y = float.MinValue;
    foreach (var vector in vectors_list)
      if (vector.y > max_y)
        max_y = vector.y;

    return max_y;
  }

  public static void SetAlpha(this Image self, float alpha)
  {
    var color = self.color;
    color.a = alpha;
    self.color = color;
  }

  public static string UnescapeName(this string str, int NAME_MAX_LENGTH)
  {
    string res = str.Unescape();

    int len = Math.Min(res.Length, NAME_MAX_LENGTH);
    res = res.Substring(0, len);
    return res;
  }

  public static string Unescape(this string str)
  {
    try
    {
      return System.Text.RegularExpressions.Regex.Unescape(str);
    }
    catch (Exception)
    {
      Debug.LogError("Could not unescape string '" + str + "'");
      return "???";
    }
  }

  public static bool IsStorageFullException(this Exception e)
  {
    const long ERROR_HANDLE_DISK_FULL = 0x27;
    const long ERROR_DISK_FULL = 0x70;
    long hr = Marshal.GetHRForException(e) & 0xFFFF;
    return hr == ERROR_HANDLE_DISK_FULL || hr == ERROR_DISK_FULL;
  }

  public static void SimulateStorageFullException()
  {
    //const int HR_ERROR_HANDLE_DISK_FULL = unchecked((int)0x80070027);
    const int HR_ERROR_DISK_FULL = unchecked((int)0x80070070);

    var ioe = new IOException("Pretending that storage is full!", HR_ERROR_DISK_FULL);
    throw ioe;
  }

  public static string Truncate(this string str, int max_length)
  {
    if (string.IsNullOrEmpty(str))
      return str;
    return str.Length <= max_length ? str : str.Substring(0, max_length);
  }

  public static Sprite GetSprite(this GameObject go)
  {
    if (go == null)
      return null;
    var sr = go.transform.GetFirstComponent<SpriteRenderer>();
    return sr == null ? null : sr.sprite;
  }

  public static long ParseStockAmount(this string amount_str)
  {
    //NOTE: a hopefully bulletproof parsing that will floor non-integer amounts sent by 3rd-parties
    return (long)double.Parse(amount_str, System.Globalization.CultureInfo.InvariantCulture);
  }

  public static void MaskSet(this ref long mask, int bit, bool val)
  {
    if (val)
      mask |= (long)(1 << bit);
    else
      mask &= ~(1 << bit);
  }

  public static bool MaskIsSet(this long mask, int bit)
  {
    return (mask & 1 << bit) != 0;
  }

  public static Color SetAlpha(this Color color, float value)
  {
    color.a = value;
    return color;
  }

  public static Color WithAlpha(this Color color, float value)
  {
    Color newColor = color;
    newColor.a = value;
    return newColor;
  }
}
