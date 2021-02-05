using UnityEngine;
using System.Reflection;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;


namespace Photon_IATK
{
    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class GenericTransformSync : MonoBehaviourPun
    {

        private Vector3 lastLocalLocation;
        private Quaternion lastLocalRotation;
        private Vector3 lastLocalScale;

        public bool isWaitingForPhotonRequestTransformEvent = true;

        private void Update()
        {
            if (isWaitingForPhotonRequestTransformEvent) { return; }

            Transform myTransform = this.gameObject.transform;
            bool isPositionDifferent = myTransform.localPosition != lastLocalLocation;
            bool isRotationDifferent = myTransform.localRotation != lastLocalRotation;
            bool isScaleDifferent = myTransform.localScale != lastLocalScale;

            if (isPositionDifferent || isRotationDifferent || isScaleDifferent)
            {
                lastLocalLocation = myTransform.localPosition;
                lastLocalRotation = myTransform.localRotation;
                lastLocalScale = myTransform.localScale;

                SendMovementEvent();
            }
        }

        private void OnEnable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

            HelperFunctions.ParentInSharedPlayspaceAnchor(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());
            Debug.LogFormat(GlobalVariables.cEvent + "GenericTransformSync calling PhotonRequestTransformEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            //this will call the master client to send the current transform data

            PhotonRequestTransformEvent();

            
        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync unregistering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        /// <summary>
        /// Checks the relavance of the event then routes the event to the right funciton.
        /// Data = Object[]
        /// </summary>
        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];

            //make sure that this object is the same as the sender object
            if (photonView.ViewID != callerPhotonViewID) { return; }

            //route the event
            switch (eventCode)
            {
            case GlobalVariables.PhotonMoveEvent:
                PhotonMoveEvent(data);
                Debug.Log("PhotonMoveEvent");
                break;
            case GlobalVariables.PhotonRequestTransformEvent:
                PhotonRespondToRequestTransformEvent(data);
                Debug.Log("PhotonRespondToRequestTransformEvent");
                break;
            case GlobalVariables.PhotonRespondToRequestTransformEvent:
                    PhotonProcessResponseToRequestTransformEvent(data);
                    Debug.Log("PhotonProcessResponseToRequestTransformEvent");
                    break;
            default:
                break;
            }

            //log it
            Debug.LogFormat(GlobalVariables.cEvent + "Event Found, Sender: {0}, Event code: {1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", photonEventData.Sender, eventCode, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

        }

        /// <summary>
        /// Sends request to update the objects local position.
        /// Data = (photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale);
        /// </summary>
        private void SendMovementEvent()
        {
            Transform myTransform = this.gameObject.transform;
            object[] content = new object[] { photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonMoveEvent, content, raiseEventOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Sends request to masterclient for their copy of this viewID's transform.
        /// Data = (photonView.ViewID, this.gameObject.GetInstanceID() );
        /// </summary>
        private void PhotonRequestTransformEvent()
        {
            isWaitingForPhotonRequestTransformEvent = true;

            Debug.LogFormat(GlobalVariables.cEvent + "PhotonRequestTransformEvent() is raising the PhotonRequestTransformEvent {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            object[] content = new object[] { photonView.ViewID, this.gameObject.GetInstanceID() };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestTransformEvent, content, raiseEventOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Updates the objects transform to match the transform sent by the master client.
        /// Expected Data = (0=photonView.ViewID, 1=senderInstanceID, 2=myTransform.localPosition, 3=myTransform.localRotation, 4=myTransform.localScale);
        /// </summary>
        private void PhotonProcessResponseToRequestTransformEvent(object[] data)
        {
            if (!isWaitingForPhotonRequestTransformEvent) { return; }

            isWaitingForPhotonRequestTransformEvent = false;

            Debug.LogFormat(GlobalVariables.cEvent + "PhotonProcessResponseToRequestTransformEvent() triggered, procssing the request{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //Check if this clients instance of the view ID was the caller
            //if (this.gameObject.GetInstanceID() != (int)data[1]) { return; }

            //if so process the event
            Vector3 newLocalPosition = (Vector3)data[2];
            Quaternion newLocalRotation = (Quaternion)data[3];
            Vector3 newLocalScale = (Vector3)data[4];

            SetLocalTransformAndLastTransform(newLocalPosition, newLocalRotation, newLocalScale);
            isWaitingForPhotonRequestTransformEvent = false;
        }

        /// <summary>
        /// Respondes to the PhotonRequestTransformEvent()
        /// Data = (photonView.ViewID, senderInstanceID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale);
        /// </summary>
        private void PhotonRespondToRequestTransformEvent(object[] data)
        {
            if (!PhotonNetwork.IsMasterClient) { return; }

            Debug.LogFormat(GlobalVariables.cEvent + "PhotonRespondToRequestTransformEvent() triggered on masterclient, raising the PhotonRespondToRequestTransformEvent{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            Transform myTransform = this.gameObject.transform;
            int senderInstanceID = (int)data[1];

            object[] content = new object[] { photonView.ViewID, senderInstanceID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRespondToRequestTransformEvent, content, raiseEventOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Updates the objects movement to match the sent transform information.
        /// Expected Data = (photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale);
        /// </summary>
        private void PhotonMoveEvent(object[] data)
        {
            Debug.LogFormat(GlobalVariables.cEvent + "PhotonMoveEvent() triggered{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Vector3 newLocalPosition = (Vector3)data[1];
            Quaternion newLocalRotation = (Quaternion)data[2];
            Vector3 newLocalScale = (Vector3)data[3];

            SetLocalTransformAndLastTransform(newLocalPosition, newLocalRotation, newLocalScale);
        }

        private void SetLocalTransformAndLastTransform(Vector3 newLocalPosition, Quaternion newLocalRotation, Vector3 newLocalScale)
        {
            Transform myTransform = this.gameObject.transform;
            myTransform.localPosition = newLocalPosition;
            myTransform.localRotation = newLocalRotation;
            myTransform.localScale = newLocalScale;

            lastLocalLocation = newLocalPosition;
            lastLocalRotation = newLocalRotation;
            lastLocalScale = newLocalScale;
        }

    }
}
