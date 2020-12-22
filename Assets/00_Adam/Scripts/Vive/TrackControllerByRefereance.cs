using UnityEngine;
using UnityEngine.XR;

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