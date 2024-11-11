using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyKit;

namespace GameBootStrapperDemo
{
    public class GameBootStrapper : GameBootstrapper
    {
        [SerializeField] LogManager _logManager;
        public override void InitGame()
        {
            Debug.Log("Game specific initialization");
            Instantiate(_logManager);
        }
    }
}
