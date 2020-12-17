using UnityEngine;
using Vuforia;
using UnityEngine.SceneManagement;

namespace Photon_IATK
{
    public class DisableEnableVuforia : MonoBehaviour
    {
        public bool startWithVuforia = true;
        void Awake()
        {
            if(VuforiaBehaviour.Instance == null)
            {

                Debug.Log(GlobalVariables.red + "No VuforiaBehaviour in scene, " + GlobalVariables.endColor + "Awake()" + " : " + this.GetType());
                return;
            }

            Debug.Log(GlobalVariables.green + "Vuforia instance enabled = " + VuforiaBehaviour.Instance.enabled + GlobalVariables.endColor + "Awake()" + " : " + this.GetType());

            VuforiaBehaviour.Instance.enabled = startWithVuforia;

            if (startWithVuforia)
            {
                VuforiaRuntime.Instance.InitVuforia();
                CameraDevice.Instance.Start();
                SceneManager.sceneUnloaded += LevelUnloaded;

                Debug.Log(GlobalVariables.yellow + "Enabling Vuforia Cammera Feed, " + GlobalVariables.endColor + "Awake()" + " : " + this.GetType());
            }

        }

        public void setVuforiaEnabled()
        {
            VuforiaBehaviour.Instance.enabled = !VuforiaBehaviour.Instance.enabled;
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
                Debug.Log(GlobalVariables.purple + "Enabling Vuforia Cammera Feed, " + GlobalVariables.endColor + "setVuforiaEnabled()" + " : " + this.GetType());
            }
        }


        private void LevelUnloaded<Scene>(Scene scene)
        {
            SceneManager.sceneUnloaded -= LevelUnloaded;
            CameraDevice.Instance.Stop();

            VuforiaBehaviour.Instance.enabled = false;
            Debug.Log(GlobalVariables.purple + "Disabling Vuforia Cammera Feed, " + GlobalVariables.endColor + "LevelUnloaded()" + " : " + this.GetType());
        }

    }
}

