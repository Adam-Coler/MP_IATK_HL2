using UnityEngine;
using Vuforia;

namespace Photon_IATK
{
    public class btns_Vuforia_Controls : MonoBehaviour
    {

        public void enableDisableLightHouseTarget()
        {
            GameObject ImageTarget = GameObject.FindGameObjectWithTag("ImageTargets");
            if (ImageTarget == null)
            {
                Debug.LogFormat(GlobalVariables.yellow + "{0}" + GlobalVariables.endColor + " enableDisableLightHouseTarget() : " + this.GetType(), "No ImageTargets found");
                return;
            } else
            {
                GameObject gameObject = GameObject.FindGameObjectWithTag("ImageTargets").transform.GetChild(0).gameObject;

                ImageTarget.transform.GetChild(0).gameObject.SetActive(!gameObject.activeSelf);

                Debug.LogFormat(GlobalVariables.yellow + "{0} Active = {1}" + GlobalVariables.endColor + " enableDisableLightHouseTarget() : " + this.GetType(), gameObject.name, gameObject.activeSelf);
            }
        }


        public void Init_Unload_Vuforia()
        {
            if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZED)
            {
                //VuforiaRuntime.Instance.UnloadVuforia();
                VuforiaRuntime.Instance.Deinit();
            } 
            else if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.NOT_INITIALIZED)
            {
                VuforiaRuntime.Instance.InitVuforia();
            }
            else if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZING)
            {
                Debug.LogFormat(GlobalVariables.yellow + "Vuforia {0}" + GlobalVariables.endColor + " Init_Unload_Vuforia() : " + this.GetType(), VuforiaRuntime.Instance.InitializationState);
                return;
            }

                Debug.LogFormat(GlobalVariables.yellow + "Vuforia {0}" + GlobalVariables.endColor + " Init_Unload_Vuforia() : " + this.GetType(), VuforiaRuntime.Instance.InitializationState);
        }

        public void enableDisableVuforia()
        {
            VuforiaBehaviour.Instance.enabled = !VuforiaBehaviour.Instance.enabled;
               
            Debug.LogFormat(GlobalVariables.yellow + "VuforiaBehaviour Enabled = {0}" + GlobalVariables.endColor + " enableDisableVuforia() : " + this.GetType(), VuforiaBehaviour.Instance.enabled);
        }

    }
}


// does not help. it looks like enable behaviour allows init of camera
//public void enableDisableVuforiaCamera()
//{
//    CameraDevice.Instance.IsActive();



//    if (CameraDevice.Instance.IsActive())
//    {
//        CameraDevice.Instance.Stop();
//    }
//    else
//    {

//        foreach (PIXEL_FORMAT px_format in PIXEL_FORMAT.GetValues(typeof(PIXEL_FORMAT)))
//        {
//            if (CameraDevice.Instance.SetFrameFormat(px_format, true))
//            {
//                CameraDevice.Instance.Init();
//                CameraDevice.Instance.Start();
//                Debug.LogFormat(GlobalVariables.green + "PIXEL_FORMAT set to {0}" + GlobalVariables.endColor + " enableDisableVuforiaCamera() : " + this.GetType(), px_format);
//                break;
//            }
//            else
//            {
//                Debug.LogFormat(GlobalVariables.red + "PIXEL_FORMAT is not {0}" + GlobalVariables.endColor + " enableDisableVuforiaCamera() : " + this.GetType(), px_format);
//            }
//        }

//    }

//    Debug.LogFormat(GlobalVariables.yellow + "Camera Enabled = {0}" + GlobalVariables.endColor + " enableDisableVuforia() : " + this.GetType(), CameraDevice.Instance.IsActive());
//}