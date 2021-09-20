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

        GameObject thisModel;

        private Dictionary<InputDevice, GameObject> inputs = new Dictionary<InputDevice, GameObject>();

#if VIVE
        // Start is called before the first frame update
        public void setUp()
        {
            //Catch new devices
            InputDevices.deviceConnected += registerDevice;
            InputDevices.deviceDisconnected += removeDevice;

            //Catch existing devices
            InputDevices.GetDevicesWithRole(InputDeviceRole.LeftHanded, devices);
            foreach(InputDevice inputDevice in devices)
            {
                registerDevice(inputDevice);
            }

            InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, devices);
            foreach (InputDevice inputDevice in devices)
            {
                registerDevice(inputDevice);
            }

        }

        private void OnDestroy()
        {
            InputDevices.deviceConnected -= registerDevice;
            InputDevices.deviceDisconnected -= removeDevice;
        }

        private void removeDevice(InputDevice inputDevice)
        {
            GameObject tmpModel;
            if (inputs.TryGetValue(inputDevice, out tmpModel))
            {
                PhotonNetwork.Destroy(tmpModel);
                inputs.Remove(inputDevice);

                Debug.LogFormat(GlobalVariables.cOnDestory + "Input destroyed, {0}, {1}, {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", inputDevice.name, inputDevice.role, name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                //if (inputDevice.role == InputDeviceRole.RightHanded)
                //{
                //    foreach(InputDevice device in inputs.Keys)
                //    {
                //        PhotonNetwork.Destroy(inputs[device]);
                //    }

                //    inputs = new Dictionary<InputDevice, GameObject>();

                //    InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, devices);
                //    foreach (InputDevice input in devices)
                //    {
                //        registerDevice(input);
                //    }

                //    InputDevices.GetDevicesWithRole(InputDeviceRole.LeftHanded, devices);
                //    foreach (InputDevice input in devices)
                //    {
                //        registerDevice(input);
                //    }

                //}
            }
        }

            private void registerDevice(InputDevice inputDevice)
            {
            GameObject tmpModel;
            if (inputs.TryGetValue(inputDevice, out tmpModel)) {
                Debug.LogFormat(GlobalVariables.cTest + "Input already tracked, {0}, {1}, {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", inputDevice.name, inputDevice.role, name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            } else if (inputDevice.role == InputDeviceRole.HardwareTracker)
            {
                Debug.LogFormat(GlobalVariables.cTest + "Input is HardwareTracker, {0}, {1}, {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", inputDevice.name, inputDevice.role, name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            } else 
            {
                Debug.LogFormat(GlobalVariables.cTest + "New input found, {0}, {1}, {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", inputDevice.name, inputDevice.role, name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

                if (inputDevice.name.Contains("VIVE") && !inputDevice.name.Contains("logi"))
                {
                
                    thisModel = PhotonNetwork.Instantiate("ViveController", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

                    HelperFunctions.ParentInSharedPlayspaceAnchor(thisModel, System.Reflection.MethodBase.GetCurrentMethod());

                    thisModel.name = inputDevice.name;
                    TrackControllerByRefereance trackControllerByRefereance = thisModel.AddComponent<TrackControllerByRefereance>();
                    trackControllerByRefereance.thisInputDevice = inputDevice;

                    Debug.LogFormat(GlobalVariables.cInstance + "{0}{1}, {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Instantiated Vive Controller", "", name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    inputs.Add(inputDevice, thisModel);

                } else if (inputDevice.name.Contains("logi"))
                {

                    thisModel = PhotonNetwork.Instantiate("LogitechController", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

                    HelperFunctions.ParentInSharedPlayspaceAnchor(thisModel, System.Reflection.MethodBase.GetCurrentMethod());

                    thisModel.name = inputDevice.name;
                    TrackControllerByRefereance trackControllerByRefereance = thisModel.AddComponent<TrackControllerByRefereance>();
                    trackControllerByRefereance.thisInputDevice = inputDevice;

                    Debug.LogFormat(GlobalVariables.cInstance + "{0}{1}, {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Instantiated Logitech Controller", "", this.gameObject.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    inputs.Add(inputDevice, thisModel);
                }
                
            }

#else
        private void Awake()
        {
            Debug.Log(GlobalVariables.purple + "Destorying LoadContollerModels, Game not set tt Vive" + GlobalVariables.endColor + " : Awake(), " + this.GetType());
            Destroy(this);
        }
#endif
    }
}