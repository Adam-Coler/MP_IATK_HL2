using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.OpenVR.Input;
using UnityEngine.XR;
using Photon.Pun;
using Photon.Realtime;

namespace Photon_IATK
{
    public class BaseSceneViveCenterPlayspace : MonoBehaviour
    {
#if VIVE

        // Start is called before the first frame update
        void Awake()
        {
            Debug.Log(GlobalVariables.yellow + "Vive setup loaded" + GlobalVariables.endColor + " : Awake(), " + this.GetType());

            FindLighthouse();
        }

        private void FindLighthouse()
        {
            //GameObject leftLightHouse;
            //GameObject rightLightHouse;

            //var inputDevices = new List<InputDevice>();
            //UnityEngine.XR.InputDevices.GetDevices(inputDevices);
            //foreach (var device in inputDevices)
            //{
            //    Debug.LogFormat(GlobalVariables.yellow + "Device: name = {0}, role={1}, device={2}" + GlobalVariables.endColor + ", FindLighthouse() : " + this.GetType(), device.name, device.role, device);
            //}

            //IMixedRealityController Controller;

            //IMixedRealityInputSystem inputSystem;
            //MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);


            //List<XRNodeState> nodeStatesCache = new List<XRNodeState>();
            //UnityEngine.XR.InputTracking.GetNodeStates(nodeStatesCache);

            //foreach (IMixedRealityInputSource controller in inputSystem.DetectedControllers)
            //{
            //    Debug.LogFormat(GlobalVariables.yellow + "Controller: InputSource = {0}, ControllerHandedness={1}, controller={2}" + GlobalVariables.endColor + ", FindLighthouse() : " + this.GetType(), controller.SourceType, controller.SourceName, controller);
            //}

            //foreach (IMixedRealityInputSource DetectedInputSources in inputSystem.DetectedInputSources)
            //{
            //    Debug.LogFormat(GlobalVariables.yellow + "Controller: DetectedInputSources = {0}, ControllerHandedness={1}, DetectedInputSources={2}" + GlobalVariables.endColor + ", FindLighthouse() : " + this.GetType(), DetectedInputSources.SourceName, DetectedInputSources.SourceType, DetectedInputSources);
            //}



            //foreach (XRNodeState xRNode in nodeStatesCache)
            //{
            //    Debug.LogFormat(GlobalVariables.yellow + "xRNode: nodeType = {0}, uniqueID={1}, xRNode={2}" + GlobalVariables.endColor + ", FindLighthouse() : " + this.GetType(), xRNode.nodeType, xRNode.uniqueID, xRNode);
            //}

            //var nodeStates = new List<XRNodeState>();
            //InputTracking.GetNodeStates(nodeStates);

            //foreach (var trackedNode in nodeStates)
            //{
            //    Debug.LogFormat(GlobalVariables.yellow + "trackedNode: nodeType = {0}, uniqueID={1}, trackedNode={2}" + GlobalVariables.endColor + ", FindLighthouse() : " + this.GetType(), trackedNode.nodeType, trackedNode.uniqueID, trackedNode);
            //}




            //var allDevices = new List<InputDevice>();
            //InputDevices.GetDevices(allDevices);

            //foreach (var device in allDevices)
            //{
            //    Debug.LogFormat(GlobalVariables.yellow + "device: name = {0}, role={1}, device={2}" + GlobalVariables.endColor + ", FindLighthouse() : " + this.GetType(), device.name, device.role, device);
            //}

            IMixedRealityDataProvider dataProvider;
            MixedRealityServiceRegistry.TryGetService<IMixedRealityDataProvider>(out dataProvider);

            //IMixedRealityDataProviderAccess dataProviderAccess;
            //var data = dataProviderAccess.GetDataProviders();

            var tmp = CoreServices.GetInputSystemDataProvider<IMixedRealityInputDeviceManager>();

            foreach (var _var in tmp.GetActiveControllers())
            {
                Debug.LogFormat(GlobalVariables.yellow + "IMixedRealityInputDeviceManager: {0}, {1}" + GlobalVariables.endColor + ", FindLighthouse() : " + this.GetType(), _var.InputSource, _var);
            }

            
            var _tmp = CoreServices.InputSystem.DetectedInputSources;

            foreach (var _var in _tmp)
            {
                Debug.LogFormat(GlobalVariables.yellow + "IMixedRealityInputDeviceManager: {0}, {1}" + GlobalVariables.endColor + ", FindLighthouse() : " + this.GetType(), _var.SourceName, _var);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

#endif
}