using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Photon_IATK
{
    public class FindAndRegisterLighthouseLocations : MonoBehaviour
    {
#if VIVE

        public static FindAndRegisterLighthouseLocations Instance;
        public Vector3 closestTrackerPosition;
        public Quaternion closestTrackerRotation;
        public ulong closestTrackerID;
        public InputDevice trackingReferanceDevice;

        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        private Vector3 lastPositionOffset = Vector3.zero;
        private Vector3 lastRotationOffset = Vector3.zero;

        private float distanceToClosestTrackingReferance = 999f;

        private void Update()
        {
            if(lastRotationOffset != rotationOffset)
            {
                lastRotationOffset = rotationOffset;
                centerPlayspace();
            }

            if (lastPositionOffset != positionOffset)
            {

                lastPositionOffset = positionOffset;
                centerPlayspace();
            }

        }

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

            foreach (XRNodeState nodeState in nodeStates)
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
                    centerPlayspace();

                    Debug.LogFormat(GlobalVariables.yellow + "Moving to closest trackingReferance: {0}, Position: {1}, Rotation: {2} " + GlobalVariables.endColor + " : " + "getTrackingReferances()" + " : " + this.GetType(), distanceToClosestTrackingReferance, closestTrackerPosition, closestTrackerRotation);
                    return;
                }

                Debug.LogFormat(GlobalVariables.red + "Tracked node failed to give its rotation, type: {0}, ID: {1} " + GlobalVariables.endColor + " : " + "getTrackingReferances()" + " : " + this.GetType(), nodeState.nodeType, nodeState.uniqueID);
            }
        }

             private void centerPlayspace()
            {
                Debug.Log(GlobalVariables.green + "centerPlayspaceCalled" + GlobalVariables.endColor + ", centerPlayspace() : " + this.GetType());

                if (PlayspaceAnchor.Instance != null)
                {
                    Transform playspaceAnchorTransform = PlayspaceAnchor.Instance.transform;

                    Vector3 newPosition = this.gameObject.transform.position + positionOffset;
                //Quaternion newRotation = this.gameObject.transform.rotation * rotationOffset;

                Quaternion newRotation = this.gameObject.transform.rotation * Quaternion.Euler(rotationOffset);

                //newRotation = Quaternion.Inverse(newRotation);

                Vector3 oldPosition = playspaceAnchorTransform.position;
                    Quaternion oldRotation = playspaceAnchorTransform.rotation;

                    playspaceAnchorTransform.position = newPosition;              
                    playspaceAnchorTransform.rotation = newRotation;

                    Debug.Log(GlobalVariables.green + "Setting playspaceAnchorTransform," + GlobalVariables.endColor + GlobalVariables.yellow + " New position: " + newPosition + ", New Rotation: " + newRotation + GlobalVariables.endColor + GlobalVariables.red + " Old Position: " + oldPosition + ", Old Rotation: " + oldRotation + GlobalVariables.endColor + ", centerPlayspace() : " + this.GetType());
                }

            }
#else
        private void Awake()
        {
                            Debug.LogFormat(GlobalVariables.red + "Destorying {0} " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType(), this.name);
            Destroy(this);
        }
#endif
    }
}