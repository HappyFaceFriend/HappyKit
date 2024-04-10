using UnityEngine;

namespace HappyKit
{
    /// <summary>
    /// A typical singleton component.
    /// If another instance of this is created, the new instance is destroyed.
    /// </summary>
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
    }

}