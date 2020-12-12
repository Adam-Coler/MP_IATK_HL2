using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Photon.Pun;
using UnityEngine.XR;

namespace Photon_IATK
{
    public class test_Center_Playspace : MonoBehaviourPunCallbacks
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        private void Awake()
        {
            attachToPlayspaceCenter();
            xr();
            UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceDetected += interactionDelegate;
        }

        private void xr()
        {
            if (!photonView.IsMine) { return; };
            if (XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale))
            {
                GameObject lookit = new GameObject();
                Debug.Log(GlobalVariables.purple + "Room Scale" + GlobalVariables.endColor);
                Debug.Log("OrginTrackingMode:" + GlobalVariables.purple + XRDevice.trackingOriginMode + GlobalVariables.endColor);
                Debug.Log("Model:" + GlobalVariables.purple + XRDevice.model + GlobalVariables.endColor);
                //XRNode.TrackingReference

                foreach (string device in XRSettings.supportedDevices)
                {
                    Debug.Log("Supported: " + GlobalVariables.purple + device + GlobalVariables.endColor);
                }
                
                //Microsoft.MixedReality.Toolkit.WindowsMixedReality.WindowsMixedRealityUtilities.SpatialCoordinateSystem.

            }
            else
            {
                // RoomScale mode was not set successfully.  App cannot make assumptions about where the floor plane is.
                Debug.Log(GlobalVariables.purple + "Failed to set roomscale" + GlobalVariables.endColor);
            }
        }

        private void interactionDelegate(UnityEngine.XR.WSA.Input.InteractionSourceDetectedEventArgs Args)
        {
            Debug.Log("Model:" + GlobalVariables.purple + Args.state + GlobalVariables.endColor);
        }


        private void attachToPlayspaceCenter()
        {
            //if (photonView.IsMine)
            //{
            //    if (Camera.allCameras.Length > 1)
            //    {
            //        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
            //    }
            //}
        }
        // Update is called once per frame
        void Update()
        {
            attachToPlayspaceCenter();
        }
    }
}