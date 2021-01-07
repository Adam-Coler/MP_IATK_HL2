
using Vuforia;
using UnityEngine;

namespace Photon_IATK
{
    public class VuforiaLevelManagement : MonoBehaviour
    {
#if HL2

        void Awake()
        {
            Debug.LogFormat(GlobalVariables.green + "Vuforia setup" + GlobalVariables.endColor + " Awake() : " + this.GetType());

            Init_Unload_Vuforia();

            TrackerManager.Instance.GetStateManager().ReassociateTrackables();

            enableDisableVuforia();

            DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnStatusChanged);

        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.red + "Vuforia takedown" + GlobalVariables.endColor + " OnDestroy() : " + this.GetType());

            enableDisableVuforia();

            Init_Unload_Vuforia();

            destroyVuforiaEmptyObjects();

            DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnStatusChanged);

#if UNITY_EDITOR
            Camera.main.clearFlags = CameraClearFlags.Skybox;
#endif
        }

        public void destroyVuforiaEmptyObjects()
        {
            if (FindObjectsOfType<ImageTargetBehaviour>() != null)
            {
                foreach (ImageTargetBehaviour obj in FindObjectsOfType<ImageTargetBehaviour>())
                {
                    if (obj.gameObject.name.Contains("New"))
                    {
                        Debug.LogFormat(GlobalVariables.red + "Destorying {0}" + GlobalVariables.endColor + " destroyVuforiaEmptyObjects() : " + this.GetType(), obj.gameObject.name);
                        Destroy(obj.gameObject);
                    }
                }
            }
        }

        void OnStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
        {
            Debug.LogFormat(GlobalVariables.green + "Status is: {0}, statusInfo is: {1}" + GlobalVariables.endColor + " OnStatusChanged() : " + this.GetType(), status, statusInfo);

            if (status == TrackableBehaviour.Status.TRACKED && statusInfo ==
            TrackableBehaviour.StatusInfo.NORMAL)
            {
                centerPlayspace();

                Debug.Log(GlobalVariables.green + "Setting Location" + GlobalVariables.endColor + " OnStatusChanged() : " + this.GetType());
            }


        }

        public void centerPlayspace()
        {
            Debug.Log(GlobalVariables.green + "centerPlayspaceCalled" + GlobalVariables.endColor + ", centerPlayspace() : " + this.GetType());

            if (PlayspaceAnchor.Instance != null)
            {
                Transform playspaceAnchorTransform = PlayspaceAnchor.Instance.transform;

                Vector3 newPosition = this.gameObject.transform.position;
                Quaternion newRotation = this.gameObject.transform.rotation;

                Vector3 oldPosition = playspaceAnchorTransform.position;
                Quaternion oldRotation = playspaceAnchorTransform.rotation;

                playspaceAnchorTransform.position = newPosition;
                playspaceAnchorTransform.rotation = newRotation;

                Debug.Log(GlobalVariables.green + "Setting playspaceAnchorTransform," + GlobalVariables.endColor + GlobalVariables.yellow + " New position: " + newPosition + ", New Rotation: " + newRotation + GlobalVariables.endColor + GlobalVariables.red + " Old Position: " + oldPosition + ", Old Rotation: " + oldRotation + GlobalVariables.endColor + ", centerPlayspace() : " + this.GetType());

#if UNITY_EDITOR
                //Can't press virtual button or hard to press virtual button in editor
                if (Btn_Functions_For_In_Scene_Scripts.Instance != null)
                {
                    Debug.Log(GlobalVariables.purple + "In Editor loading new scene on centerplayspace" + GlobalVariables.endColor + " : " + "centerPlayspace()" + " : " + this.GetType());

                    Btn_Functions_For_In_Scene_Scripts.Instance.sceneManager_Load_01_SetupMenu();
                }
#endif
            }
        }

        public void Init_Unload_Vuforia()
        {

            if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZED)
            {
                //VuforiaRuntime.Instance.UnloadVuforia();
                //VuforiaRuntime.Instance.Deinit();
            }
            else if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.NOT_INITIALIZED)
            {
                VuforiaRuntime.Instance.InitVuforia();
            }
            else if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZING)
            {
                Debug.LogFormat(GlobalVariables.yellow + "Vuforia {0}" + GlobalVariables.endColor + " Init_Unload_Vuforia() : " + this.GetType(), VuforiaRuntime.Instance.InitializationState);
            }


            Debug.LogFormat(GlobalVariables.yellow + "Vuforia {0}" + GlobalVariables.endColor + " Init_Unload_Vuforia() : " + this.GetType(), VuforiaRuntime.Instance.InitializationState);

        }

        public void enableDisableVuforia()
        {
            //Debug.LogFormat(GlobalVariables.yellow + "VuforiaBehaviour Enabled = {0}" + GlobalVariables.endColor + " enableDisableVuforia() : " + this.GetType(), VuforiaBehaviour.Instance.enabled);

            VuforiaBehaviour.Instance.enabled = !VuforiaBehaviour.Instance.enabled;


        }

#endif
    }
}

