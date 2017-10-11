// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace RESTClient {
  /// <summary>
  /// Helper methods to check and get object properties
  /// </summary>
  public class Model {
    public static bool HasProperty(object obj, string propertyName) {
      return GetProperty(obj, propertyName) != null;
    }

    public static PropertyInfo GetProperty(object obj, string propertyName) {
#if NETFX_CORE
			return obj.GetType().GetTypeInfo().GetDeclaredProperty(propertyName); // GetProperty for UWP
#else
      return obj.GetType().GetProperty(propertyName);
#endif
    }

    public static bool HasField(object obj, string fieldName) {
      return GetField(obj, fieldName) != null;
    }

    public static FieldInfo GetField(object obj, string fieldName) {
#if NETFX_CORE
			return obj.GetType().GetTypeInfo().GetDeclaredField(fieldName); // GetField for UWP
#else
      return obj.GetType().GetField(fieldName);
#endif
    }
  }
}
