using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;

namespace Photon_IATK
{
    public class TrackerToFinger : MonoBehaviour
    {
        public bool isLogToDebug = false;
        private GameObject tracker;
        // Start is called before the first frame update
        void Awake()
        {
            tracker = PhotonNetwork.Instantiate("Tracker", Vector3.zero, Quaternion.identity);

            tracker.transform.SetParent(PlayspaceAnchor.Instance.gameObject.transform);

            tracker.name = "FingerTip";
            tracker.transform.localScale = new Vector3(.5f,.5f,.5f);
        }

              // Update is called once per frame
            void Update()
        {
                if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose pose))
                {
                MixedRealityPose _pose = new MixedRealityPose();
                HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out _pose);

                if (isLogToDebug)
                {
                    Debug.Log(GlobalVariables.green + "Setting tracker pose," + GlobalVariables.endColor + GlobalVariables.yellow + " New position: " + _pose.Position + GlobalVariables.endColor + GlobalVariables.red + ", Rotation: " + _pose.Rotation + GlobalVariables.endColor + ", Update() : " + this.GetType());
                }

                tracker.transform.position = _pose.Position;
                tracker.transform.rotation = _pose.Rotation;
            }
            }
    }
}
