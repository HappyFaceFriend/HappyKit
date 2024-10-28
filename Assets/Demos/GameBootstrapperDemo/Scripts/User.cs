using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBootStrapperDemo
{
    public class User : MonoBehaviour
    {
        [SerializeField] string speakerName;
        private void Awake()
        {
            LogManager.Instance.LogMessage(speakerName);
        }
    }
}
