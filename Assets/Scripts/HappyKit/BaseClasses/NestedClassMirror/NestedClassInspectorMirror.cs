using System.Linq;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HappyKit
{
    using static HappyKit.NestedClassInspectorMirror;
    // Change this base class to a component that has nested classes
    using BaseClass = MonoBehaviour;

    [RequireComponent(typeof(BaseClass))]
    public class NestedClassInspectorMirror : MonoBehaviour
    {
        [Serializable]
        public class SerializedPropertyEntry
        {
            public enum Type { BasicType, Object, Array, CustomClass}
            public string FieldName = "None";
            public string StringValue = "";
            public UnityEngine.Object ObjectReference = null;
            public Type FieldType = Type.BasicType;
            public List<SerializedPropertyEntry> NestedEntries = null;
        }

        [SerializeField] string _serializedClassName;
        [SerializeField] List<SerializedPropertyEntry> _serializedEntries = new List<SerializedPropertyEntry>();

        public List<SerializedPropertyEntry> SerializedEntries => _serializedEntries;

        public T ReplaceUsingSerializedData<T>() where T : MonoBehaviour
        {
            var newComponent = gameObject.AddComponent<T>();
            Deserialize(typeof(T), newComponent);
            Destroy(this);
            return newComponent;
        }
        public Component ReplaceUsingSerializedData()
        {
            if (string.IsNullOrEmpty(_serializedClassName))
            {
                Debug.LogError("This nested class mirror doesn't have a class set.");
                Destroy(this);
                return null;
            }

            Type baseType = GetComponent<BaseClass>().GetType();
            Type nestedType = baseType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                                           .FirstOrDefault(t => t.Name == _serializedClassName);

            if (nestedType != null)
            {
                var newComponent = gameObject.AddComponent(nestedType);
                Deserialize(nestedType, newComponent);
                Destroy(this);
                return newComponent;
            }
            else
            {
                Debug.LogError("Can't find nested class named "+ _serializedClassName + ".");
                Destroy(this);
                return null;
            }
        }
        void Deserialize(System.Type type, Component targetComponent)
        {
            foreach(var entry in _serializedEntries)
            {
                FieldInfo fieldInfo = type.GetField(entry.FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fieldInfo == null)
                    throw new Exception("No field named " +entry.FieldName + " at " + type.Name);
                DeserializeEntry(targetComponent, entry);
            }
        }
        private void DeserializeEntry(object targetComponent, SerializedPropertyEntry entry)
        {
            switch (entry.FieldType)
            {
                case SerializedPropertyEntry.Type.BasicType:
                    DeserializeBasicType(targetComponent, entry);
                    break;
                case SerializedPropertyEntry.Type.Object:
                    DeserializeObject(targetComponent, entry);
                    break;
                case SerializedPropertyEntry.Type.Array:
                    DeserializeArray(targetComponent, entry);
                    break;
                case SerializedPropertyEntry.Type.CustomClass:
                    DeserializeClass(targetComponent, entry);
                    break;
                default:
                    throw new ArgumentException("Unknown FieldType to deserialize: " + entry.FieldType);
            }
        }
        private void DeserializeBasicType(object target, SerializedPropertyEntry entry)
        {
            FieldInfo fieldInfo = target.GetType().GetField(entry.FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Type fieldType = fieldInfo.FieldType;
            object parsedValue = null;

            if (fieldType == typeof(int))
                parsedValue = int.Parse(entry.StringValue);
            else if (fieldType == typeof(float))
                parsedValue = float.Parse(entry.StringValue);
            else if (fieldType == typeof(bool))
                parsedValue = bool.Parse(entry.StringValue);
            else if (fieldType == typeof(string))
                parsedValue = entry.StringValue;
            else if (fieldType == typeof(Color))
                parsedValue = JsonUtility.FromJson<Color>(entry.StringValue);
            else if (fieldType == typeof(Vector2))
                parsedValue = JsonUtility.FromJson<Vector2>(entry.StringValue);
            else if (fieldType == typeof(Vector3))
                parsedValue = JsonUtility.FromJson<Vector3>(entry.StringValue);
            else if (fieldType == typeof(Vector4))
                parsedValue = JsonUtility.FromJson<Vector4>(entry.StringValue);
            else if (fieldType.IsEnum)  // Add this for handling enums
                parsedValue = Enum.Parse(fieldType, entry.StringValue);
            if (parsedValue != null)
                fieldInfo.SetValue(target, parsedValue);
        }

        private void DeserializeObject(object target, SerializedPropertyEntry entry)
        {
            FieldInfo fieldInfo = target.GetType().GetField(entry.FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null && entry.ObjectReference != null)
            {
                fieldInfo.SetValue(target, entry.ObjectReference);
            }
        }
        private void DeserializeArray(object target, SerializedPropertyEntry entry)
        {
            FieldInfo fieldInfo = target.GetType().GetField(entry.FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null || entry.NestedEntries == null || entry.NestedEntries.Count == 0)
                return;  // Exit if the field is null or there are no elements to deserialize

            Type fieldType = fieldInfo.FieldType;

            Type elementType = null;

            // Check if it's an array
            if (fieldType.IsArray)
            {
                elementType = fieldType.GetElementType();
            }
            // Check if it's a List<T>
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                elementType = fieldType.GetGenericArguments()[0];  // Get the type of T in List<T>
            }

            if (elementType == null)
            {
                Debug.LogError("Field is neither an array nor a List<T>, cannot deserialize.");
                return;
            }

            // Handle array deserialization
            if (fieldType.IsArray)
            {
                Array arrayInstance = Array.CreateInstance(elementType, entry.NestedEntries.Count);

                for (int i = 0; i < entry.NestedEntries.Count; i++)
                {
                    object elementInstance = null;

                    if (elementType.IsValueType || elementType == typeof(string))
                    {
                        elementInstance = Activator.CreateInstance(elementType);
                        DeserializeEntry(elementInstance, entry.NestedEntries[i]);
                    }
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(elementType))
                    {
                        elementInstance = entry.NestedEntries[i].ObjectReference;
                    }
                    else
                    {
                        elementInstance = Activator.CreateInstance(elementType);
                        DeserializeEntry(elementInstance, entry.NestedEntries[i]);
                    }

                    arrayInstance.SetValue(elementInstance, i);
                }

                fieldInfo.SetValue(target, arrayInstance);
            }
            // Handle List<T> deserialization
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listInstance = (IList)Activator.CreateInstance(fieldType);

                for (int i = 0; i < entry.NestedEntries.Count; i++)
                {
                    object elementInstance = null;

                    if (entry.NestedEntries[i].FieldType == SerializedPropertyEntry.Type.BasicType)
                    {
                        elementInstance = Activator.CreateInstance(elementType);
                        DeserializeBasicType(elementInstance, entry.NestedEntries[i]);
                    }
                    else if (entry.NestedEntries[i].FieldType == SerializedPropertyEntry.Type.Object)
                    {
                        elementInstance = entry.NestedEntries[i].ObjectReference;
                    }
                    else
                    {
                        elementInstance = Activator.CreateInstance(elementType);
                        // Recursively deserialize nested entries
                        foreach (var nestedEntry in entry.NestedEntries[i].NestedEntries)
                        {
                            DeserializeEntry(elementInstance, nestedEntry);
                        }
                    }
                    listInstance.Add(elementInstance);  // Add the element to the list
                }
                fieldInfo.SetValue(target, listInstance);  // Set the populated List<T> back to the field
            }
        }
        private void DeserializeClass(object target, SerializedPropertyEntry entry)
        {
            FieldInfo fieldInfo = target.GetType().GetField(entry.FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (fieldInfo != null)
            {
                object classInstance = fieldInfo.GetValue(target);
                if (entry.NestedEntries == null || entry.NestedEntries.Count == 0)
                    return;
                // If the class instance is null, create a new instance
                if (classInstance == null)
                {
                    classInstance = Activator.CreateInstance(fieldInfo.FieldType);
                    fieldInfo.SetValue(target, classInstance);
                }

                // Recursively deserialize nested entries
                foreach (var nestedEntry in entry.NestedEntries)
                {
                    DeserializeEntry(classInstance, nestedEntry);
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NestedClassInspectorMirror))]
    public partial class CustomComponentEditor : Editor
    {
        Type[] _innerClasses;
        string[] _innerClassNames;
        int _selectedIndex = 0;

        SerializedObject _tempComponent;
        GameObject _tempObject;

        SerializedProperty _classNameProperty;
        SerializedProperty _serializedEntriesProperty;

        NestedClassInspectorMirror _originalPrefabComponent = null;

        private void OnEnable()
        {
            // Get properties
            _classNameProperty = serializedObject.FindProperty("_serializedClassName");
            _serializedEntriesProperty = serializedObject.FindProperty("_serializedEntries");

            Debug.Assert(_classNameProperty != null);
            Debug.Assert(_classNameProperty.propertyType == SerializedPropertyType.String);
            Debug.Assert(_serializedEntriesProperty != null);

            NestedClassInspectorMirror mirror = (NestedClassInspectorMirror)target;

            // Get nested class
            Type baseType = mirror.GetComponent<BaseClass>().GetType();
            _innerClasses = baseType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(MonoBehaviour)))
                                        .ToArray();
            _innerClassNames = _innerClasses.Select(innerClass => innerClass.Name).ToArray();

            Debug.Assert(_innerClasses.Length > 0, "No nested class found from part");

            if (!string.IsNullOrEmpty(_classNameProperty.stringValue))
            {
                _selectedIndex = Array.IndexOf(_innerClassNames, _classNameProperty.stringValue);
            }
            else
            {
                _selectedIndex = -1;
            }
            if (_selectedIndex == -1)
            {
                _selectedIndex = 0;
                ResetMirrorComponent();
            }

            CreateAndDeserializeToTempComponent(_innerClasses[_selectedIndex]);

            _originalPrefabComponent = PrefabUtility.GetCorrespondingObjectFromOriginalSource(mirror);
            if (_originalPrefabComponent == mirror)
                _originalPrefabComponent = null;
        }

        void CreateAndDeserializeToTempComponent(System.Type type)
        {
            if (_tempObject != null)
                DestroyImmediate(_tempObject);

            // Create temp object and add component
            _tempObject = new GameObject(_innerClassNames[_selectedIndex] + " Temp Instance");
            _tempObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;

            NestedClassInspectorMirror mirror = (NestedClassInspectorMirror)target;

            var tempComponent = (MonoBehaviour)_tempObject.AddComponent(type);
            SerializedObject tempSerializedObject = new SerializedObject(tempComponent);
            SerializedProperty property = tempSerializedObject.GetIterator();
            bool children = true;
            int startDepth = property.depth;
            while (property.NextVisible(children) && property.depth > startDepth)
            {
                children = false;
                for (int i = 0; i < mirror.SerializedEntries.Count; i++)
                {
                    if (mirror.SerializedEntries[i].FieldName == property.name)
                    {
                        DeserializePropertyEntry(property, mirror.SerializedEntries[i]);
                        break;
                    }
                }
            }
            tempSerializedObject.ApplyModifiedProperties();
            _tempComponent = tempSerializedObject;
        }

        public override void OnInspectorGUI()
        {
            if (targets.Length > 1)
            {
                EditorGUILayout.HelpBox("Multiple selections are not allowed for this component.", MessageType.Warning);
                return;
            }

            NestedClassInspectorMirror mirror = (NestedClassInspectorMirror)target;

            if (_innerClasses.Length == 0)
            {
                EditorGUILayout.HelpBox("No inner classes found within " + typeof(BaseClass).Name + ".", MessageType.Error);
                return;
            }

            // Inner class selection
            if (_originalPrefabComponent != null)
                GUI.enabled = false;
            EditorGUI.BeginChangeCheck();
            _selectedIndex = EditorGUILayout.Popup("Select Nested Class", _selectedIndex, _innerClassNames);

            if (EditorGUI.EndChangeCheck())
            {
                _classNameProperty.stringValue = _innerClassNames[_selectedIndex];
                serializedObject.ApplyModifiedProperties();
                CreateAndDeserializeToTempComponent(_innerClasses[_selectedIndex]);
            }
            if (_originalPrefabComponent != null)
                GUI.enabled = true;

            if (_tempComponent == null || _tempComponent.targetObject == null)
            {
                EditorGUILayout.HelpBox("Failed to initialize serialized target.", MessageType.Error);
                return;
            }

            // Nested class inspector
            _tempComponent.Update();

            SerializedProperty iterator = _tempComponent.GetIterator();
            bool children = true;
            int startDepth = iterator.depth;
            while (iterator.NextVisible(children) && iterator.depth > startDepth)
            {
                children = false;
                if (iterator.name == "m_Script")
                    continue;
                // Check if is different from original prefab
                bool isDifferent = false;
                if (_originalPrefabComponent != null)
                {
                    for(int i=0; i<_serializedEntriesProperty.arraySize; i++)
                    {
                        var fieldNameProperty = _serializedEntriesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("FieldName");
                        if (iterator.name == fieldNameProperty.stringValue)
                        {
                            isDifferent = _serializedEntriesProperty.GetArrayElementAtIndex(i).prefabOverride;
                            break;
                        }
                    }
                }
                if (isDifferent)
                {
                    Rect rect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(iterator));
                    Rect highlightRect = rect;
                    highlightRect.xMin -= 14;
                    highlightRect.xMax = rect.xMin - 1;
                    highlightRect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.DrawRect(highlightRect, Color.blue);

                    EditorStyles.label.fontStyle = FontStyle.Bold;
                    EditorGUI.PropertyField(rect, iterator, true);
                    EditorStyles.label.fontStyle = FontStyle.Normal;
                }
                else
                    EditorGUILayout.PropertyField(iterator, true);
            }

            _tempComponent.ApplyModifiedProperties();

            // Serialize
            if (GUI.changed)
            {
                SaveObjectToFieldEntries(_tempComponent);
            }
        }

        void SaveObjectToFieldEntries(SerializedObject objectToSerialize)
        {
            var mirrorComponent = (target as NestedClassInspectorMirror);
            SerializedProperty property = objectToSerialize.GetIterator();
            mirrorComponent.SerializedEntries.Clear();
            bool children = true;
            int startDepth = property.depth;
            while (property.NextVisible(children) && property.depth > startDepth)
            {
                children = false;
                if (property.name == "m_Script")
                    continue;
                var entry = SerializePropertyAsEntry(property);
                mirrorComponent.SerializedEntries.Add(entry);
            }

            objectToSerialize.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(mirrorComponent);
            //AssetDatabase.SaveAssetIfDirty(mirrorComponent);
        }
        void ResetMirrorComponent()
        {
            _classNameProperty.stringValue = _innerClassNames[_selectedIndex];
            _serializedEntriesProperty.ClearArray();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnDisable()
        {
            if (_tempComponent != null)
            {
                _tempComponent.Dispose();
            }
            if (_tempObject != null)
                DestroyImmediate(_tempObject);
        }

    }
#endif
}
