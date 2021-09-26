using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using System.Globalization;
using ExitGames.Client.Photon;

namespace Photon_IATK
{
    /// <summary>
    /// This interfaces with the moveplayspace menu to set the playspace location on remote clients. This works well when a single user adjusts play spaces. More tuning is needed if multiple people will be adjusting them. I would start by storing the list of playspace centeres on the master client and polling for them. Then updating them based on update events from vuforia etc...
    /// </summary>
    public class MovePlayspaceInterface : MonoBehaviourPunCallbacks
    {
        private Dictionary<string, Vector3[]> baseValues = new Dictionary<string, Vector3[]>();

        public static MovePlayspaceInterface Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            } else if (Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }
            
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        //public override void OnPlayerLeftRoom(Player other)
        //{
        //    getplayers();
        //    Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);
        //}


        //public override void OnPlayerEnteredRoom(Player other)
        //{
        //    getplayers();
        //    Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        //}


        private void OnEvent(EventData photonEventData)
        {

            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];

            //make sure that this object is the same as the sender object
            if (photonView.ViewID != callerPhotonViewID)
            {
                return;
            }

            //route the event
            switch (eventCode)
            {
                case GlobalVariables.RequestPlayspaceTransform:
                    sendPlayspaceTransform(data);
                    break;
                case GlobalVariables.SendPlayspaceTransform:
                    updateTransformBase(data);
                    break;
                case GlobalVariables.RequestUpdatePlayspaceTransform:
                    updatePlayspaceTransform(data);
                    break;
                case GlobalVariables.RequestHideTrackers:
                    updateTrackerVisability(data);
                    break;
                default:
                    break;
            }
        }

        public void requestPlayspaceTransform(Player selcetedPlayer)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "Player selected, Nickname: {0}, ID: {1}, IsLocal: {2}. Requesting PlayspaceTransform" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", selcetedPlayer.NickName, selcetedPlayer.UserId, selcetedPlayer.IsLocal, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}, requested userID {7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Sending RequestPlayspaceTransform", "All", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestPlayspaceTransform, selcetedPlayer.UserId, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            object[] content = new object[] { photonView.ViewID, selcetedPlayer.UserId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestPlayspaceTransform, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        public void requestTrackerUpdate(Player selcetedPlayer)
        {
            Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}, requested userID {7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Sending RequestHideTrackers", "All", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestHideTrackers, selcetedPlayer.UserId, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, selcetedPlayer.UserId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestHideTrackers, content, raiseEventOptions, GlobalVariables.sendOptions);
        }


        private void sendPlayspaceTransform(object[] data)
        {
            string requestedUserID = (string)data[1];

            if (requestedUserID != PhotonNetwork.LocalPlayer.UserId)
            {
                Debug.LogFormat(GlobalVariables.cCommon + "Not sending playspace transform, UserID is not a match: Requested ID {0}, My ID: {1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", PhotonNetwork.LocalPlayer.UserId, requestedUserID, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}. Pos: {7}, Rot: {8}, ID: {9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Sending SendPlayspaceTransform", "All", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.SendPlayspaceTransform, PlayspaceAnchor.Instance.transform.position.ToString(), PlayspaceAnchor.Instance.transform.rotation.ToString(), PhotonNetwork.LocalPlayer.UserId, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, PlayspaceAnchor.Instance.transform.position, PlayspaceAnchor.Instance.transform.rotation, PhotonNetwork.LocalPlayer.UserId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.SendPlayspaceTransform, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        public void SendUpdatePlayspaceTransform(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, Player playerToMovePlayspaceFor)
        {
            Vector3 newPos;
            Vector3 newRot;

            newPos = new Vector3(posX, posY, posZ) + baseValues[playerToMovePlayspaceFor.UserId][0];
            newRot = new Vector3(rotX, rotY, rotZ) + baseValues[playerToMovePlayspaceFor.UserId][1];

            object[] content = new object[] { photonView.ViewID, newPos, newRot, playerToMovePlayspaceFor.UserId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestUpdatePlayspaceTransform, content, raiseEventOptions, GlobalVariables.sendOptions);

            Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Sending RequestUpdatePlayspaceTransform", "All", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestUpdatePlayspaceTransform, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void updatePlayspaceTransform(object[] data)
        {

            Vector3 position = (Vector3)data[1];
            Vector3 rotation = (Vector3)data[2];
            string requestedUserID = (string)data[3];

            Debug.LogFormat(GlobalVariables.cCommon + "Updated transform recived, position: {0}, rotation: {1}, ID: {2}, MyID: " + PhotonNetwork.LocalPlayer.UserId + "." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", position.ToString(), rotation.ToString(), requestedUserID, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (requestedUserID != PhotonNetwork.LocalPlayer.UserId) { return; }

            PlayspaceAnchor.Instance.transform.position = position;
            PlayspaceAnchor.Instance.transform.eulerAngles = rotation;
        }


        private void updateTransformBase(object[] data)
        {
            Vector3 position = (Vector3)data[1];
            Quaternion rotation = (Quaternion)data[2];
            string userID = (string)data[3];

            Vector3 eulars = rotation.eulerAngles;


            Debug.LogFormat(GlobalVariables.cCommon + "Transform recived, position: {0}, rotation: {1}, ID: {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", position.ToString(), eulars.ToString(), userID, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            baseValues.Add(userID, new Vector3[] { position, eulars });


            Vector3[] tmp;
            if (baseValues.TryGetValue(userID, out tmp)) {
                baseValues[userID] = new Vector3[] { position, eulars };
            } else
            {
                baseValues.Add(userID, new Vector3[] { position, eulars });
            }
        }

        private void updateTrackerVisability(object[] data)
        {
            string userID = (string)data[1];

            if (userID != PhotonNetwork.LocalPlayer.UserId) { return; }

            Debug.LogFormat(GlobalVariables.cEvent + "Hiding trackers{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            GameObject[] Trackers = GameObject.FindGameObjectsWithTag(GlobalVariables.TrackerTag);

            foreach (GameObject tracker in Trackers)
            {
                tracker.transform.GetChild(0).gameObject.SetActive(!tracker.transform.GetChild(0).gameObject.activeSelf); 

            }

        }



    }
}
