using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBootStrapperDemo
{
    
    public class GameBootStrapper : HappyTools.GameBootstrapper
    {
        [SerializeField] LogManager _logManager;
        public override void InitGame()
        {
            Debug.Log("Game specific initialization");
            Instantiate(_logManager);
        }
    }
}
