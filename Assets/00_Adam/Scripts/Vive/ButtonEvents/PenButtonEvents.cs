using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;


namespace Photon_IATK
{
    public class PenTriggerButtonPressEvent : UnityEvent<bool> { }
    public class PenGripButtonPressEvent : UnityEvent<bool> { }
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
        public PenGripButtonPressEvent penGripButtonPressEvent;

        private float lastButtonPressure = 0f;
        private bool lastButtonState = false;
        private bool lastGripState = false;
        private Vector3 lastTriggerLocation = Vector3.zero;

        private void Awake()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Primary button event broadcaster added", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (penTriggerPress == null)
                penTriggerPress = new PenTriggerButtonPressEvent();

            if (penTriggerPressedForce == null)
                penTriggerPressedForce = new PenTriggerButtonPressFoceEvent();

            if (penTriggerPressedLocation == null)
                penTriggerPressedLocation = new PenTriggerButtonPressedLocation();

            if (penGripButtonPressEvent == null)
                penGripButtonPressEvent = new PenGripButtonPressEvent();

            drawingVariables = DrawingVariables.Instance;
            if (drawingVariables == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Drawing variables not found", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

        }


        void Update()
        {
            if (!penSet) return;

            bool tempGripState = false;
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

                    //tempPenPressedLocation = PlayspaceAnchor.Instance.transform.InverseTransformPoint(tempPenPressedLocation);

                    penTriggerPressedLocation.Invoke(tempPenPressedLocation);
                    lastTriggerLocation = tempPenPressedLocation;

                    if (GetDrawingVariables())
                    {
                        drawingVariables.penTipPosition = tempPenPressedLocation;
                    }
                }
            }

            if (penInputDevice.TryGetFeatureValue(CommonUsages.gripButton, out tempGripState))
            {
                if (tempGripState != lastGripState)
                {
                    penGripButtonPressEvent.Invoke(tempGripState);
                    lastGripState = tempGripState;
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
                    Debug.LogFormat(GlobalVariables.cLevel + "VR-Pen found: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", device.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
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
                Debug.LogFormat(GlobalVariables.cLevel + "VR-Pen disconnected: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", device.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
        }


    }
}
