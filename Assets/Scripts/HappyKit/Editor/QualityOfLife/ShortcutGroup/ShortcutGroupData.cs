using UnityEngine;
using System.Collections.Generic;
using HappyKit;
using UnityEditor;
using Codice.CM.SEIDInfo;
using TMPro;
using Unity.VisualScripting;

namespace HappyKit
{
    [System.Serializable]
    public class Shortcut
    {
        public string Name;
        [SerializeField] string _path;
        [SerializeField] Texture2D _icon = null;
        public Texture2D Icon
        {
            get
            {
                if (IsValid)
                    return _icon;
                else
                    return ErrorIcon;
            }
        }
        public string Path
        {
            get => _path;
            set
            {
                _path = value.Trim();
                IsValid = true;
                var asset = AssetDatabase.LoadAssetAtPath<Object>(_path);
                if (asset == null)
                    IsValid = false;
                else
                {
                    _icon = EditorGUIUtility.ObjectContent(asset, asset.GetType()).image as Texture2D;
                }
            }
        }
        public bool IsValid;
        public bool IsFile => System.IO.Path.HasExtension(Path); 

        static Texture2D ErrorIcon
        {
            get
            {
                if (_errorIcon == null)
                    _errorIcon = EditorGUIUtility.FindTexture("console.erroricon");
                return _errorIcon;
            }
        }
        static Texture2D _errorIcon = null;
        public Shortcut(string name, string path)
        {
            Name = name;
            Path = path;
        }
        public Rect OnGUI(Rect rect, bool indentWithSace = true, int indentSize = 8)
        {
            if (indentWithSace)
            {
                int spaceCount = CountLeadingSpaces(Name);
                rect.x += spaceCount * indentSize;
                rect.width -= spaceCount * indentSize;
            }
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };

            GUIContent buttonName;
            string trimmedName = Name.Trim();

            if (IsValid)
            {
                if (Icon == null)
                    Path = Path;
                buttonName = new GUIContent(trimmedName, Icon, Path);
            }
            else
                buttonName = new GUIContent(trimmedName, ErrorIcon, "This path is invalid");

            if (GUI.Button(rect, buttonName, buttonStyle))
            {
                Selection.activeObject = null;
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Path);
                if (obj != null)
                {
                    EditorUtility.FocusProjectWindow();
                    if (AssetDatabase.IsValidFolder(Path))
                    {
                        // https://discussions.unity.com/t/focus-folder-in-project-window/715790/8
                        // Open the directory in project view.
                        var pt = System.Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
                        var ins = pt.GetField("s_LastInteractedProjectBrowser", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null);
                        var showDirMeth = pt.GetMethod("ShowFolderContents", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        showDirMeth.Invoke(ins, new object[] { obj.GetInstanceID(), true });
                    }
                    else
                    {
                        EditorGUIUtility.PingObject(obj);
                        Selection.activeObject = obj;
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Failed to open path {0}", Path);
                    IsValid = false;
                }
            }
            return rect;
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

    [CreateAssetMenu(fileName = "Shortcut Group Data", menuName = "HappyKit/Editor/Shortcut Group Data", order = 1)]
    public class ShortcutGroupData : ScriptableObject
    {
        [SerializeField] List<Shortcut> _shortcuts = new List<Shortcut>();
        
        public List<Shortcut> Shortcuts { get { return _shortcuts; } }
    }
}