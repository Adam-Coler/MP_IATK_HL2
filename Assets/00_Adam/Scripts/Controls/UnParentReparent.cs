using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;



namespace Photon_IATK
{
    [DisallowMultipleComponent]
    public class UnParentReparent : MonoBehaviourPun
    {
        private Transform myParent;
        void Awake()
        {
            myParent = this.transform.parent;
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

            Debug.LogFormat(GlobalVariables.cRegister + "Annotation registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Annotation unregistering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        #region Events
        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];

            //Debug.Log("reciving event: " + eventCode);

            //make sure that this object is the same as the sender object
            if (photonView.ViewID != callerPhotonViewID) { return; }

            switch (eventCode)
            {
                case GlobalVariables.RequestParentEvent:
                    reParent();
                    break;
                case GlobalVariables.RequestUnParentEvent:
                    deParent();
                    break;
                default:
                    break;
            }

        }

        public void Unparent()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestUnParentEvent", "all", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestGrabEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestUnParentEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public void Parent()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestParentEvent", "all", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestGrabEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestParentEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        #endregion //events

        public void deParent()
        {
            if (myParent == null) return;
            this.transform.parent = null;
        }

        public void reParent()
        {
            if (myParent == null) return;
            this.transform.parent = myParent;
        }

    }
}
