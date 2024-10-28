using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyKit
{
    /// <summary>
    /// This will create the instance at the first call of the property Instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazySingleton<T> where T : class, new()
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    InitializeInstance();
                return _instance;
            }
        }
        public static bool IsValid => _instance != null;
        static T _instance = null;

        public static T InitializeInstance()
        {
            _instance = new T();
            return _instance;
        }
    }

}