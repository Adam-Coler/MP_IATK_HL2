using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR;
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Experimental.Input;

namespace Photon_IATK
{
    public class OpenVR_Vive_Input_Actions : MonoBehaviour //InputSystemGlobalHandlerListener
    {
        public void Start()
        {
            var inputDevices = new List<InputDevice>();
            UnityEngine.XR.InputDevices.GetDevices(inputDevices);
            foreach (var device in inputDevices)
            {
                Debug.Log(string.Format("Device found with name '{0}' and role '{1}'",
                          device.name, device.role.ToString()));
            }
        }

        //private void Update()
        //{
        //    var inputDevices = new List<InputDevice>();
        //    UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        //    foreach (var device in inputDevices)
        //    {
        //        float buttonState;

        //        bool pressed;
        //        device.TryGetFeatureValue(CommonUsages.trigger, out buttonState);
        //        Debug.Log(string.Format("Device '{0}' and pressed '{1}'",
        //                  device.name, buttonState));
        //        device.TryGetFeatureValue(CommonUsages.triggerButton, out pressed);
        //        Debug.Log(string.Format("Device '{0}' and pressed '{1}'",
        //                  device.name, pressed));

        //              }

        //}

        public void logAction()
        {
            Debug.Log(GlobalVariables.green + "Action" + GlobalVariables.endColor);
        }

    }
}
