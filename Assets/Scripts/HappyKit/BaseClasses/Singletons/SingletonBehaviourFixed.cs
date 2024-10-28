using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyKit
{
    /// <summary>
    /// If another instance of this is created, the new instance is destroyed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBehaviourFixed<T> : MonoBehaviour where T : MonoBehaviour
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
            if (instance == null)
            {
                instance = GetComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }


        /// <summary>
        /// Calling this is used to instantiate the prefab from resources folder.
        /// This is used when you want to create the singleton at a desired moment.
        /// This will fail if there is already an instance of T.
        /// </summary>
        /// <param name="prefabPathInResources">Path of the prefab inside Resources directory that contains this component</param>
        /// <returns></returns>
        protected static T InstantiateInstance(string prefabPathInResources)
        {
            Object resourceOb = Resources.Load(prefabPathInResources);

            Debug.Assert(resourceOb != null, prefabPathInResources);

            if (instance != null)
            {
                Debug.LogWarningFormat("Failed InstantiateInstance on {0} becuase instance already exists.", typeof(T).Name);
                return instance;
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