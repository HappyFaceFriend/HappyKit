using UnityEngine;

namespace HappyKit
{
    /// <summary>
    /// This is similar to a singleton, but is replaced when another instance is created.
    /// If another instance of this is created, the old instance will be destroyed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBehaviourWeak<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                        Debug.LogError("No object of " + typeof(T).Name + " is not found");
                }
                return instance;
            }
        }
        /// <summary>
        /// true if Instance is not null
        /// </summary>
        public static bool IsValid => instance != null;

        static T instance = null;

        protected virtual void Awake()
        {
            if (instance != null)
            {
                if (instance.gameObject != null)
                    Destroy(Instance.gameObject);
            }

            instance = GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
    }

}