﻿using Photon.Pun;
using UnityEngine;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Photon_IATK
{
    public class NetworkedHandDataSender : MonoBehaviourPun, IPunObservable
    {
        public bool Wrist = false;
        public bool Palm = false;
        public bool ThumbMetacarpalJoint = false;
        public bool ThumbProximalJoint = false;
        public bool ThumbDistalJoint = false;
        public bool ThumbTip = false;
        public bool IndexMetacarpal = false;
        public bool IndexKnuckle = false;
        public bool IndexMiddleJoint = false;
        public bool IndexDistalJoint = false;
        public bool IndexTip = false;
        public bool MiddleMetacarpal = false;
        public bool MiddleKnuckle = false;
        public bool MiddleMiddleJoint = false;
        public bool MiddleDistalJoint = false;
        public bool MiddleTip = false;
        public bool RingMetacarpal = false;
        public bool RingKnuckle = false;
        public bool RingMiddleJoint = false;
        public bool RingDistalJoint = false;
        public bool RingTip = false;
        public bool PinkyMetacarpal = false;
        public bool PinkyKnuckle = false;
        public bool PinkyMiddleJoint = false;
        public bool PinkyDistalJoint = false;

        public List<TrackedHandJoint> setUpTrackedJoints()
        {
            List<TrackedHandJoint> trackedHandJoints = new List<TrackedHandJoint>();

            if (Wrist ){ trackedHandJoints.Add(TrackedHandJoint.Wrist); }
            if (Palm ){trackedHandJoints.Add(TrackedHandJoint.Palm); }
            if (ThumbMetacarpalJoint ){trackedHandJoints.Add(TrackedHandJoint.ThumbMetacarpalJoint); }
            if (ThumbProximalJoint ){trackedHandJoints.Add(TrackedHandJoint.ThumbProximalJoint); }
            if (ThumbDistalJoint ){trackedHandJoints.Add(TrackedHandJoint.ThumbDistalJoint); }
            if (ThumbTip ){trackedHandJoints.Add(TrackedHandJoint.ThumbTip); }
            if (IndexMetacarpal ){trackedHandJoints.Add(TrackedHandJoint.IndexMetacarpal); }
            if (IndexKnuckle ){trackedHandJoints.Add(TrackedHandJoint.IndexKnuckle); }
            if (IndexMiddleJoint ){trackedHandJoints.Add(TrackedHandJoint.IndexMiddleJoint); }
            if (IndexDistalJoint ){trackedHandJoints.Add(TrackedHandJoint.IndexDistalJoint); }
            if (IndexTip ){trackedHandJoints.Add(TrackedHandJoint.IndexTip); }
            if (MiddleMetacarpal ){trackedHandJoints.Add(TrackedHandJoint.MiddleMetacarpal); }
            if (MiddleKnuckle ){trackedHandJoints.Add(TrackedHandJoint.MiddleKnuckle); }
            if (MiddleMiddleJoint ){trackedHandJoints.Add(TrackedHandJoint.MiddleMiddleJoint); }
            if (MiddleDistalJoint ){trackedHandJoints.Add(TrackedHandJoint.MiddleDistalJoint); }
            if (MiddleTip ){trackedHandJoints.Add(TrackedHandJoint.MiddleTip); }
            if (RingMetacarpal ){trackedHandJoints.Add(TrackedHandJoint.RingMetacarpal); }
            if (RingKnuckle ){trackedHandJoints.Add(TrackedHandJoint.RingKnuckle); }
            if (RingMiddleJoint ){trackedHandJoints.Add(TrackedHandJoint.RingMiddleJoint); }
            if (RingDistalJoint ){trackedHandJoints.Add(TrackedHandJoint.RingDistalJoint); }
            if (RingTip ){trackedHandJoints.Add(TrackedHandJoint.RingTip); }
            if (PinkyMetacarpal ){trackedHandJoints.Add(TrackedHandJoint.PinkyMetacarpal); }
            if (PinkyKnuckle ){trackedHandJoints.Add(TrackedHandJoint.PinkyKnuckle); }
            if (PinkyMiddleJoint ){trackedHandJoints.Add(TrackedHandJoint.PinkyMiddleJoint); }
            if (PinkyDistalJoint ){trackedHandJoints.Add(TrackedHandJoint.PinkyDistalJoint); }

            return trackedHandJoints;
        }

        public Handedness handedness = Handedness.None;

        private SerializeableHandData myHandData = new SerializeableHandData();

        private IMixedRealityHandJointService handJointService = null;

        private List<GameObject> representations = new List<GameObject>();

        public bool isSending = true;
        public bool isShowing = true;
        public bool isUpdating = false;

        private float scale = .015f;

        public Material thumb;
        public Material index;
        public Material middle;
        public Material ring;
        public Material pinky;
        public Material wrist;

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                updateLocations();
                stream.SendNext(HelperFunctions.getJson(myHandData, PhotonNetwork.NickName));
            }
            else
            {
                string handData = (string)stream.ReceiveNext();
                updateFromSerializedHandData(handData);
            }
        }

        private void Awake()
        {
            setUp();
        }

        private void updateFromSerializedHandData(string serializedHandData)
        {
            SerializeableHandData handData = JsonUtility.FromJson<SerializeableHandData>(serializedHandData);

            for (int i = 0; i < representations.Count; i++)
            {
                if (handData.myPositions[i] != new Vector3(10, 10, 10))
                {
                    Debug.LogFormat("Rep {0}, Joint {1}, Pos {2}", representations[i], handData.myJoints[i], handData.myPositions[i]);
                    representations[i].transform.localPosition = handData.myPositions[i];
                } else
                {

                }
            }
        }

        public void setUp()
        {
            HelperFunctions.ParentInSharedPlayspaceAnchor(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());

            foreach (GameObject obj in representations)
            {
                Destroy(obj);
            }

            myHandData.myJoints = new List<string>();
            myHandData.myPositions = new List<Vector3>();
            representations = new List<GameObject>();


            foreach (TrackedHandJoint joint in setUpTrackedJoints())
            {

                Debug.LogFormat("Adding {0} to tracked joints dictionary", joint.ToString());
                myHandData.myJoints.Add(joint.ToString());
                myHandData.myPositions.Add(Vector3.one);

                if (isShowing)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    representations.Add(sphere);
                    sphere.name = joint.ToString();
                    sphere.transform.localScale = new Vector3(scale, scale, scale);
                    sphere.transform.parent = this.transform;

                    Renderer tmp = sphere.GetComponent<Renderer>();
                    if (tmp != null)
                    {
                        tmp.material = getMat(joint.ToString());
                    }
                }
            }

            if (isSending)
            {
                if (CoreServices.InputSystem != null)
                {
                    var dataProviderAccess = CoreServices.InputSystem as IMixedRealityDataProviderAccess;
                    if (dataProviderAccess != null)
                    {
                        handJointService = dataProviderAccess.GetDataProvider<IMixedRealityHandJointService>();
                    }
                }
            }
        }

        private Material getMat(string forJoint)
        {
            Material output;

            if (forJoint.Contains("Met"))
            {
                output = wrist;
            }
            else if (forJoint.Contains("Thumb"))
            {
                output = thumb;
            }
            else if (forJoint.Contains("Index"))
            {
                output = index;
            } else if (forJoint.Contains("Ring"))
            {
                output = ring;
            } else if (forJoint.Contains("Middle"))
            {
                output = middle;
            } else if (forJoint.Contains("Pink"))
            {
                output = pinky;
            } else
            {
                output = wrist;
            }
            return output;
        }


        private void updateLocations()
        {
            if (handedness == Handedness.Right || handedness == Handedness.Left)
            {

                for (int i = 0; i < myHandData.myJoints.Count; i++)
                {
                    TrackedHandJoint jointToTrack = (TrackedHandJoint)System.Enum.Parse(typeof(TrackedHandJoint), myHandData.myJoints[i]);

                    MixedRealityPose _pose;
                    if (HandJointUtils.TryGetJointPose(jointToTrack, handedness, out _pose))
                    {
                        myHandData.myPositions[i] = PlayspaceAnchor.Instance.gameObject.transform.InverseTransformPoint(_pose.Position);
                    } else
                    {
                        myHandData.myPositions[i] = new Vector3(10,10,10);
                    }

                    if (isShowing)
                    {
                        representations[i].transform.position = myHandData.myPositions[i];
                    }

                }
            }
        }


        private void Update()
        {
            if (isShowing && isUpdating)
            {
                updateLocations();
            }
        }
    }

    [System.Serializable]
    public class SerializeableHandData
    {
        public List<string> myJoints = new List<string>();
        public List<Vector3> myPositions = new List<Vector3>();
    }
}
