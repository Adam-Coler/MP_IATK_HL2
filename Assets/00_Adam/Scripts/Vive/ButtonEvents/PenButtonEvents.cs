using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;


namespace Photon_IATK
{
    public class PenTriggerButtonPressEvent : UnityEvent<bool> { }
    public class PenTriggerButtonPressFoceEvent : UnityEvent<float> { }
    public class PenTriggerButtonPressedLocation : UnityEvent<Vector3> { }

    public class PenButtonEvents : MonoBehaviour
    {
        private DrawingVariables drawingVariables;

        public bool penSet;
        private InputDevice penInputDevice;

        public PenTriggerButtonPressEvent penTriggerPress;
        public PenTriggerButtonPressFoceEvent penTriggerPressedForce;
        public PenTriggerButtonPressedLocation penTriggerPressedLocation;

        private float lastButtonPressure = 0f;
        private bool lastButtonState = false;
        private Vector3 lastTriggerLocation = Vector3.zero;

        private void Awake()
        {
            Debug.LogFormat(GlobalVariables.blue + "Primary button event broadcaster added" + GlobalVariables.endColor + " : Awake()" + this.GetType());

            if (penTriggerPress == null)
                penTriggerPress = new PenTriggerButtonPressEvent();

            if (penTriggerPressedForce == null)
                penTriggerPressedForce = new PenTriggerButtonPressFoceEvent();

            if (penTriggerPressedLocation == null)
                penTriggerPressedLocation = new PenTriggerButtonPressedLocation();

            drawingVariables = DrawingVariables.Instance;
            if (drawingVariables == null)
            {
                Debug.LogFormat(GlobalVariables.red + "Drawing variables not found" + GlobalVariables.endColor + " : Awake()" + this.GetType());
            }

        }


        void Update()
        {
            if (!penSet) return;

            bool tempPenPressedState = false;
            float tempPenPressure = 0f;
            Vector3 tempPenPressedLocation = Vector3.zero;

            if (penInputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out tempPenPressedState))
            {
                if(lastButtonState != tempPenPressedState)
                {
                    penTriggerPress.Invoke(tempPenPressedState);
                    lastButtonState = tempPenPressedState;

                    if (GetDrawingVariables())
                    {
                        drawingVariables.isDrawing = tempPenPressedState;
                    }
                }
            }
                

            if (penInputDevice.TryGetFeatureValue(CommonUsages.trigger, out tempPenPressure))
            {
                if (tempPenPressedState && lastButtonPressure != tempPenPressure)
                {
                    penTriggerPressedForce.Invoke(tempPenPressure);
                    lastButtonPressure = tempPenPressure;

                    if (GetDrawingVariables())
                    {
                        drawingVariables.lineWidthFromButtonForce = tempPenPressure;
                    }
                }
            }
                

            if (penInputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out tempPenPressedLocation))
            {
                if (tempPenPressedState && lastTriggerLocation != tempPenPressedLocation)
                {
                    penTriggerPressedLocation.Invoke(tempPenPressedLocation);
                    lastTriggerLocation = tempPenPressedLocation;

                    if (GetDrawingVariables())
                    {
                        drawingVariables.penTipPosition = tempPenPressedLocation;
                    }
                }
            }
        }

        private bool GetDrawingVariables()
        {
            drawingVariables = DrawingVariables.Instance;
            if (drawingVariables != null) return true;
            return false;
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
        }

        private void InputDevices_deviceConnected(InputDevice device)
        {
            bool discardedValue;
            if (!penSet && device.TryGetFeatureValue(CommonUsages.triggerButton, out discardedValue))
            {
                if (device.name.ToLower().Contains("logi"))
                {
                    Debug.Log(GlobalVariables.yellow + "VR-Pen found: " + device.name + GlobalVariables.endColor + " : InputDevices_deviceConnected()" + this.GetType());
                    penInputDevice = device;
                    penSet = true;
                    return;
                }
            }
        }

        private void InputDevices_deviceDisconnected(InputDevice device)
        {
            if (device == penInputDevice)
            {
                penSet = false;
                Debug.Log(GlobalVariables.yellow + "VR-Pen disconnected: " + device.name + GlobalVariables.endColor + " : InputDevices_deviceDisconnected()" + this.GetType());
            }
        }


    }
}
