using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;


namespace trash
{
    public class TriggerButtonEvent : UnityEvent<bool> { }
    public class TriggerButtonFoceEvent : UnityEvent<float> { }

    public class TriggerPressedLocation : UnityEvent<Vector3> { }

    public class TriggerButtonWatcher : MonoBehaviour
    {
        public TriggerButtonEvent TriggerButtonPress;
        public TriggerButtonFoceEvent TriggerButtonPressForce;
        public TriggerPressedLocation triggerPressedLocation;

        private float lastButtonPressure = 0f;
        private bool lastButtonState = false;
        private Vector3 lastTriggerLocation = Vector3.zero;
        private List<InputDevice> devicesWithTriggerButton;

        private void Awake()
        {
            //Debug.LogFormat(GlobalVariables.blue + "Primary button event broadcaster added" + GlobalVariables.endColor + " : Awake()" + this.GetType());

            if (TriggerButtonPress == null)
            {
                TriggerButtonPress = new TriggerButtonEvent();
            }

            if (TriggerButtonPressForce == null)
            {
                TriggerButtonPressForce = new TriggerButtonFoceEvent();
            }

            if (triggerPressedLocation == null)
            {
                triggerPressedLocation = new TriggerPressedLocation();
            }

            devicesWithTriggerButton = new List<InputDevice>();
        }

        void OnEnable()
        {
            List<InputDevice> allDevices = new List<InputDevice>();
            InputDevices.GetDevices(allDevices);
            foreach (InputDevice device in allDevices)
                InputDevices_deviceConnected(device);

            InputDevices.deviceConnected += InputDevices_deviceConnected;
            InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
        }

        private void OnDisable()
        {
            InputDevices.deviceConnected -= InputDevices_deviceConnected;
            InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
            devicesWithTriggerButton.Clear();
        }

        private void InputDevices_deviceConnected(InputDevice device)
        {
            bool discardedValue;
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out discardedValue))
            {
                devicesWithTriggerButton.Add(device); // Add any devices that have a primary button.
            }
        }

        private void InputDevices_deviceDisconnected(InputDevice device)
        {
            if (devicesWithTriggerButton.Contains(device))
                devicesWithTriggerButton.Remove(device);
        }

        void Update()
        {

            float tempPressure = 0f;
            bool tempState = false;
            Vector3 tempVector3 = Vector3.zero;
            
            foreach (var device in devicesWithTriggerButton)
            {
                tempState = false;
                tempState = device.TryGetFeatureValue(CommonUsages.triggerButton, out tempState);

                if (tempState != lastButtonState) // Button state changed since last frame
                {
                    TriggerButtonPress.Invoke(tempState);
                    lastButtonState = tempState;
                }

                //if (tempState)
                //{
                //    device.TryGetFeatureValue(CommonUsages.trigger, out tempPressure);
                //}

                //if (tempState)
                //{
                //    device.TryGetFeatureValue(CommonUsages.devicePosition, out tempVector3);
                //}



                //if (tempState && tempPressure != lastButtonPressure) // Button state changed since last frame
                //{
                //    TriggerButtonPressForce.Invoke(tempPressure);
                //    lastButtonPressure = tempPressure;
                //}

                //if (tempState && tempVector3 != lastTriggerLocation) // Button state changed since last frame
                //{
                //    triggerPressedLocation.Invoke(tempVector3);
                //    lastTriggerLocation = tempVector3;
                //}

            }


        }
    }
}