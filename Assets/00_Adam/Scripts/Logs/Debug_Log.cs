using UnityEngine;


namespace Photon_IATK
{
    public class Debug_Log : MonoBehaviour
    {   
        static string myLog = "";
        private string output;
        private string stack;

        public TMPro.TextMeshProUGUI DebugLog;

        void Awake()
        {

            if (DebugLog == null)
            {
                if (!HelperFunctions.GetComponentInChild<TMPro.TextMeshProUGUI>(out DebugLog, this.gameObject, System.Reflection.MethodBase.GetCurrentMethod()))
                {

                    Debug.LogFormat(GlobalVariables.cOnDestory + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No input found, destorying self", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    Destroy(this.gameObject);
                }
            }

            TMPro.TextMeshPro component;
            if (HelperFunctions.GetComponentInChild<TMPro.TextMeshPro>(out component, this.gameObject, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                component.text = "Debug Log Window";
            }

                Debug.Log(GlobalVariables.red + "GUI LOG WORKING" + GlobalVariables.endColor + " : " + this.GetType());

            Application.logMessageReceived += Log;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= Log;
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
