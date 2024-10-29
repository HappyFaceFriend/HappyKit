using UnityEngine;
using UnityEditor;
using HappyKit;

namespace RTD.Ingame
{
    public class ShortcutGroupWindow : EditorWindow
    {
        ShortcutEditorGUI _shortcutEditor;
        ShortcutGroupData _shortcutGroupData;


        [MenuItem("HappyKit/Quality of life/Shortcut Group Window")]
        static void Init()
        {
            var window = GetWindow(typeof(ShortcutGroupWindow));
            window.titleContent = new GUIContent("Custom Shortcuts");
        }
        private void OnEnable()
        {
            var storedDataPath = EditorPrefs.GetString("CustomShortcutGroupDataPath", "INVALID");
            if (storedDataPath != "INVALID")
            {
                _shortcutGroupData = AssetDatabase.LoadAssetAtPath<ShortcutGroupData>(storedDataPath);
                InitData();
            }

        }
        private void OnDisable()
        {
            EditorPrefs.SetString("CustomShortcutGroupDataPath", AssetDatabase.GetAssetPath(_shortcutGroupData));
        }
        void InitData()
        {
            if (_shortcutGroupData != null)
            {
                _shortcutEditor = new ShortcutEditorGUI(_shortcutGroupData.name, false, 16);
                _shortcutEditor.Initialize(_shortcutGroupData);
            }
        }
        public void OnGUI()
        {
            var newShortcutData = EditorGUILayout.ObjectField(_shortcutGroupData, typeof(ShortcutGroupData), false) as ShortcutGroupData;
            if (newShortcutData != _shortcutGroupData)
            {
                _shortcutGroupData = newShortcutData;
                InitData();
            }

            EditorGUILayout.Space();
            if (_shortcutEditor != null)
                _shortcutEditor.OnGUI();

        }
    }
}