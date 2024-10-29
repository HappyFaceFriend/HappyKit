using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace HappyKit
{
    public class ShortcutEditorGUI
    {
        ShortcutGroupData _currentShortcutGroupData;
        ReorderableList _shortcutList;
        string _name;
        bool _centerName = false;
        int _nameSize = 10;
        bool _isLocked = false;

        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        public ShortcutGroupData ShortcutGroupData
        {
            get { return _currentShortcutGroupData; }
            set { _currentShortcutGroupData = value; }
        }
        public ShortcutEditorGUI(string name, bool centerName = false, int nameSize = 12)
        {
            _name = name;
            _centerName = centerName;
            _nameSize = nameSize;
        }
        public void Initialize(ShortcutGroupData data)
        {
            _currentShortcutGroupData = data;
            if (data != null)
                SetupReorderableList();
        }

        private void SetupReorderableList()
        {
            if (_currentShortcutGroupData != null)
            {
                List<Shortcut> shortcuts = _currentShortcutGroupData.Shortcuts;
                _shortcutList = new ReorderableList(shortcuts, typeof(Shortcut), true, false, false, false)
                {
                    footerHeight = 0,
                };
                _shortcutList.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Shortcuts");
                };
                _shortcutList.onReorderCallback = (ReorderableList list) =>
                {
                    SaveData();
                };
                _shortcutList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= _currentShortcutGroupData.Shortcuts.Count)
                        return;
                    Shortcut shortcut = shortcuts[index];
                    rect.y += 2;
                    float removeButtonWidth = 16f;
                    float editButtonWidth = 36f;
                    float spacing = 4f;

                    if (!_isLocked)
                    {
                        var buttonRect = shortcut.OnGUI(new Rect(rect.x, rect.y, rect.width - spacing * 2 - removeButtonWidth - editButtonWidth, EditorGUIUtility.singleLineHeight));
                        if (GUI.Button(new Rect(buttonRect.x + buttonRect.width + spacing, rect.y, editButtonWidth, EditorGUIUtility.singleLineHeight), "Edit"))
                        {
                            OpenEditDialog(shortcut);
                        }
                        if (GUI.Button(new Rect(buttonRect.x + buttonRect.width + spacing * 2 + editButtonWidth, rect.y, removeButtonWidth, EditorGUIUtility.singleLineHeight), "-"))
                        {
                            Undo.RegisterCompleteObjectUndo(_currentShortcutGroupData, "Change Shortcut Data");
                            shortcuts.RemoveAt(index);
                            SaveData();
                        }
                    }
                    else
                    {
                        shortcut.OnGUI(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight));
                    }
                };
            }
        }
        void SaveData()
        {
            EditorUtility.SetDirty(_currentShortcutGroupData);
            AssetDatabase.SaveAssetIfDirty(_currentShortcutGroupData);
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label) { alignment = _centerName ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft, fontSize = _nameSize };
            GUILayout.Label(_name, centeredStyle);

            GUIContent lockIcon = _isLocked ? EditorGUIUtility.IconContent("LockIcon-On") : EditorGUIUtility.IconContent("LockIcon");

            // Display the lock/unlock button
            float size = _nameSize * 1.5f;
            GUIStyle lockButtonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.LowerRight };
            lockButtonStyle.imagePosition = ImagePosition.ImageOnly;
            lockButtonStyle.fixedWidth = size;
            lockButtonStyle.fixedHeight = size;
            if (GUILayout.Button(lockIcon, lockButtonStyle, GUILayout.Width(size), GUILayout.Height(size)))
            {
                _isLocked = !_isLocked;
            }
            _shortcutList.draggable = !_isLocked;
            EditorGUILayout.EndHorizontal();

            if (_currentShortcutGroupData == null)
            {
                EditorGUILayout.LabelField("No Shortcut groupd data assigned!");
                return;
            }

            if (_shortcutList == null || _shortcutList.list != _currentShortcutGroupData.Shortcuts)
            {
                SetupReorderableList();
            }

            if (_shortcutList != null)
            {
                _shortcutList.DoLayoutList();
            }

            if (!_isLocked)
                DrawAddShortcutButton();

        }
        void DrawAddShortcutButton()
        {
            var selectedObjects = Selection.objects;
            bool isAssetsSelected = true;
            foreach (var obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (string.IsNullOrEmpty(path))
                {
                    isAssetsSelected = false;
                    break;
                }
            }

            if (isAssetsSelected)
                GUI.enabled = true;
            else
                GUI.enabled = false;
            bool addSelected = false;
            bool startFromBottom = false;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add selected to bottom"))
            {
                addSelected = true;
                startFromBottom = true;
            }
            if (GUILayout.Button("Add selected to top"))
            {
                addSelected = true;
                startFromBottom = false;
            }
            GUILayout.EndHorizontal();
            if (addSelected)
            {
                int index = 0;
                foreach (var obj in selectedObjects)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    string name = "";
                    if (AssetDatabase.IsValidFolder(path))
                        name = new DirectoryInfo(path).Name;
                    else
                        name = Path.GetFileNameWithoutExtension(path);
                    Undo.RegisterCompleteObjectUndo(_currentShortcutGroupData, "Change Shortcut Data");
                    if (startFromBottom)
                        _currentShortcutGroupData.Shortcuts.Add(new Shortcut(name, path));
                    else
                        _currentShortcutGroupData.Shortcuts.Insert(index++, new Shortcut(name, path));
                    SaveData();
                }
            }
            GUI.enabled = true;
        }

        private void OpenEditDialog(Shortcut shortcut)
        {
            EditWindow window = ScriptableObject.CreateInstance<EditWindow>();
            window.titleContent = new GUIContent("Edit shortcut");
            window.Init(shortcut, _currentShortcutGroupData);
            window.ShowUtility();

        }

        public class EditWindow : EditorWindow
        {
            string _newName = "";
            string _newPath = "";
            Shortcut _shortcut = null;
            ShortcutGroupData _shortcutGroupData = null;

            bool _focusTextField;
            private void OnGUI()
            {
                EditorGUILayout.LabelField("Edit shortcut", EditorStyles.boldLabel);
                GUI.SetNextControlName("NameField");
                _newName = EditorGUILayout.TextField("", _newName);

                EditorGUILayout.LabelField("Change path", EditorStyles.boldLabel);
                GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
                textAreaStyle.wordWrap = true;
                _newPath = EditorGUILayout.TextArea(_newPath, textAreaStyle);

                bool apply = false;
                if (GUILayout.Button("Apply"))
                {
                    apply = true;
                }

                Event e = Event.current;
                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
                {
                    apply = true;
                }

                if (apply)
                {
                    Undo.RegisterCompleteObjectUndo(_shortcutGroupData, "Change Shortcut Data");
                    _shortcut.Name = _newName;
                    _shortcut.Path = _newPath;
                    EditorUtility.SetDirty(_shortcutGroupData);
                    AssetDatabase.SaveAssetIfDirty(_shortcutGroupData);
                    Close();
                }
                if (_focusTextField)
                {
                    EditorGUI.FocusTextInControl("NameField");
                    _focusTextField = false;
                }

            }
            public void Init(Shortcut shortcut, ShortcutGroupData groupData)
            {
                _shortcutGroupData = groupData;
                _shortcut = shortcut;
                _newName = shortcut.Name;
                _newPath = shortcut.Path;
                _focusTextField = true;
            }
            private void Update()
            {
                if (!focusedWindow)
                {
                    Focus();
                }
            }
            private void OnLostFocus()
            {
                Focus();
            }
        }
        static int CountLeadingSpaces(string input)
        {
            int spaceCount = 0;
            foreach (char c in input)
            {
                if (c == ' ')
                    spaceCount++;
                else
                    break;
            }
            return spaceCount;
        }
    }
}