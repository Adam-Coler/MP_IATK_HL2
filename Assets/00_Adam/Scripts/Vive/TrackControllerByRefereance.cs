using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

namespace Photon_IATK
{
    public class TrackControllerByRefereance : MonoBehaviour
    {
        public InputDevice thisInputDevice;
        // Start is called before the first frame update

        private void Awake()
        {
            Debug.LogFormat(GlobalVariables.green + "TrackControllerByRefereance attached to {0}" + GlobalVariables.endColor + " : Awake() " + this.GetType(), this.gameObject.name);
        }

        private void UpdateLocation()
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

        private void tryGetInputsStatus()
        {
            var inputFeatures = new List<UnityEngine.XR.InputFeatureUsage>();
            if (thisInputDevice.TryGetFeatureUsages(inputFeatures))
            {
                foreach (var feature in inputFeatures)
                {
                    if (feature.type == typeof(bool))
                    {
                        bool featureValue;
                        if (thisInputDevice.TryGetFeatureValue(feature.As<bool>(), out featureValue))
                        {
                            Debug.Log(string.Format("Bool feature {0}'s value is {1}", feature.name, featureValue.ToString()));
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if (thisInputDevice != null)
            {
                UpdateLocation();
                //tryGetInputsStatus();
            }
        }
    }
}