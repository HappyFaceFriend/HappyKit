using UnityEngine;
using System;
using static HappyKit.NestedClassInspectorMirror;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HappyKit
{
#if UNITY_EDITOR
    public partial class CustomComponentEditor : Editor
    {
        void DeserializePropertyEntry(SerializedProperty property, SerializedPropertyEntry entry)
        {
            switch (entry.FieldType)
            {
                case SerializedPropertyEntry.Type.Object:
                    DeserializeObjectEntry(property, entry); break;
                case SerializedPropertyEntry.Type.CustomClass:
                    DeserializeClassEntry(property, entry); break;
                case SerializedPropertyEntry.Type.Array:
                    DeserializeArrayEntry(property, entry); break;
                case SerializedPropertyEntry.Type.BasicType:
                    DeserializeBasicTypeEntry(property, entry); break;
                default:
                    throw new ArgumentException("Unknown FieldType to deserialize: " + entry.FieldType);
            }
        }

        SerializedPropertyEntry SerializePropertyAsEntry(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            {
                    return SerializeObjectAsEntry(property);
            }
            else if (property.propertyType == SerializedPropertyType.Generic)
            {
                if (property.isArray)
                    return SerializeArrayAsEntry(property);
                else
                    return StoreClassAsEntry(property);
            }
            else
                return SerializeBasicTypeAsEntry(property);
        }
        SerializedPropertyEntry SerializeObjectAsEntry(SerializedProperty property)
        {
            var entry = new SerializedPropertyEntry
            {
                FieldName = property.name,
                FieldType = SerializedPropertyEntry.Type.Object,
                ObjectReference = property.objectReferenceValue
            };
            return entry;
        }
        void DeserializeObjectEntry(SerializedProperty property, SerializedPropertyEntry entry)
        {
            property.objectReferenceValue = entry.ObjectReference;
        }
        SerializedPropertyEntry StoreClassAsEntry(SerializedProperty property)
        {
            SerializedProperty classProperty = property.Copy();
            var entry = new SerializedPropertyEntry
            {
                FieldName = property.name,
                FieldType = SerializedPropertyEntry.Type.CustomClass,
                NestedEntries = new()
            };
            // Iterate over all fields in the custom class
            int startDepth = classProperty.depth;
            bool children = true;
            while (classProperty.NextVisible(children) && classProperty.depth > startDepth)
            {
                children = false;
                entry.NestedEntries.Add(SerializePropertyAsEntry(classProperty));
            }
            return entry;
        }
        void DeserializeClassEntry(SerializedProperty property, SerializedPropertyEntry entry)
        {
            SerializedProperty classProperty = property.Copy();
            if (entry.NestedEntries == null || entry.NestedEntries.Count == 0)
                return;
            int startDepth = classProperty.depth;
            bool children = true;
            while (classProperty.NextVisible(children) && classProperty.depth > startDepth)
            {
                children = false;
                for (int i = 0; i < entry.NestedEntries.Count; i++)
                {
                    if (entry.NestedEntries[i].FieldName == classProperty.name)
                    {
                        DeserializePropertyEntry(classProperty, entry.NestedEntries[i]);
                        break;
                    }
                }
            }
        }
        SerializedPropertyEntry SerializeArrayAsEntry(SerializedProperty property)
        {
            var entry = new SerializedPropertyEntry
            {
                FieldName = property.name,
                FieldType = SerializedPropertyEntry.Type.Array,
                NestedEntries = new()
            };
            for (int i = 0; i < property.arraySize; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                entry.NestedEntries.Add(SerializePropertyAsEntry(element));
            }
            return entry;
        }
        void DeserializeArrayEntry(SerializedProperty property, SerializedPropertyEntry entry)
        {
            property.ClearArray(); // Clear the current array

            if (entry.NestedEntries == null || entry.NestedEntries.Count == 0)
                return;
            property.arraySize = entry.NestedEntries.Count;

            for (int i = 0; i < entry.NestedEntries.Count; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                DeserializePropertyEntry(element, entry.NestedEntries[i]);
            }
        }
        SerializedPropertyEntry SerializeBasicTypeAsEntry(SerializedProperty property)
        {
            var entry = new SerializedPropertyEntry
            {
                FieldName = property.name,
                FieldType = SerializedPropertyEntry.Type.BasicType,
            };
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    entry.StringValue = property.intValue.ToString(); break;
                case SerializedPropertyType.Enum:
                    entry.StringValue = property.enumNames[property.enumValueIndex].ToString(); break;
                case SerializedPropertyType.Boolean:
                    entry.StringValue = property.boolValue.ToString(); break;
                case SerializedPropertyType.Float:
                    entry.StringValue = property.floatValue.ToString(); break;
                case SerializedPropertyType.String:
                    entry.StringValue = property.stringValue; break;
                case SerializedPropertyType.Color:
                    entry.StringValue = JsonUtility.ToJson(property.colorValue); break;
                case SerializedPropertyType.Vector2:
                    entry.StringValue = JsonUtility.ToJson(property.vector2Value); break;
                case SerializedPropertyType.Vector3:
                    entry.StringValue = JsonUtility.ToJson(property.vector3Value); break;
                case SerializedPropertyType.Vector4:
                    entry.StringValue = JsonUtility.ToJson(property.vector4Value); break;
            }
            return entry;
        }
        void DeserializeBasicTypeEntry(SerializedProperty property, SerializedPropertyEntry entry)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = int.Parse(entry.StringValue); break;
                case SerializedPropertyType.Enum:
                    for(int i=0; i<property.enumNames.Length; i++)
                    {
                        if (property.enumNames[i] == entry.StringValue)
                        {
                            property.enumValueIndex = i;
                            break;
                        }
                    }
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = bool.Parse(entry.StringValue); break;
                case SerializedPropertyType.Float:
                    property.floatValue = float.Parse(entry.StringValue); break;
                case SerializedPropertyType.String:
                    property.stringValue = entry.StringValue; break;
                case SerializedPropertyType.Color:
                    property.colorValue = JsonUtility.FromJson<Color>(entry.StringValue); break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = JsonUtility.FromJson<Vector2>(entry.StringValue); break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = JsonUtility.FromJson<Vector3>(entry.StringValue); break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = JsonUtility.FromJson<Vector4>(entry.StringValue); break;
            }
        }
    }
#endif
}
