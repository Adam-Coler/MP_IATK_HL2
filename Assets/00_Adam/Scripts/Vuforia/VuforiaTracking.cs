using UnityEngine;
using Photon.Pun;
using Vuforia;

namespace Photon_IATK {

    public class VuforiaTracking : MonoBehaviour
    {
        [SerializeField] private GameObject trackerPrefab = default;


        GameObject tracker;
        public void OnTargetFound()
        {

            if (!PhotonNetwork.IsConnected)
            {
                if (tracker == null)
                {
                    tracker = Instantiate(trackerPrefab, Vector3.zero, Quaternion.identity);
                }
            }
            else
            {
                if (tracker == null)
                {
                    tracker = PhotonNetwork.Instantiate(trackerPrefab.name, Vector3.zero, Quaternion.identity);
                }
            }

            tracker.name = "Local Vuforia Tracker";
            tracker.transform.localScale = new Vector3(.25f, .25f, .25f);
            tracker.transform.position = this.gameObject.transform.position;
            tracker.transform.rotation = this.gameObject.transform.rotation;
            //UnityEngine.XR.InputTracking.Recenter();

            Debug.LogFormat(GlobalVariables.green + "{1} Instantiated" + GlobalVariables.endColor + " OnTargetFound() : " + this.GetType(), tracker.name, gameObject.activeSelf);

            centerPlayspace();
        }


        private void centerPlayspace()
        {

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


                tracker.transform.SetParent(playspaceAnchorTransform);

                //if (FindObjectOfType<ImageTargetBehaviour>() != null)
                //{
                //    Destroy(FindObjectOfType<ImageTargetBehaviour>().gameObject);
                //}


                //FindObjectOfType<MRTK_Scene_Manager>().load_01_SetupMenu();
            }
        }

        void Awake()
        {
            DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnStatusChanged);
        }

        void OnStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
        {
            if (tracker == null)
            {
                return;
            }

            if (status == TrackableBehaviour.Status.NO_POSE)
            {
                Destroy(tracker);
            }

            Debug.LogFormat(GlobalVariables.green + "Status is: {0}, statusInfo is: {1}" + GlobalVariables.endColor + " OnStatusChanged() : " + this.GetType(), status, statusInfo);

            centerPlayspace();

            tracker.transform.position = this.gameObject.transform.position;
            tracker.transform.rotation = this.gameObject.transform.rotation;

            Debug.LogFormat(GlobalVariables.green + "Setting Location" + GlobalVariables.endColor + " OnStatusChanged() : " + this.GetType());
        }

        void OnDestroy()
        {
            DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnStatusChanged);
        }
    }
}

//Bad don't move the camera, MRTK discourages this heavily

//Microsoft.MixedReality.Toolkit.MixedRealityPlayspace.Position.Set(newPosition.x, newPosition.y, newPosition.z);
//Microsoft.MixedReality.Toolkit.MixedRealityPlayspace.Rotation.Set(newRotation.x, newRotation.y, newRotation.z, newRotation.w);

//Debug.LogFormat(GlobalVariables.green + "Set New Position:" + newPosition + GlobalVariables.endColor + GlobalVariables.red + " , Old Position: " + oldPosition + GlobalVariables.endColor + " centerPlayspace() : " + this.GetType());
//Debug.LogFormat(GlobalVariables.green + "Set New Rotation:" + newRotation + GlobalVariables.endColor + GlobalVariables.red + " , Old Rotation: " + oldRotation + GlobalVariables.endColor + " centerPlayspace() : " + this.GetType());