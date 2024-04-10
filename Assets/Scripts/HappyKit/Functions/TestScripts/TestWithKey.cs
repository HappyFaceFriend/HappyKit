using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HappyKit
{
#pragma warning disable 0612

    /// <summary>
    /// TestWithKey.GetKeyDown is used instead of Input.GetKeyDown when you are testing a functionality.
    /// This will add labels and compiler warnings so that you don't forget to remove the test code.
    /// </summary>
    public static class TestWithKey
    {
        static GUIComponent _guiObject;

        [Obsolete("Test script is used")]
        /// <summary>
        /// This is used instead of Input.GetKeyDown when you are testing a functionality.
        /// This will add labels and compiler warnings so that you don't forget to remove the test code.
        /// </summary>
        public static bool GetKeyDown(KeyCode keyCode, string label = "", [CallerFilePath] string sourceFilePath = "")
        {
            _guiObject.AddKey(keyCode, label, sourceFilePath);
            if (UnityEngine.Input.GetKeyDown(keyCode))
                return true;
            else
                return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void CreateGUI()
        {
            GameObject go = new GameObject("TestWithKey GUI");
            _guiObject = go.AddComponent<GUIComponent>();
            GameObject.DontDestroyOnLoad(go.gameObject);
        }

        class GUIComponent : MonoBehaviour
        {
            class GUIData
            {
                public KeyCode KeyCode;
                public string CallerInfo;
                public string Label;
            }
            List<GUIData> _guiDatas = new List<GUIData>();
            public void AddKey(KeyCode keyCode, string label, string callerInfo)
            {
                if (!_guiDatas.Exists(x => x.KeyCode == keyCode && x.CallerInfo == callerInfo && x.Label == label))
                    _guiDatas.Add(new GUIData { KeyCode = keyCode, CallerInfo = callerInfo, Label = label });
            }
            private void OnGUI()
            {
                var centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.UpperRight;
                centeredStyle.fontSize = 30;
                for (int i = 0; i < _guiDatas.Count; i++)
                {
                    string label = _guiDatas[i].Label;
                    label += " (" + System.IO.Path.GetFileName(_guiDatas[i].CallerInfo) + ")";
                    GUI.Label(new Rect(0, 35 + i * 35, Screen.width, 50), _guiDatas[i].KeyCode.ToString() + ": " + label, centeredStyle);
                }
            }
        }
    }

#pragma warning restore 0612
}