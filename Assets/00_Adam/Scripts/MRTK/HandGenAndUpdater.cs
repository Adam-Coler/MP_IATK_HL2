using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
namespace Photon_IATK
{
    public class HandGenAndUpdater : MonoBehaviour
    {

        public TrackedHandJoint trackedJoint = TrackedHandJoint.None;
        public Handedness handedness = Handedness.None;
        public GameObject representation;

        private IMixedRealityHandJointService handJointService = null;



        private void Awake()
        {
            if (trackedJoint == TrackedHandJoint.None || handedness == Handedness.None) { return; }

            if (CoreServices.InputSystem != null)
            {
                var dataProviderAccess = CoreServices.InputSystem as IMixedRealityDataProviderAccess;
                if (dataProviderAccess != null)
                {
                    handJointService = dataProviderAccess.GetDataProvider<IMixedRealityHandJointService>();
                }
            }
        }

        void Update()
        {
            if (handJointService != null && representation != null)
            {
                Transform tmp = handJointService.RequestJointTransform(trackedJoint, handedness);
                representation.transform.position = PlayspaceAnchor.Instance.gameObject.transform.InverseTransformPoint(tmp.position);
                representation.transform.rotation = tmp.rotation;

            }

        }
    }
}