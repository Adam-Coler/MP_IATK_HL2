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
    public class MovePlayspace : MonoBehaviourPunCallbacks
    {
        //trigger sending values
        //trigger reciving values
        //trigger update

        public TMP_Dropdown playerDropDown;

        public TMP_InputField posX_TMP;
        public TMP_InputField posY_TMP;
        public TMP_InputField posZ_TMP;
        public TMP_InputField rotX_TMP;
        public TMP_InputField rotY_TMP;
        public TMP_InputField rotZ_TMP;

        private Player[] players;
        private Dictionary<string, Vector3[]> baseValues = new Dictionary<string, Vector3[]>();
        private string currentUserID;

        private void Awake()
        {
            getplayers();

            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        public void dropDownChanged()
        {
            if (playerDropDown.options[playerDropDown.value].text == "Undefined")
            {

            } else if (playerDropDown.options[playerDropDown.value].text == "FindPlayers")
            {
                getplayers();
            } else
            {
                requestPlayspaceTransform();
            }
        }

        public void getplayers()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GetPlayers called{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            players = PhotonNetwork.PlayerList;
            setAxisDropdowns();
        }

        private void clearDropdownOptions()
        {
            playerDropDown.ClearOptions();
        }

        private void setAxisDropdowns()
        {
            clearDropdownOptions();

            List<TMP_Dropdown.OptionData> listDataDimensions = new List<TMP_Dropdown.OptionData>();

            listDataDimensions.Add(new TMP_Dropdown.OptionData() { text = "Undefined" });
            listDataDimensions.Add(new TMP_Dropdown.OptionData() { text = "FindPlayers" });

            foreach (Player player in players)
            {
                listDataDimensions.Add(new TMP_Dropdown.OptionData() { text = player.NickName });
            }

            playerDropDown.AddOptions(listDataDimensions);
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            getplayers();
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);
        }


        public override void OnPlayerEnteredRoom(Player other)
        {
            getplayers();
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        }


        private void OnEvent(EventData photonEventData)
        {
            
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];

            //make sure that this object is the same as the sender object
            if (photonView.ViewID != callerPhotonViewID) {
                Debug.LogFormat("OnEvent will not trigger View ID's do not match. Caller ID: {0}, My ID: {1}", callerPhotonViewID, photonView.ViewID);
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
                default:
                    break;
            }
        }

        public void requestPlayspaceTransform()
        {

            Player selcetedPlayer = players[playerDropDown.value - 2];
            currentUserID = selcetedPlayer.UserId;

            Debug.LogFormat(GlobalVariables.cCommon + "Player selected, Nickname: {0}, ID: {1}, IsLocal: {2}. Requesting PlayspaceTransform" + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", selcetedPlayer.NickName, selcetedPlayer.UserId, selcetedPlayer.IsLocal, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}, requested userID {7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Sending RequestPlayspaceTransform", "All", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestPlayspaceTransform, selcetedPlayer.UserId, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            object[] content = new object[] { photonView.ViewID, selcetedPlayer.UserId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestPlayspaceTransform, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        private void sendPlayspaceTransform(object[] data)
        {
            string requestedUserID = (string)data[1];

            if (requestedUserID != PhotonNetwork.LocalPlayer.UserId) {
                Debug.LogFormat(GlobalVariables.cCommon + "Not sending playspace transform, UserID is not a match: Requested ID {0}, My ID: {1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", PhotonNetwork.LocalPlayer.UserId, requestedUserID, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}. Pos: {7}, Rot: {8}, ID: {9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Sending SendPlayspaceTransform", "All", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.SendPlayspaceTransform, PlayspaceAnchor.Instance.transform.position.ToString(), PlayspaceAnchor.Instance.transform.rotation.ToString(), PhotonNetwork.LocalPlayer.UserId, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, PlayspaceAnchor.Instance.transform.position, PlayspaceAnchor.Instance.transform.rotation, PhotonNetwork.LocalPlayer.UserId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.SendPlayspaceTransform, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;


        public void updateValues()
        {
            float.TryParse(posX_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out posX);
            float.TryParse(posY_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out posY);
            float.TryParse(posZ_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out posZ);
            float.TryParse(rotX_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out rotX);
            float.TryParse(rotY_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out rotY);
            float.TryParse(rotZ_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out rotZ);

            Debug.LogFormat(GlobalVariables.cCommon + "Values updated, Position: X {0}, Y {1}, Z {2}. Rotation: X {3}, Y {4}, Z {5}." + GlobalVariables.endColor + " {6}: {7} -> {8} -> {9}", posX, posY, posZ, rotX, rotY, rotZ, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            SendUpdatePlayspaceTransform();
        }

        private void SendUpdatePlayspaceTransform()
        {
            Vector3 newPos;
            Vector3 newRot;

            currentUserID = players[playerDropDown.value - 2].UserId;

            newPos = new Vector3 (posX, posY, posZ) + baseValues[currentUserID][0];
            newRot = new Vector3(rotX, rotY, rotZ) + baseValues[currentUserID][1];


            object[] content = new object[] { photonView.ViewID, newPos, newRot, currentUserID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestUpdatePlayspaceTransform, content, raiseEventOptions, GlobalVariables.sendOptions);

            Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Sending RequestUpdatePlayspaceTransform", "All", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestUpdatePlayspaceTransform, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void updatePlayspaceTransform(object[] data)
        {
            Vector3 position = (Vector3)data[1];
            Vector3 rotation = (Vector3)data[2];
            string requestedUserID = (string)data[3];

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
            currentUserID = userID;
        }



    }
}
