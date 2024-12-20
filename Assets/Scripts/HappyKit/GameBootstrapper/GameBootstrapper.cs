using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyKit
{
    public abstract class GameBootstrapper : MonoBehaviour
    {
        /// <summary>
        /// This is called right before the scene loads, before all Awake() calls.
        /// </summary>
        public abstract void InitGame();
    }
}
