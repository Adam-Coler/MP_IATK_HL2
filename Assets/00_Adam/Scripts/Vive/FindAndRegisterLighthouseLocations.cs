using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Photon_IATK
{
    public class FindAndRegisterLighthouseLocations : MonoBehaviour
    {


        public static FindAndRegisterLighthouseLocations Instance;
        public Vector3 closestTrackerPosition;
        public Quaternion closestTrackerRotation;
        public ulong closestTrackerID;
        public InputDevice trackingReferanceDevice;

        private float distanceToClosestTrackingReferance = 999f;


        private void Start()
        {
            if (Instance == null)
            {
                Debug.Log(GlobalVariables.green + "Setting FindAndRegisterLighthouseLocations.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());
                Instance = this;
            }
            else
            {
                if (Instance == this) return;

                Debug.Log(GlobalVariables.green + "Destroying then setting FindAndRegisterLighthouseLocations.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());

                Destroy(Instance.gameObject);
                Instance = this;
            }

            UnityEngine.XR.InputTracking.trackingAcquired += InputTracking_Tracked;

            getTrackingReferances();
        }

        private void InputTracking_Tracked(XRNodeState obj)
        {
            getTrackingReferances();
        }

        private void setPosition()
        {
            this.transform.position = closestTrackerPosition;
            this.transform.rotation = closestTrackerRotation;
        }

        private void OnDestroy()
        {
            UnityEngine.XR.InputTracking.trackingAcquired -= InputTracking_Tracked;
        }


        private void getTrackingReferances()
        {
            var nodeStates = new List<XRNodeState>();
            InputTracking.GetNodeStates(nodeStates);
            nodeStates.RemoveAll(x => x.nodeType != XRNode.TrackingReference);

            foreach(XRNodeState nodeState in nodeStates)
            {
                if (!nodeState.TryGetPosition(out closestTrackerPosition))
                {
                    Debug.LogFormat(GlobalVariables.red + "Tracked node failed to give its position, type: {0}, ID: {1} " + GlobalVariables.endColor + " : " + "getTrackingReferances()" + " : " + this.GetType(), nodeState.nodeType, nodeState.uniqueID);
                    return;
                }


                if (!nodeState.TryGetRotation(out closestTrackerRotation))
                {
                    Debug.LogFormat(GlobalVariables.red + "Tracked node failed to give its rotation, type: {0}, ID: {1} " + GlobalVariables.endColor + " : " + "getTrackingReferances()" + " : " + this.GetType(), nodeState.nodeType, nodeState.uniqueID);
                    return;
                }

                float tmpDistanceToTrackingReferance = Vector3.Distance(Camera.main.transform.position, closestTrackerPosition);
                if (tmpDistanceToTrackingReferance < distanceToClosestTrackingReferance)
                {
                    distanceToClosestTrackingReferance = tmpDistanceToTrackingReferance;
                    setPosition();

                    Debug.LogFormat(GlobalVariables.yellow + "Moving to closest trackingReferance: {0}, Position: {1}, Rotation: {2} " + GlobalVariables.endColor + " : " + "getTrackingReferances()" + " : " + this.GetType(), distanceToClosestTrackingReferance, closestTrackerPosition, closestTrackerRotation);
                    return;
                }

                Debug.LogFormat(GlobalVariables.red + "Tracked node failed to give its rotation, type: {0}, ID: {1} " + GlobalVariables.endColor + " : " + "getTrackingReferances()" + " : " + this.GetType(), nodeState.nodeType, nodeState.uniqueID);
            }


        }
    }
}


//private void setUpLightHouses()
//{

//    var nodeStates = new List<XRNodeState>();
//    InputTracking.GetNodeStates(nodeStates);

//    nodeStates.RemoveAll(x => x.nodeType != XRNode.TrackingReference);

//    Debug.LogFormat(GlobalVariables.yellow + "Node List Length: {0}" + GlobalVariables.endColor + ", setUpLightHouses() : " + this.GetType(), nodeStates.Count);

//}
