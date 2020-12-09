using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.XR;

namespace Photon_IATK
{

    // this script updates the attached objects transfrom to match a controllers transform
    public class OpenVR_Pose_Tracking : ControllerPoseSynchronizer
    {
        public Handedness Handedness;
        private new IMixedRealityController Controller;

        public override void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId == Controller?.InputSource.SourceId)
            {
                    TrackingState = TrackingState.Tracked;
                    transform.position = eventData.InputData.Position;
                    transform.rotation = eventData.InputData.Rotation;
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
            IMixedRealityInputSystem inputSystem;
            MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
            Debug.Log(GlobalVariables.green + inputSystem.DetectedControllers + GlobalVariables.endColor);
            foreach (IMixedRealityController controller in inputSystem.DetectedControllers)
            {
                if(controller.ControllerHandedness == Handedness)
                {
                    Controller = controller;
                    Debug.Log(GlobalVariables.green + "Controller Set" + GlobalVariables.endColor);
                }
                
            }
            }


        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            Debug.Log("OnSourceDetected: Attemping to find controller");
            if (eventData.Controller.ControllerHandedness == Handedness)
            {
                Controller = eventData.Controller;
                Debug.Log("OnSourceDetected: Tracking - " + GlobalVariables.green + Handedness + " controller" + GlobalVariables.endColor + " : ID=" + Controller.InputSource.SourceId + " : Name= " + Controller.InputSource.SourceName);
            }

        }
    }
}

//== eventData.MixedRealityInputAction