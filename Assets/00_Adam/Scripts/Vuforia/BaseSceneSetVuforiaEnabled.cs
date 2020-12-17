using UnityEngine;
using Vuforia;


namespace Photon_IATK
{
    public class BaseSceneSetVuforiaEnabled : MonoBehaviour
    {
        public bool startWithVuforia = false;

        void Awake()
        {
            if (VuforiaBehaviour.Instance == null)
            {

                Debug.Log(GlobalVariables.red + "No VuforiaBehaviour in scene, " + GlobalVariables.endColor + "Awake()" + " : " + this.GetType());
                return;
            }

            Debug.Log(GlobalVariables.green + "Vuforia instance enabled = " + VuforiaBehaviour.Instance.enabled + GlobalVariables.endColor + "Awake()" + " : " + this.GetType());

            VuforiaBehaviour.Instance.enabled = startWithVuforia;

            if (VuforiaBehaviour.Instance.enabled)
            {
                VuforiaRuntime.Instance.InitVuforia();
                CameraDevice.Instance.Start();
                Debug.Log(GlobalVariables.yellow + "Enabling Vuforia Cammera Feed, " + GlobalVariables.endColor + "setVuforiaEnabled()" + " : " + this.GetType());
            }
            else
            {
                VuforiaRuntime.Instance.UnloadVuforia();
                CameraDevice.Instance.Stop();
                Debug.Log(GlobalVariables.purple + "Disabling Vuforia Cammera Feed, " + GlobalVariables.endColor + "setVuforiaEnabled()" + " : " + this.GetType());
            }
        }
    }
}
