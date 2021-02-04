using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.XR;
using Photon.Pun;


namespace Photon_IATK
{
    public class LoadControllerModels : MonoBehaviour
    {
        static List<InputDevice> devices = new List<InputDevice>();

        public bool isLeft = false;

        private InputDeviceRole inputDeviceRole;
        private Handedness handedness;


#if VIVE
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
            //this will not register inputs on the HL2 need something else

            if (inputDevice.role == inputDeviceRole)
            {
                InputDevices.deviceConnected -= registerDevice;
                Debug.LogFormat(GlobalVariables.purple + "InputDevice registered: {0}, {1}" + GlobalVariables.endColor + " : registerDevice(), " + this.GetType(), inputDevice.name, inputDevice.role);

                if (inputDevice.name.Contains("VIVE"))
                {
                    GameObject thisModel = PhotonNetwork.Instantiate("ViveController", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                    thisModel.transform.SetParent(this.transform);
                    //thisModel.GetComponent<GenericNetworkSyncTrackedDevice>().isUser = true;
                    thisModel.name = inputDevice.name;
                    TrackControllerByRefereance trackControllerByRefereance = thisModel.AddComponent<TrackControllerByRefereance>();
                    trackControllerByRefereance.thisInputDevice = inputDevice;


                    Debug.Log(GlobalVariables.purple + "Instantiated Vive Controller" + GlobalVariables.endColor + " : registerDevice(), " + this.GetType());

                } else if (inputDevice.name.Contains("logi"))
                {
                    GameObject thisModel = PhotonNetwork.Instantiate("LogitechController", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                    thisModel.transform.SetParent(this.transform);
                    //thisModel.GetComponent<GenericNetworkSyncTrackedDevice>().isUser = true;
                    thisModel.name = inputDevice.name;
                    TrackControllerByRefereance trackControllerByRefereance = thisModel.AddComponent<TrackControllerByRefereance>();
                    trackControllerByRefereance.thisInputDevice = inputDevice;

                    Debug.Log(GlobalVariables.purple + "Instantiated Logitech Controller" + GlobalVariables.endColor + " : registerDevice(), " + this.GetType());
                }
                
            }
        }

#else
        private void Awake()
        {
            Debug.Log(GlobalVariables.purple + "Deestorying LoadContollerModels, Game not set tt Vive" + GlobalVariables.endColor + " : Awake(), " + this.GetType());
            Destroy(this);
        }
#endif
    }
}