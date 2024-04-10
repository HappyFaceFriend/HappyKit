using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HappyKit
{
    /// <summary>
    /// This component is used for pooling a prefab.
    /// This will store it's local file ID, which won't work if it is not a prefab stored in disk.
    /// You have to manually reset the ID in the inspector.
    /// </summary>
    public class ObjectWithPrefabID : MonoBehaviour
    {
        [SerializeField] [ReadOnly] long _PrefabID;

        public long ID => _PrefabID;

#if UNITY_EDITOR
        public void ResetID()
        {
            string guid;
            long file;
            //var handle = PrefabUtility.GetPrefabInstanceHandle(this.gameObject);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(gameObject, out guid, out file);
            _PrefabID = file;
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ObjectWithPrefabID), true)]
    public class ObjectWithPrefabIDInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (ObjectWithPrefabID)target;
            if (t.gameObject.scene.name == null)
            {
                string guid;
                long file;
                //var handle = PrefabUtility.GetPrefabInstanceHandle(t.gameObject);
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(t.gameObject, out guid, out file);
                if (file != t.ID)
                {
                    EditorGUILayout.HelpBox("ID doesn't match! Try reseting ID ", MessageType.Error);
                    if (GUILayout.Button("Reset ID"))
                    {
                        t.ResetID();
                        PrefabUtility.SavePrefabAsset(t.gameObject);
                    }
                }
            }
        }
    }
#endif
}