using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyKit
{
    /// <summary>
    /// You need to call InitializeInstance to create the singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : class, new()
    {
        public static T Instance
        {
            get
            {
                Debug.Assert(_instance != null);
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