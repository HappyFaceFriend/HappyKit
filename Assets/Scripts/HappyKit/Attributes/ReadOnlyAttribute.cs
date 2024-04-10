using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif 

namespace HappyKit
{
    /// <summary>
    /// The field with this attribute will be drawn with non-interactable GUI.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {

    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}