using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using UInput = UnityEngine.Input;
using UnityEngine.XR;


namespace Photon_IATK
{
    public class LoadControllerModels : MonoBehaviour
    {
        static List<InputDevice> devices = new List<InputDevice>();

        public bool isLeft = false;
        public bool isAutoSetup = false;
        public GameObject viveControllerModel;
        public GameObject logiControllerModel;

        private InputDeviceRole inputDeviceRole;
        private Handedness handedness;

        public InputDevice thisInputDevice;


        private void Awake()
        {
            if (isAutoSetup)
            {
                setUp();
            }
        }

        // Start is called before the first frame update
        public void setUp()
        {
            if (isLeft)
            {
                handedness = Handedness.Left;
                inputDeviceRole = InputDeviceRole.LeftHanded;
            } 
            else
            {
                handedness = Handedness.Right;
                inputDeviceRole = InputDeviceRole.RightHanded;
            }

            //Catch new devices
            InputDevices.deviceConnected += registerDevice;

            //Catch existing devices
            InputDevices.GetDevicesWithRole(inputDeviceRole, devices);
            foreach(InputDevice inputDevice in devices)
            {
                registerDevice(inputDevice);
            }

        }

        private void OnDestroy()
        {
            InputDevices.deviceConnected -= registerDevice;
        }

        private void registerDevice(InputDevice inputDevice)
        {
            if (inputDevice.role == inputDeviceRole)
            {
                InputDevices.deviceConnected -= registerDevice;

                thisInputDevice = inputDevice;

                Debug.LogFormat(GlobalVariables.purple + "InputDevice registered: {0}, {1}" + GlobalVariables.endColor + " : registerDevice(), " + this.GetType(), inputDevice.name, inputDevice.role);

                this.gameObject.name = inputDevice.name;

                if (inputDevice.name.Contains("VIVE"))
                {
                    GameObject thisModel = Instantiate(viveControllerModel, transform.position, transform.rotation);
                    thisModel.transform.SetParent(this.transform);
                    thisModel.transform.rotation = thisModel.transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0));

                } else if (inputDevice.name.Contains("logi"))
                {
                    GameObject thisModel = Instantiate(logiControllerModel, transform.position, transform.rotation);
                    thisModel.transform.SetParent(this.transform);
                }
                
            }
        }

        private void Update()
        {
            if (thisInputDevice != null)
            {

                Vector3 position;
                if (thisInputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out position))
                {
                    this.transform.position = position;
                }


                Quaternion rotation;
                if (thisInputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
                {
                    this.transform.rotation = rotation;
                }
            }
        }
    }
}