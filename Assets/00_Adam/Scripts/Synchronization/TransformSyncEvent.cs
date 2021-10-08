using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Photon_IATK
{
    /// <summary>
    /// Three types of updates with local only send
    /// gaze = vector 3, vector 3, string
    /// hand = string, string
    /// player = vector3, rot
    /// </summary>


    public class TransformSyncEvent : MonoBehaviourPun
    {
        public enum TypeOfTracking
        {
            Gaze,
            Hand,
            Headset
        }

        public TypeOfTracking typeOfTracking;
        public bool isLocal = false;

        public NetworkedGazeDataSender networkedGazeDataSender;
        public NetworkedHandDataSender handDataSender;

        private void Awake()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        private float updateRate = (1 / 30);
        private float nextUpdate = (1/30);
        private void LateUpdate()
        {
            if (Time.time >= nextUpdate)
            {
                // Change the next update (current second+1)
                nextUpdate = Time.time + updateRate;
                // Call your fonction
                if (!isLocal) { return; }

                switch (typeOfTracking)
                {
                    case TypeOfTracking.Gaze:
                        sendGaze();
                        break;
                    case TypeOfTracking.Hand:
                        sendHand();
                        break;
                    case TypeOfTracking.Headset:
                        break;
                    default:
                        break;
                }
            }
        }

        private void sendGaze()
        {
            Vector3 orgin = CoreServices.InputSystem.GazeProvider.GazeOrigin;
            Vector3 direction = CoreServices.InputSystem.GazeProvider.GazeDirection;

            Vector3 newPoint = PlayspaceAnchor.Instance.transform.InverseTransformPoint(orgin);
            Vector3 newPoint1 = PlayspaceAnchor.Instance.transform.InverseTransformPoint(orgin + (direction * .1f));

            GameObject gazeLocation = CoreServices.InputSystem.GazeProvider.GazeTarget;
            string gazeName = "";

            if (gazeLocation != null) { gazeName = gazeLocation.name; }

            object[] content = new object[] { photonView.ViewID, name, PhotonNetwork.NickName, newPoint, newPoint1, gazeName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonSendGazeEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        private void updateGaze(object[] data)
        {
            if (networkedGazeDataSender == null)
            {
                Debug.LogError("Transform sync on " + name + " no gaze sender");
                return;
            }

            networkedGazeDataSender.updateBeam((Vector3)data[3], (Vector3)data[4]);
        }

        private void sendHand()
        {
            if (handDataSender == null) { return; }

            object[] content = new object[] { photonView.ViewID, name, PhotonNetwork.NickName,  handDataSender.updateLocations()};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonSendHandEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        private void updateHand(object[] data)
        {
            if (handDataSender == null) { return; }

            handDataSender.updateFromSerializedHandData((string)data[3]);
        }

        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Debug.LogError(eventCode);

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];

            if (photonView.ViewID != callerPhotonViewID) { return; }

            switch (eventCode)
            {
                case (GlobalVariables.PhotonSendGazeEvent):
                    updateGaze(data);
                    break;
                case (GlobalVariables.PhotonSendHandEvent):
                    updateHand(data);
                    break;
                default:
                    break;
            }
        }
    }
}

