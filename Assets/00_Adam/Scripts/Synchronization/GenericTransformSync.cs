using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;


namespace Photon_IATK
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class GenericTransformSync : MonoBehaviourPun
    {

        private Vector3 lastLocalLocation;
        private Quaternion lastLocalRotation;
        private Vector3 lastLocalScale;

        public bool isWaitingForPhotonRequestTransformEvent = true;
        public bool isCoroutineRunning = false;

        /// <summary>
        /// The time that the movement event reciver takes to update its transform
        /// </summary>
        public float time = .2f;

        /// <summary>
        /// The distance required to be moved or rotated before the move event will be called
        /// </summary>
        public float meaningfulDist = .015f;
        private void LateUpdate()
        {
            if (!PhotonNetwork.IsConnected) { return; }
            if (isWaitingForPhotonRequestTransformEvent) { return; }
            if (isCoroutineRunning) { return; }

            CheckIfPositionWasUpdated();
        }

        private void Awake()
        {
            HelperFunctions.ParentInSharedPlayspaceAnchor(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void OnEnable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                PhotonRequestTransformEvent();
                return;
            } else
            {
                isWaitingForPhotonRequestTransformEvent = false;
                Debug.LogFormat(GlobalVariables.cEvent + "GenericTransformSync Not calling PhotonRequestTransformEvent, Is master client.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

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
                break;
            case GlobalVariables.PhotonRequestTransformEvent:
                PhotonRespondToRequestTransformEvent(data);
                break;
            case GlobalVariables.PhotonRespondToRequestTransformEvent:
                    PhotonProcessResponseToRequestTransformEvent(data);
                    break;
            default:
                break;
            }

            //log it
            //Debug.LogFormat(GlobalVariables.cEvent + "Event Found, Sender: {0}, Event code: {1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", photonEventData.Sender, eventCode, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

        }

        private void CheckIfPositionWasUpdated()
        {
            if (isCoroutineRunning) { return; }

            Transform myTransform = this.gameObject.transform;

            float distPosition = Vector3.Distance(myTransform.localPosition, lastLocalLocation);
            float angle = Quaternion.Angle(myTransform.localRotation, lastLocalRotation);
            float distScale = Vector3.Distance(myTransform.localScale, lastLocalScale);

            bool isDistanceMeaningful = distPosition > meaningfulDist;
            bool isAngleMeaningful = angle > meaningfulDist;
            bool isScaleMeaningful = distScale > meaningfulDist;

            if (isDistanceMeaningful || isAngleMeaningful || isScaleMeaningful)
            {
                SendMovementEvent();
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

            object[] content = new object[] { photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale, this.photonView.GetInstanceID() };

            //object[] content = new object[] { photonView.ViewID, HelperFunctions.PRA(this.gameObject), HelperFunctions.RRA(this.gameObject), myTransform.localScale, this.photonView.GetInstanceID() };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonMoveEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            lastLocalLocation = myTransform.localPosition;
            lastLocalRotation = myTransform.localRotation;
            lastLocalScale = myTransform.localScale;
        }

        /// <summary>
        /// Sends request to masterclient for their copy of this viewID's transform.
        /// Data = (photonView.ViewID, this.gameObject.GetInstanceID() );
        /// </summary>
        private void PhotonRequestTransformEvent()
        {
            isWaitingForPhotonRequestTransformEvent = true;

            //Debug.LogFormat(GlobalVariables.cEvent + "{0}Client ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", "", "Requesting transform from master", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestTransformEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, this.gameObject.GetInstanceID() };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestTransformEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        /// <summary>
        /// Updates the objects transform to match the transform sent by the master client.
        /// Expected Data = (0=photonView.ViewID, 1=senderInstanceID, 2=myTransform.localPosition, 3=myTransform.localRotation, 4=myTransform.localScale);
        /// </summary>
        private void PhotonProcessResponseToRequestTransformEvent(object[] data)
        {

            if (!isWaitingForPhotonRequestTransformEvent) 
            {
                //Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Client ~ {1}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonRespondToRequestTransformEvent, "I am not waiting for this event", "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestTransformEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                return;
            }

            //Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Client ~ {1}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonRespondToRequestTransformEvent, " procssing the request", "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestTransformEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

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

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Masterclient ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonRequestTransformEvent, "Responding to transform", "Others", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRespondToRequestTransformEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Transform myTransform = this.gameObject.transform;
            int senderInstanceID = (int)data[1];

            object[] content = new object[] { photonView.ViewID, senderInstanceID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; //Will not recived own message

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRespondToRequestTransformEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        public Vector3 movingToPosition;

        /// <summary>
        /// Updates the objects movement to match the sent transform information.
        /// Expected Data = (photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale);
        /// </summary>
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

            StopAllCoroutines();
            StartCoroutine("LerpToNewLocation");
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

        IEnumerator LerpToNewLocation()
        {
            isCoroutineRunning = true;

            Transform myTransform = this.gameObject.transform;
            float elapsedTime = 0;

            Vector3 startingPos = myTransform.localPosition;
            Quaternion startingRot = myTransform.localRotation ;
            Vector3 startingScale = myTransform.localScale;

            transform.localScale = lastLocalScale;

            while (elapsedTime < time+.1)
            {
                if (transform.localPosition != lastLocalLocation)
                    transform.localPosition = Vector3.Lerp(startingPos, lastLocalLocation, (elapsedTime / time));

                if (transform.localRotation != lastLocalRotation)
                    transform.localRotation = Quaternion.Lerp(startingRot, lastLocalRotation, (elapsedTime / time));

                if (transform.localScale != lastLocalScale)
                    transform.localScale = Vector3.Lerp(startingScale, lastLocalScale, (elapsedTime / time));

                elapsedTime += Time.deltaTime;

                yield return null;

            }
            SetLocalTransformAndLastTransform(lastLocalLocation, lastLocalRotation, lastLocalScale);
            isCoroutineRunning = false;
        }


    }
}
