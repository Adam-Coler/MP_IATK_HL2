using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.XR;
using Photon.Pun;
using Photon.Realtime;

namespace Photon_IATK
{


    // this script updates the attached objects transfrom to match a controllers transform
    public class OpenVR_Pose_Tracking : ControllerPoseSynchronizer
    {
        public Handedness Handedness;
        public PhotonView photonView;
        private new IMixedRealityController Controller;

        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (photonView.IsMine)
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
            //add code to find the controller if it wasnt found on detect
            //var inputDevices = new List<InputDevice>();
            //UnityEngine.XR.InputDevices.GetDevices(inputDevices);
            //foreach (var device in inputDevices)
            //{
            //    Debug.Log(string.Format("Device found with name '{0}' and role '{1}'",
            //              device.name, device.role.ToString()));

            //}

            if (photonView == null)
            {
                photonView = this.GetComponent<PhotonView>();
                Debug.Log("Awake: Setting Veiw - " + GlobalVariables.green + photonView + GlobalVariables.endColor);
            }


            if (photonView.IsMine)
            {
                IMixedRealityInputSystem inputSystem;
                MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                
                foreach (IMixedRealityController controller in inputSystem.DetectedControllers)
                {
                    Debug.Log(GlobalVariables.green + controller.InputSource + GlobalVariables.endColor);
                    if (controller.ControllerHandedness == Handedness)
                    {
                        Controller = controller;
                        Debug.Log("Awake: Tracking - " + GlobalVariables.green + Handedness + " controller" + GlobalVariables.endColor + " : ID=" + Controller.InputSource.SourceId + " : Name = " + Controller.InputSource.SourceName + " : Source Type = " + Controller.InputSource.SourceType);
                    }

                }
            }
            }


        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            if (photonView.IsMine)
            {
                if (eventData.Controller.ControllerHandedness == Handedness)
                {
                    Controller = eventData.Controller;
                    Debug.Log("OnSourceDetected: Tracking - " + GlobalVariables.green + Handedness + " controller" + GlobalVariables.endColor + " : ID=" + Controller.InputSource.SourceId + " : Name = " + Controller.InputSource.SourceName + " : Source Type = " + Controller.InputSource.SourceType);
                } else
                {
                    Debug.Log("OnSourceDetected: " + GlobalVariables.red + "No controller found" + GlobalVariables.endColor);
                }
            }
        }
    }
}

//== eventData.MixedRealityInputAction