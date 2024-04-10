using UnityEngine;

namespace HappyKit
{
    /// <summary>
    /// A singleton-like interface for component in scene.
    /// If another instance of this is created, the old instance is destroyed.
    /// This is used to grant static access to some components.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Scenegleton<T> : MonoBehaviour where T : MonoBehaviour
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
        }
    }

}