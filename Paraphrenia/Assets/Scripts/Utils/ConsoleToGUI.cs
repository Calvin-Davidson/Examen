using System;
using UnityEngine;

namespace Utils
{
    public class ConsoleToGUI : MonoBehaviour
    {
#if (!UNITY_EDITOR)
        static string _myLog = "";
        private string _output;

        private bool _active = false;
        void OnEnable()
        {
            DontDestroyOnLoad(this.gameObject);
            Application.logMessageReceived += Log;
        }
     
        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }
     
        public void Log(string logString, string stackTrace, LogType type)
        {
            _output = logString;
            _myLog = _output + "\n" + _myLog;
            if (_myLog.Length > 5000)
            {
                _myLog = _myLog.Substring(0, 4000);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab)) _active = !_active;
        }

        void OnGUI()
        {
            if (!_active) return;
            if (!Application.isEditor)
            {
                _myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), _myLog);
            }
        }
#endif
    }
}