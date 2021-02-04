using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class GameSetupAuto : MonoBehaviour
    {

        private Btn_Functions_For_In_Scene_Scripts btn_functions;
        // Start is called before the first frame update
        void Start()
        {
            btn_functions = FindObjectOfType<Btn_Functions_For_In_Scene_Scripts>();
            setup();
        }


        static void enableVR()
        {
            UnityEngine.XR.XRSettings.enabled = true;
            UnityEngine.XR.XRSettings.LoadDeviceByName("OpenVR");
        }

        static void disableVR()
        {
            UnityEngine.XR.XRSettings.enabled = false;
            UnityEngine.XR.XRSettings.LoadDeviceByName("None");
        }

#if DESKTOP
        private void setup()
        {
            Screen.SetResolution(1920, 1080, false);

            if (btn_functions != null)
                btn_functions.showDebugLog();

            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Seting up Desktop Environement", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            disableVR();
        }

#elif HL2

        private void setup()
        {


        }

#elif VIVE

        private void setup()
        {
         enableVR();

        }

#else

#endif


    }
}
