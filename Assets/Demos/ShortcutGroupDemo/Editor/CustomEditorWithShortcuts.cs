using UnityEditor;
using UnityEngine;
using HappyKit;

namespace ShortcutGroupDemo
{
    public class CustomEditorWithShortcuts : EditorWindow
    {
        const string ShortcutsPath = "Assets/Demos/ShortcutGroupDemo/Shortcut Group Data.asset";
        ShortcutGroupData _shortcutsData;
        ShortcutEditorGUI _shortcutEditorGUI;
        bool _foldShortcuts = true;

        Shortcut _groupDataShortcut;


        [MenuItem("HappyKit/Demos/Quality of life/Shortcut group")]
        public static void ShowWindow()
        {
            GetWindow<CustomEditorWithShortcuts>();
        }

        private void OnEnable()
        {
            _shortcutsData = AssetDatabase.LoadAssetAtPath<ShortcutGroupData>(ShortcutsPath);
            _shortcutEditorGUI = new ShortcutEditorGUI("Data paths");
            _shortcutEditorGUI.Initialize(_shortcutsData);
            _shortcutEditorGUI.IsLocked = true;

            _groupDataShortcut = new Shortcut("Shortcut Group Data", ShortcutsPath);
        }

        private void OnGUI()
        {
            GUILayout.Label("This shortcut group can be used inside other editors");
            GUILayout.Label("You can make indents by adding space at the front of the name");
            GUILayout.Space(10);

            var titleLabelStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;


            // Shortcut group
            _foldShortcuts = EditorGUILayout.BeginFoldoutHeaderGroup(_foldShortcuts, "Example shortcut group");
            if (_foldShortcuts)
                _shortcutEditorGUI.OnGUI();
            EditorGUILayout.EndFoldoutHeaderGroup();

            GUILayout.Space(10);
            GUILayout.Label("You can have single shortcut inside editor");
            GUILayout.Space(10);

            // Single shortcut
            _groupDataShortcut.OnGUI(GUILayoutUtility.GetRect(50, EditorGUIUtility.singleLineHeight));

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }
    }
}