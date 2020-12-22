using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using Photon.Pun;

namespace Photon_IATK
{


    // this script updates the attached objects transfrom to match a controllers transform
    public class OpenVR_Pose_Tracking : ControllerPoseSynchronizer
    {
        public Handedness _handedness;
        public PhotonView photonView;
        private new IMixedRealityController Controller;

        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (photonView.IsMine || PhotonNetwork.IsConnected == false)
            {
                if (eventData.SourceId == Controller?.InputSource.SourceId)
                {
                    TrackingState = TrackingState.Tracked;
                    transform.position = eventData.InputData.Position;
                    transform.rotation = eventData.InputData.Rotation;
                }
            }
        }


        public void Awake()
        {

            if (photonView == null)
            {
                photonView = this.GetComponent<PhotonView>();
                Debug.Log("Awake: Setting Veiw - " + GlobalVariables.green + photonView + GlobalVariables.endColor + " : Awake(), " + this.GetType());
            }


            if (photonView.IsMine || PhotonNetwork.IsConnected == false)
            {
                IMixedRealityInputSystem inputSystem;
                MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                
                foreach (IMixedRealityController controller in inputSystem.DetectedControllers)
                {
                    Debug.Log(GlobalVariables.green + controller.InputSource + GlobalVariables.endColor);
                    if (controller.ControllerHandedness == _handedness)
                    {
                        Controller = controller;
                        Debug.Log("Awake: Tracking - " + GlobalVariables.purple + _handedness + " controller" + GlobalVariables.endColor + " : ID=" + Controller.InputSource.SourceId + " : Name = " + Controller.InputSource.SourceName + " : Source Type = " + Controller.InputSource.SourceType + " : Awake(), " + this.GetType()) ;
                    }
                }
            }
        }

        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            if (photonView.IsMine || PhotonNetwork.IsConnected == false)
            {
                if (eventData.Controller?.ControllerHandedness == _handedness)
                {
                    Controller = eventData.Controller;
                    Debug.Log("OnSourceDetected: Tracking - " + GlobalVariables.green + _handedness + " controller" + GlobalVariables.endColor + " : ID=" + Controller.InputSource.SourceId + " : Name = " + Controller.InputSource.SourceName + " : Source Type = " + Controller.InputSource.SourceType + " : OnSourceDetected(), " + this.GetType());
                } else
                {
                    Debug.Log("OnSourceDetected: " + GlobalVariables.red + "No controller found" + GlobalVariables.endColor + " : OnSourceDetected(), " + this.GetType());
                }
            }
        }
    }
}

//== eventData.MixedRealityInputAction