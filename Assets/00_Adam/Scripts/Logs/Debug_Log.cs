using UnityEngine;


namespace Photon_IATK
{
    public class Debug_Log : MonoBehaviour
    {   
        static string myLog = "";
        private string output;
        private string stack;

        public TMPro.TextMeshPro DebugLog;

        void Start()
        {
            Debug.Log(GlobalVariables.red + "GUI LOG WORKING" + GlobalVariables.endColor + " : " + this.GetType());
        }

        private void Update()
        {

        }

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        private void OnDestroy()
        {
            OnDisable();
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
            DebugLog.text = myLog;

        }

    }
}
