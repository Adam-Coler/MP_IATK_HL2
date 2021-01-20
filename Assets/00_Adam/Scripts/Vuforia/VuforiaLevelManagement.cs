#if HL2
using Vuforia;
#endif

using UnityEngine;

namespace Photon_IATK
{
    public class VuforiaLevelManagement : MonoBehaviour
    {
        private Vector3 lastPositionOffset = Vector3.zero; //With my setup it looks like (0, -0.05f, -0.065f)
        private Vector3 lastRotationOffset = Vector3.zero; // (90, 0, 0)

        public Vector3 positionOffset;
        public Vector3 rotationOffset;

#if HL2

        private void Update()
        {
            if (lastRotationOffset != rotationOffset)
            {
                lastRotationOffset = rotationOffset;
                centerPlayspace();
            }

            if (lastPositionOffset != positionOffset)
            {

                lastPositionOffset = positionOffset;
                centerPlayspace();
            }
        }

        void Awake()
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Vuforia setup", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Init_Unload_Vuforia();

            TrackerManager.Instance.GetStateManager().ReassociateTrackables();

            enableDisableVuforia();

            Debug.LogFormat(GlobalVariables.cRegister + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Registering Vuforia OnStatusChanged", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnStatusChanged);

        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Vuforia takedown", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            enableDisableVuforia();

            Init_Unload_Vuforia();

            destroyVuforiaEmptyObjects();

            Debug.LogFormat(GlobalVariables.cRegister + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Un-registering Vuforia OnStatusChanged", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnStatusChanged);

#if UNITY_EDITOR
            Camera.main.clearFlags = CameraClearFlags.Skybox;
#endif
        }

        private void destroyVuforiaEmptyObjects()
        {
            if (FindObjectsOfType<ImageTargetBehaviour>() != null)
            {
                foreach (ImageTargetBehaviour obj in FindObjectsOfType<ImageTargetBehaviour>())
                {
                    if (obj.gameObject.name.Contains("New"))
                    {
                        Debug.LogFormat(GlobalVariables.cLevel + "Destorying: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", obj.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                        Destroy(obj.gameObject);

                    }
                }
            }
        }

        private void OnStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "Status is: {0}, statusInfo is: " + statusInfo + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", status, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (status == TrackableBehaviour.Status.TRACKED && statusInfo ==
            TrackableBehaviour.StatusInfo.NORMAL)
            {
                centerPlayspace();

                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Setting Location and removing registration to tracked status change", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                Debug.LogFormat(GlobalVariables.cRegister + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Un-registering Vuforia OnStatusChanged", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnStatusChanged);
            }


        }

        public void centerPlayspace()
        {
            Debug.LogFormat(GlobalVariables.cLevel + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Centering Playspace", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (PlayspaceAnchor.Instance != null)
            {

                Transform trackerTransform = this.gameObject.transform;

                //Move playspace to the tracker
                Transform playspaceAnchorTransform = PlayspaceAnchor.Instance.transform;
                playspaceAnchorTransform.position = trackerTransform.position;
                playspaceAnchorTransform.rotation = trackerTransform.rotation;

                //get location
                Vector3 oldPosition = playspaceAnchorTransform.position;
                Quaternion oldRotation = playspaceAnchorTransform.rotation;

                //set up hard coded differance in position between tracker image and vive base station
                Vector3 tempPosition = new Vector3(0f, -0.06f, 0f);  //Vector3(0f, 0f, -0.03f); (0f, -0.085f, 0f)
                Vector3 tempRotation = new Vector3(90f, 0f, 0f);

                if (positionOffset == new Vector3(0f, 0f, 0f))
                    positionOffset = tempPosition;

                if (rotationOffset == new Vector3(0f, 0f, 0f))
                    rotationOffset = tempRotation;

                //rotate the anchor by that much
                playspaceAnchorTransform.transform.Rotate(rotationOffset, Space.Self);
                playspaceAnchorTransform.transform.Translate(positionOffset, Space.Self);

                Vector3 distanceMoved = oldPosition - playspaceAnchorTransform.transform.position;

                Debug.LogFormat(GlobalVariables.cCommon + "Moving playspace anchor. Position offset: {0}, Rotation offset: {1}" + GlobalVariables.endColor + GlobalVariables.cAlert + ", Moved distance: {2}, X distance: {3}, Y distance: {4}, Z distance: {5}" + GlobalVariables.endColor + " {6}: {7} -> {8} -> {9}", positionOffset, rotationOffset, oldPosition, oldPosition.x, oldPosition.y, oldPosition.z, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                //    playspaceAnchorTransform.position += positionOffset;
                //playspaceAnchorTransform.rotation *= Quaternion.Euler(rotationOffset);

                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Setting playspaceAnchorTransform", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

#if UNITY_EDITOR
                //Can't press virtual button or hard to press virtual button in editor
                //if (Btn_Functions_For_In_Scene_Scripts.Instance != null)
                //{
                //    Debug.Log(GlobalVariables.purple + "In Editor loading new scene on centerplayspace" + GlobalVariables.endColor + " : " + "centerPlayspace()" + " : " + this.GetType());

                //    Btn_Functions_For_In_Scene_Scripts.Instance.sceneManager_Load_01_SetupMenu();
                //}
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
                //Nothing
            }

            Debug.LogFormat(GlobalVariables.cInstance + "Vuforia: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", VuforiaRuntime.Instance.InitializationState, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public void enableDisableVuforia()
        {
            //Debug.LogFormat(GlobalVariables.yellow + "VuforiaBehaviour Enabled = {0}" + GlobalVariables.endColor + " enableDisableVuforia() : " + this.GetType(), VuforiaBehaviour.Instance.enabled);

            VuforiaBehaviour.Instance.enabled = !VuforiaBehaviour.Instance.enabled;


        }

#endif
    }
}

