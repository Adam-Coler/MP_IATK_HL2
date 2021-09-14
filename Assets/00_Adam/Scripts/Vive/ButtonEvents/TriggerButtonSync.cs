using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;



namespace Photon_IATK
{
    public class TriggerButtonSync : MonoBehaviour
    {
        private List<InputDevice> devicesWithTriggerButton;

        public GameObject tracker1;
        public GameObject tracker2;
        public GameObject tracker3;

        private GameObject currentTracker;

        private void Awake()
        {
            devicesWithTriggerButton = new List<InputDevice>();
        }

        void OnEnable()
        {
            List<InputDevice> allDevices = new List<InputDevice>();
            InputDevices.GetDevices(allDevices);
            foreach (InputDevice device in allDevices)
            {
                if (device.name.Contains("VIVE"))
                {
                    InputDevices_deviceConnected(device);
                } else
                {
                    allDevices.Remove(device);
                }
            }
                
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

        public bool lastButtonState = false;
        Vector3 tempVector3 = Vector3.zero;
        Quaternion tempRotation = Quaternion.identity;
        void Update()
        {

            bool tempState = false;

            foreach (var device in devicesWithTriggerButton)
            {
                device.TryGetFeatureValue(CommonUsages.triggerButton, out tempState);

                if (tempState != lastButtonState) // Button state changed since last frame
                {
                    lastButtonState = tempState;
                    if (tempState)
                    {
                        Debug.Log(device.name + "button pressed: " + tempState);
                        device.TryGetFeatureValue(CommonUsages.devicePosition, out tempVector3);

                        device.TryGetFeatureValue(CommonUsages.deviceRotation, out tempRotation);

                        updateTracker();
                        currentTracker.transform.position = tempVector3;
                        currentTracker.transform.rotation = tempRotation;

                    } else
                    {
                        Debug.Log(device.name + "button released: " + tempState);
                    }
                    
                }
                device.TryGetFeatureValue(CommonUsages.devicePosition, out tempVector3);
                device.TryGetFeatureValue(CommonUsages.deviceRotation, out tempRotation);
            }
        }

        public Vector3 angle = Vector3.zero;
        public float distSecond = 0;
        public float distFirst = 0;
        private void OnDrawGizmos()
        {
            float r = .005f;
            float scale = 2f;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(tempVector3, r);
            Gizmos.DrawWireSphere(tempVector3, r/ scale);

            Vector3 secondPoint = tempVector3 + (tempRotation * Vector3.forward * distFirst);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(secondPoint, r);
            Gizmos.DrawWireSphere(secondPoint, r/ scale);

            Quaternion test = tempRotation;
            Vector3 eular = test.eulerAngles;
            eular = eular + angle;
            test = Quaternion.Euler(eular);

            Vector3 pos2 = (secondPoint + (test * Vector3.forward * distSecond));

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pos2, r);
            Gizmos.DrawWireSphere(pos2, r/scale);
            Gizmos.DrawLine(secondPoint, pos2);

            Vector3 pos3 = (pos2 + (test * Vector3.forward * distSecond));

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pos3, r);
            Gizmos.DrawWireSphere(pos3, r / scale);
            Gizmos.DrawLine(pos3, pos2);

            //    Gizmos.color = Color.green;
            //    Gizmos.DrawWireSphere(point3, r);

            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawWireSphere(point4, r);

            //    Gizmos.color = Color.black;
            //    Gizmos.DrawWireSphere(middle, r);

            //    Gizmos.color = Color.cyan;
            //    Gizmos.DrawLine(point2, point3);
            //    Gizmos.DrawLine(point2, point4);
            //    Gizmos.DrawLine(point4, point3);
        }

        private void updateTracker()
        {
            if (currentTracker == tracker1)
            {
                currentTracker = tracker2;
            } else if (currentTracker == tracker2)
            {
                currentTracker = tracker3;
            } else if (currentTracker == tracker3)
            {
                currentTracker = tracker1;
            } else
            {
                currentTracker = tracker1;
            }
        }

    }

}
