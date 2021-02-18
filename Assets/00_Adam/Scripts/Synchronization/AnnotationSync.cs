using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;


namespace Photon_IATK
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class AnnotationSync : MonoBehaviourPun
    {
        //local vs cloud movement
        //how to

        private Vector3 lastLocalLocation;
        private Quaternion lastLocalRotation;
        private Vector3 lastLocalScale;

        private Vector3 myLastPosition;
        private Vector3 myParentsLastPosition;

        private bool isCoroutineRunning = false;

        /// <summary>
        /// The time that the movement event reciver takes to update its transform
        /// </summary>
        public float time = 1f;

        /// <summary>
        /// The distance required to be moved or rotated before the move event will be called
        /// </summary>
        public float meaningfulDist = .3f;

        private void OnEnable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync unregistering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        private void LateUpdate()
        {
            movementCheck();
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
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sends request to update the objects local position.
        /// Data = (photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale);
        /// </summary>
        private void SendMovementEvent()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "{0}Any ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Sending movement event", "Others", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonMoveEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Transform myTransform = this.gameObject.transform;

            //object[] content = new object[] { photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale };

            object[] content = new object[] { photonView.ViewID, HelperFunctions.PRA(this.gameObject), HelperFunctions.RRA(this.gameObject), myTransform.localScale, this.photonView.GetInstanceID() };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonMoveEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            lastLocalLocation = myTransform.localPosition;
            lastLocalRotation = myTransform.localRotation;
            lastLocalScale = myTransform.localScale;
        }

        private void PhotonMoveEvent(object[] data)
        {

            if (this.photonView.GetInstanceID() == (int)data[4]) { return; }


            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Any ~ {1}{2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonMoveEvent, " Move Event", "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRespondToRequestTransformEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Vector3 newLocalPosition = (Vector3)data[1];
            Quaternion newLocalRotation = (Quaternion)data[2];
            Vector3 newLocalScale = (Vector3)data[3];

            //SetLocalTransformAndLastTransform(newLocalPosition, newLocalRotation, newLocalScale);
            lastLocalLocation = newLocalPosition;
            lastLocalRotation = newLocalRotation;
            lastLocalScale = newLocalScale;
        }

        private void movementCheck()
        {
            //if I moved 
            //did my parent move
            bool iMoved = myLastPosition == transform.localPosition;
            bool myParentMoved = myParentsLastPosition == transform.parent.localPosition;

            myParentsLastPosition = transform.parent.localPosition;
            myLastPosition = transform.localPosition;

            if (iMoved && !myParentMoved)
            {
                SendMovementEvent();
            }

        }

    }
}
