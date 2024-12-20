using UnityEngine;

namespace HappyKit
{
    /// <summary>
    /// If another instance of this is created, the old instance is destroyed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                        Debug.LogError("No object of " + typeof(T).Name + " is no found");
                }
                return instance;
            }
        }
        public static bool IsValid => instance != null;

        static T instance = null;

        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(Instance.gameObject);
            }

            instance = GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Calling this is used to instantiate the prefab from resources folder.
        /// This is used when you want to create the singleton at a desired moment.
        /// Instance prior to this will get destroyed.
        /// </summary>
        /// <param name="prefabPathInResources">Path of the prefab inside Resources directory that contains this component</param>
        /// <returns></returns>
        protected static T InstantiateInstance(string prefabPathInResources)
        {
            Object resourceOb = Resources.Load(prefabPathInResources);

            Debug.Assert(resourceOb != null, prefabPathInResources);

            if (instance != null)
            {
                Destroy(Instance.gameObject);
            }
            GameObject ob = UnityEngine.Object.Instantiate(resourceOb) as GameObject;
            UnityEngine.Object.DontDestroyOnLoad(ob);
            instance = ob.GetComponent<T>();
            if (instance == null)
                Debug.LogWarningFormat("Resource from {0} doesn't have component {1}.", prefabPathInResources, typeof(T).Name);

            return instance;
        }
    }

}