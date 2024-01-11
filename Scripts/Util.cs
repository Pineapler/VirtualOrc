using System;
using System.Reflection;
using UnityEngine;

namespace VirtualOrc.Scripts;

public class Util {
    public static T CopyComponent<T>(T original, GameObject destination) where T : Component {
        // from https://discussions.unity.com/t/copy-a-component-at-runtime/71172/3 
        Type type = typeof(T);
        T dstComponent = destination.GetComponent<T>() ?? destination.AddComponent<T>();
        
        foreach (FieldInfo field in typeof(T).GetFields()) {
            if (field.IsStatic) continue;
            field.SetValue(dstComponent, field.GetValue(original));
        }

        foreach (PropertyInfo prop in typeof(T).GetProperties()) {
            if (!prop.CanWrite || !prop.CanRead || prop.Name == "name" || prop.IsDefined(typeof(ObsoleteAttribute), true)) continue;
            prop.SetValue(dstComponent, prop.GetValue(original, null), null);
        }

        return dstComponent;
    }
}