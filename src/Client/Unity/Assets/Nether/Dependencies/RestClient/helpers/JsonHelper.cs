#pragma warning disable 0649 // ignores warning: array "is never assigned to, and will always have its default value 'null'"
using UnityEngine;
using System;
using System.Text.RegularExpressions;
#if NETFX_CORE
using Windows.Data.Json;
using System.Collections.Generic;
using System.Linq;
#endif

namespace RESTClient {
  /// <summary>
  /// Wrapper work-around for json array described on https://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/
  /// </summary>
  [Serializable]
  internal class Wrapper<T> {
    public T[] array;
  }

  public class JsonHelper {
    /// <summary>
    /// Work-around to parse json array
    /// </summary>
    public static T[] FromJsonArray<T>(string json) {
      // Work-around for JsonUtility array serialization issues in Windows Store Apps.
#if NETFX_CORE
            JsonArray jsonArray = new JsonArray();
            if (JsonArray.TryParse(json, out jsonArray))
            {
                return GetArray<T>(jsonArray);
            }
            Debug.LogWarning("Failed to parse json array of type:" + typeof(T).ToString() );
            return default(T[]);
#endif
      string newJson = "{\"array\":" + json + "}";
      Wrapper<T> wrapper = new Wrapper<T>();
      try {
        wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
      } catch (Exception e) {
        Debug.LogWarning("Failed to parse json array of type:" + typeof(T).ToString() + " Exception message: " + e.Message);
        return default(T[]);
      }
      return wrapper.array;
    }

#if NETFX_CORE
    private static T[] GetArray<T>(JsonArray array)
    {
        List<T> list = new List<T>();
        foreach (var x in array)
        {
            try
            {
                T item = JsonUtility.FromJson<T>(x.ToString());
                list.Add(item);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to parse json of type:" + typeof(T).ToString() + " Exception message: " + e.Message + " json:'" + x.ToString() + "'");
            }
        }
        return list.ToArray();
    }
#endif

    /// <summary>
    /// Workaround to only exclude Data Model's read only system properties being returned as json object. Unfortunately there is no JsonUtil attribute to do this as [NonSerialized] will just ignore the properties completely (both in and out).
    /// </summary>
    public static string ToJsonExcludingSystemProperties(object obj) {
      string jsonString = JsonUtility.ToJson(obj);
      return Regex.Replace(jsonString, "(?i)(\\\"id\\\":\\\"\\\",)?(\\\"createdAt\\\":\\\"[0-9TZ:.-]*\\\",)?(\\\"updatedAt\\\":\\\"[0-9TZ:.-]*\\\",)?(\\\"version\\\":\\\"[A-Z0-9=]*\\\",)?(\\\"deleted\\\":(true|false),)?(\\\"ROW_NUMBER\\\":\\\"[0-9]*\\\",)?", "");
    }
  }
}
