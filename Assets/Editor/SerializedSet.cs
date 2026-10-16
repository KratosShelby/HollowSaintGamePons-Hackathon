using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HollowSaint.Core;

namespace HollowSaint.EditorTools
{
    /// <summary>Sets private [SerializeField] values on components from editor code.</summary>
    public static class SerializedSet
    {
        public static void Set(Object target, string property, object value)
        {
            var so = new SerializedObject(target);
            var p = so.FindProperty(property);
            if (p == null)
            {
                Debug.LogError($"[SerializedSet] {target.GetType().Name} has no serialized field '{property}'.");
                return;
            }
            Assign(p, value);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void Assign(SerializedProperty p, object value)
        {
            switch (value)
            {
                case null: p.objectReferenceValue = null; break;
                case string s: p.stringValue = s; break;
                case bool b: p.boolValue = b; break;
                case int i: p.intValue = i; break;
                case float f: p.floatValue = f; break;
                case Color c: p.colorValue = c; break;
                case Vector2 v: p.vector2Value = v; break;
                case Object o: p.objectReferenceValue = o; break;
                case IList<DialogueLine> lines:
                    p.arraySize = lines.Count;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        var e = p.GetArrayElementAtIndex(i);
                        e.FindPropertyRelative("speaker").stringValue = lines[i].speaker;
                        e.FindPropertyRelative("text").stringValue = lines[i].text;
                    }
                    break;
                case IList<GameObject> objects:
                    p.arraySize = objects.Count;
                    for (int i = 0; i < objects.Count; i++)
                        p.GetArrayElementAtIndex(i).objectReferenceValue = objects[i];
                    break;
                default:
                    Debug.LogError($"[SerializedSet] Unsupported value type {value.GetType().Name} for '{p.name}'.");
                    break;
            }
        }
    }
}
