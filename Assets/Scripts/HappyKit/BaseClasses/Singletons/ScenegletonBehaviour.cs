using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyKit
{
    /// <summary>
    /// You can think of this as a temporary singleton with a lifetime of a scene. 
    /// The only difference with a HappyTools.SingletonBehaviour is that DontDestroyOnLoad is not called.
    /// This is used to grant global access to a behaviour by using a static property: Instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScenegletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
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
                if (instance.gameObject != gameObject)
                    Destroy(Instance.gameObject);
            }

            instance = GetComponent<T>();
        }
    }

}