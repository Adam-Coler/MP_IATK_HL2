using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;



namespace Photon_IATK
{
    public class TriggerButtonSync : MonoBehaviour
    {
        public GameObject tracker1;
        public GameObject tracker2;
        public GameObject tracker3;

        private GameObject currentTracker;
        private bool lastButtonState = false;
        private Vector3 tempVector3 = Vector3.zero;
        private Quaternion tempRotation = Quaternion.identity;
        private List<InputDevice> devicesWithTriggerButton;

        private void Awake()
        {
#if HL2
            Destroy(this.gameObject);
#endif
            devicesWithTriggerButton = new List<InputDevice>();
        }

        void OnEnable()
        {
            getDevices();
            InputDevices.deviceConnected += InputDevices_deviceConnected;
            InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
        }

        public void getDevices()
        {


            List<InputDevice> allDevices = new List<InputDevice>();
            InputDevices.GetDevices(allDevices);

            Debug.LogFormat(GlobalVariables.cCommon + "Getting devices, {0} found" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", allDevices.Count, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod()) ;

            foreach (InputDevice device in allDevices)
            {
                if (!device.name.Contains("logi"))
                {
                    filterDevices(device);

                    Debug.LogFormat(GlobalVariables.cCommon + "Controller connected: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", device.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
        }
        

        private void OnDisable()
        {
#if HL2
            return;
#endif
            InputDevices.deviceConnected -= InputDevices_deviceConnected;
            InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
            devicesWithTriggerButton.Clear();
        }

        private void InputDevices_deviceConnected(InputDevice device)
        {
            getDevices();
        }

        private void filterDevices(InputDevice device)
        {
            bool discardedValue;
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out discardedValue))
            {
                Debug.LogFormat(GlobalVariables.cCommon + "Adding device {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", device.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                devicesWithTriggerButton.Add(device); // Add any devices that have a primary button.
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "Device connected but not added.  Device name: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", device.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
        }



        private void InputDevices_deviceDisconnected(InputDevice device)
        {
            if (devicesWithTriggerButton.Contains(device))
                devicesWithTriggerButton.Remove(device);
        }

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
                        Vector3 middlePos;
                        Quaternion middleRot;
                        getmiddleOfRingOnViveControler(out middlePos, out middleRot);
                        currentTracker.transform.position = middlePos;
                        currentTracker.transform.rotation = middleRot;

                        PlayspaceAnchor.Instance.transform.position = HelperFunctions.getMiddle(new GameObject[] {tracker1, tracker2, tracker3});
                        PlayspaceAnchor.Instance.transform.rotation = HelperFunctions.getRotation(tracker1, tracker2, tracker3);

                    } else
                    {
                        Debug.Log(device.name + "button released: " + tempState);
                    }
                    
                }
            }
        }

        public float angle = -58f;
        public float distSecond = 0.03f;
        public float distFirst = 0.012f;

        private void getmiddleOfRingOnViveControler(out Vector3 middleOfRing, out Quaternion rotation)
        {
            Vector3 edgeOfInnerRing = tempVector3 + (tempRotation * Vector3.forward * distFirst);
            rotation = tempRotation * Quaternion.AngleAxis(angle, Vector3.left);
            middleOfRing = (edgeOfInnerRing + (rotation * Vector3.forward * distSecond));
            rotation = rotation * Quaternion.AngleAxis(180, Vector3.left);
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


//public Vector3 angle = Vector3.zero;
//public float distSecond = 0;
//public float distFirst = 0;
//public Vector3 eluarAngles;
//private void OnDrawGizmos()
//{
//    float r = .005f;
//    float lineLength = .05f;
//    float scale = 2f;

//    Gizmos.color = Color.blue;
//    Gizmos.DrawWireSphere(tempVector3, r / scale);

//    Vector3 secondPoint = tempVector3 + (tempRotation * Vector3.forward * distFirst);

//    Gizmos.color = Color.cyan;
//    Gizmos.DrawWireSphere(secondPoint, r / scale);

//    Gizmos.color = Color.blue;
//    Gizmos.DrawLine(secondPoint, secondPoint + (tempRotation * Vector3.forward * lineLength));

//    Gizmos.color = Color.green;
//    Gizmos.DrawLine(secondPoint, secondPoint + (tempRotation * Vector3.up * lineLength));

//    Gizmos.color = Color.red;
//    Gizmos.DrawLine(secondPoint, secondPoint + (tempRotation * Vector3.left * lineLength));


//    Quaternion test = tempRotation;
//    Vector3 eular = test.eulerAngles;
//    eular = eular + angle;

//    eluarAngles = eular;

//    test = Quaternion.Euler(eular);

//    test = tempRotation * Quaternion.AngleAxis(angle.x, Vector3.left);
//    Vector3 pos2 = (secondPoint + (test * Vector3.forward * distSecond));

//    Gizmos.color = Color.cyan;
//    Gizmos.DrawWireSphere(pos2, r / scale);
//    Gizmos.DrawLine(secondPoint, pos2);

//    Vector3 pos3 = (pos2 + (test * Vector3.forward * distSecond));

//    Gizmos.color = Color.blue;
//    Gizmos.DrawWireSphere(pos3, r / scale);
//    Gizmos.DrawLine(pos3, pos2);

//    Vector3 pos4 = (pos2 + (test * Vector3.left * distSecond));

//    Gizmos.color = Color.magenta;
//    Gizmos.DrawWireSphere(pos4, r / scale);
//    Gizmos.DrawLine(pos4, pos2);

//    Vector3 pos5 = (pos2 + (test * Vector3.right * distSecond));

//    Gizmos.color = Color.red;
//    Gizmos.DrawWireSphere(pos5, r / scale);
//    Gizmos.DrawLine(pos5, pos2);
//}