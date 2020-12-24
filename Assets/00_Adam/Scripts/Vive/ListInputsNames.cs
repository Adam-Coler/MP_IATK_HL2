using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UInput = UnityEngine.Input;
using UnityEngine.XR;
using System.Collections.Generic;

namespace Photon_IATK
{
    public class ListInputsNames : MonoBehaviour
    {

        private string leftJoystickName = "";
        private string rightJoystickName = "";

        // Start is called before the first frame update
        void Start()
        {
            getJoystickNames();

        }

        public void getJoystickNames() {

            string[] joystickNames = UInput.GetJoystickNames();

            foreach (string joystickName in joystickNames)
            {
                if (joystickName.Contains("VIVE") || joystickName.Contains("logi")) {
                    Debug.LogFormat(GlobalVariables.purple + "Joystick found: {0}" + GlobalVariables.endColor + " : getJoystickNames(), " + this.GetType(), joystickName);

                    if (joystickName.Contains("Left"))
                    {
                        leftJoystickName = joystickName;
                    } else if (joystickName.Contains("Left"))
                    {
                        rightJoystickName = joystickName;
                    }
                }
            }
        }

        //Keep this around to avoid creating heap garbage
        static List<InputDevice> devices = new List<InputDevice>();

        private void getHandedController(InputDeviceRole role)
        {
            InputDevices.GetDevices(devices);
            InputDevices.GetDevicesWithRole(role, devices);
            InputDevice device = devices[0];
        }

        //void Update()
        //{
        //    InputDevices.GetDevices(devices);
        //    foreach (InputDevice joystick in devices)
        //    {
        //        Debug.LogFormat(GlobalVariables.purple + "Joystick found: {0}, {1}, {2}" + GlobalVariables.endColor + " : getJoystickNames(), " + this.GetType(), joystick.name, joystick.role, joystick.isValid);
        //    }

        //    InputDevices.GetDevicesWithRole(role, devices);
        //    if (devices.Count > 0)
        //    {
        //        Debug.LogFormat(GlobalVariables.purple + "Joystick found: {0}, {1}, {2}" + GlobalVariables.endColor + " : getJoystickNames(), " + this.GetType(), devices[0].name);
        //        InputDevice device = devices[0];
        //        Vector3 position;
        //        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out position))
        //            this.transform.position = position;
        //        Quaternion rotation;
        //        if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
        //            this.transform.rotation = rotation;
        //    }

        //}
    }
}

//List<IMixedRealityDataProvider> dataProviders = new List<IMixedRealityDataProvider>();
//foreach (IMixedRealityDataProvider dataProvider in dataProviders)
//{
//    Debug.LogFormat(GlobalVariables.purple + "Input found {0}" + GlobalVariables.endColor + " : Start(), " + this.GetType(), dataProvider.Name);
//}