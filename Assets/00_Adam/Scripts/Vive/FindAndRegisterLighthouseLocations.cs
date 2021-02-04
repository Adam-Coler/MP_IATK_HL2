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

        public bool isUseClosestTrackingReferance = true;

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

                if (isUseClosestTrackingReferance)
                {
                    if (tmpDistanceToTrackingReferance < distanceToClosestTrackingReferance)
                    {
                        distanceToClosestTrackingReferance = tmpDistanceToTrackingReferance;
                        setPosition();
                        centerPlayspace();

                        Debug.LogFormat(GlobalVariables.yellow + "Moving to closest trackingReferance: {0}, Position: {1}, Rotation: {2} " + GlobalVariables.endColor + " : " + "getTrackingReferances()" + " : " + this.GetType(), distanceToClosestTrackingReferance, closestTrackerPosition, closestTrackerRotation);
                        return;
                    }
                }
                else
                {
                    if (tmpDistanceToTrackingReferance > distanceToClosestTrackingReferance)
                    {
                        distanceToClosestTrackingReferance = tmpDistanceToTrackingReferance;
                        setPosition();
                        centerPlayspace();

                        Debug.LogFormat(GlobalVariables.yellow + "Moving to farthest trackingReferance: {0}, Position: {1}, Rotation: {2} " + GlobalVariables.endColor + " : " + "getTrackingReferances()" + " : " + this.GetType(), distanceToClosestTrackingReferance, closestTrackerPosition, closestTrackerRotation);
                        return;
                    }
                }


                //Debug.LogFormat(GlobalVariables.red + "Tracked node failed to give its rotation, type: {0}, ID: {1} " + GlobalVariables.endColor + " : " + "getTrackingReferances()" + " : " + this.GetType(), nodeState.nodeType, nodeState.uniqueID);
            }
        }

             private void centerPlayspace()
            {
                    getTrackingReferances();

                    Debug.Log(GlobalVariables.green + "centerPlayspaceCalled" + GlobalVariables.endColor + ", centerPlayspace() : " + this.GetType());

                    if (PlayspaceAnchor.Instance != null)
                    {
                    Transform trackerTransform = this.gameObject.transform;

                    //Move playspace to the tracker
                    Transform playspaceAnchorTransform = PlayspaceAnchor.Instance.transform;
                    playspaceAnchorTransform.position = trackerTransform.position;
                    playspaceAnchorTransform.rotation = trackerTransform.rotation;

                    //get location
                    Vector3 oldPosition = playspaceAnchorTransform.position;
                    Quaternion oldRotation = playspaceAnchorTransform.rotation;

                    ////set up hard coded differance in position between tracker image and vive base station
                    //Vector3 tempPosition = new Vector3(0f, -0.06f, 0f);  //Vector3(0f, 0f, -0.03f); (0f, -0.085f, 0f)
                    //Vector3 tempRotation = new Vector3(90f, 0f, 0f);

                    //if (positionOffset == new Vector3(0f, 0f, 0f))
                    //    positionOffset = tempPosition;

                    //if (rotationOffset == new Vector3(0f, 0f, 0f))
                    //    rotationOffset = tempRotation;

                    //rotate the anchor by that much
                    playspaceAnchorTransform.transform.Rotate(rotationOffset, Space.Self);
                    playspaceAnchorTransform.transform.Translate(positionOffset, Space.Self);

                    Vector3 distanceMoved = oldPosition - playspaceAnchorTransform.transform.position;

                    Debug.LogFormat(GlobalVariables.cCommon + "Moving playspace anchor. Position offset: {0}, Rotation offset: {1}" + GlobalVariables.endColor + GlobalVariables.cAlert + ", Moved distance: {2}, X distance: {3}, Y distance: {4}, Z distance: {5}" + GlobalVariables.endColor + " {6}: {7} -> {8} -> {9}", positionOffset, rotationOffset, distanceMoved, oldPosition.x, oldPosition.y, oldPosition.z, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

            }
#else
        private void Awake()
        {
            Debug.LogFormat(GlobalVariables.cOnDestory + "Destorying {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            Destroy(this.gameObject);
        }
#endif
    }
}