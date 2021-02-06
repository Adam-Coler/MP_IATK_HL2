using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using IATK;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Photon_IATK
{
    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class AnnotationManagerSaveLoadEvents : MonoBehaviourPun
    {
        public bool isWaitingForListOfAnnotationIDs = false;

        private List<int> myListOfAnnotationIDs
        {
            get
            {
                Annotation[] annotations = FindObjectsOfType<Annotation>();
                List<int> listOfIDs = new List<int> { };
                foreach (Annotation annotation in annotations)
                {
                    listOfIDs.Add(annotation.myUniqueAnnotationNumber);
                }

                return listOfIDs;
            }
        }

        private void OnEnable()
        {
            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate += UpdatedView;
            VisualizationEvent_Calls.RPCvisualisationUpdateRequestDelegate += UpdatedViewRequested;

            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

            Debug.LogFormat(GlobalVariables.cRegister + "AnnotationManagerSaveLoadEvents registering OnEvent, RPCvisualisationUpdateRequestDelegate, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync unregistering OnEvent, RPCvisualisationUpdateRequestDelegate, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
            VisualizationEvent_Calls.RPCvisualisationUpdateRequestDelegate -= UpdatedViewRequested;
            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate -= UpdatedView;

        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} updated. Requesting Annotation List of IDs, Name: " + PhotonNetwork.NickName + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            RequestAnnotationsListOfIDs();
        }

        private void UpdatedViewRequested(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} update requested." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];

            //make sure that this object is the same as the sender object
            if (photonView.ViewID != callerPhotonViewID) { return; }

            //Debug.LogFormat(GlobalVariables.cEvent + "Event Found, Sender: {0}, Event code: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", photonEventData.Sender, eventCode, PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            //route the event
            switch (eventCode)
            {
                case GlobalVariables.PhotonRequestAnnotationsListOfIDsEvent:
                    RespondToRequestAnnotationsListOfIDs(data);
                    break;
                case GlobalVariables.PhotonResponseRequestAnnotationsListOfIDsEventNONE_FOUNDEvent:
                   ProcessResponseToRequestAnnotationsListOfIDsNONE_FoundEvent();
                    break;
                case GlobalVariables.PhotonResponseToRequestAnnotationsListOfIDsEvent:
                    ProcessResponseToRequestAnnotationsListOfIDs(data);
                    break;
                case GlobalVariables.PhotonRequestAnnotationsByListOfIDsEvent:
                    ResponseToRequestAnnotationsListOfIDsEvent(data);
                    break;
                case GlobalVariables.PhotonResponseToRequestAnnotationsByListOfIDsEvent:
                    ProcessResponseToRequestAnnotationsListOfIDsEvent(data);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sends request to master client for a list of annotations on current vis, should happen on awake.
        /// Data = { photonView.ViewID };
        /// Raises = GlobalVariables.PhotonRequestAnnotationsListOfIDsEvent
        /// Reciver = ReceiverGroup.MasterClient
        /// </summary>
        public void RequestAnnotationsListOfIDs()
        {
            if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat(GlobalVariables.cEvent + "Masterclient ~ not calling: {0}, {1}My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestAnnotationsListOfIDs()", "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                return; 
            }

            isWaitingForListOfAnnotationIDs = true;

            Debug.LogFormat(GlobalVariables.cEvent + "Client ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestAnnotationsListOfIDs()", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestAnnotationsListOfIDsEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestAnnotationsListOfIDsEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        /// <summary>
        ///Master client respone to request for annotaion IDs.
        /// Path 1, no annotations found
        /// Data sent = 
        /// Raises = GlobalVariables.PhotonResponseRequestAnnotationsListOfIDsEventNONE_FOUNDEvent
        /// Reciver = ReceiverGroup.Others
        /// </summary>
        private void RespondToRequestAnnotationsListOfIDs(object[] data)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            object[] content;

            if (myListOfAnnotationIDs.Count == 0)
            {
                Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Masterclient ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonRequestAnnotationsListOfIDsEvent, "No annotations found", "Others", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonResponseRequestAnnotationsListOfIDsEventNONE_FOUNDEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                content = new object[] { photonView.ViewID };
                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonResponseRequestAnnotationsListOfIDsEventNONE_FOUNDEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cEvent + "Recived Code {0}: Masterclient ~ {1}, Receivers: {2}, My Name: {3}, I am the Master Client: {4}, Server Time: {5}, Sending Event Code: {6}{7}{8}{9}." + GlobalVariables.endColor + " {10}: {11} -> {12} -> {13}", GlobalVariables.PhotonRequestAnnotationsListOfIDsEvent, "Annotations found", "Others", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonResponseToRequestAnnotationsByListOfIDsEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                content = new object[] { photonView.ViewID, HelperFunctions.IntListToString(myListOfAnnotationIDs, System.Reflection.MethodBase.GetCurrentMethod()) };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonResponseToRequestAnnotationsByListOfIDsEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }


        }

        public void ProcessResponseToRequestAnnotationsListOfIDsNONE_FoundEvent()
        {
            if (isWaitingForListOfAnnotationIDs)
            {
                Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, Client ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.PhotonResponseRequestAnnotationsListOfIDsEventNONE_FOUNDEvent, "No annotations were sent back", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                isWaitingForListOfAnnotationIDs = false;

                return;
            }

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, Client ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.PhotonResponseRequestAnnotationsListOfIDsEventNONE_FOUNDEvent, "I am not waiting for any IDs", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            isWaitingForListOfAnnotationIDs = false;
        }


        /// <summary>
        ///Processes the Master client respone to request for annotaion IDs.
        /// Expected Data = { photonView.ViewID, List of string listOfIDs};
        /// If the lists don't match we will request the missing annotaitons as serializable annotations
        /// Reqeust Data = Data = { photonView.ViewID, List of int missingListOfIDs};
        /// </summary>
        private void ProcessResponseToRequestAnnotationsListOfIDs(object[] data)
        {

        }

        /// <summary>
        ///Processes the Master client respone to request for annotaion IDs.
        /// Expected Data = { photonView.ViewID, List of int listOfIDs};
        /// Sent Data = { photonView.ViewID, List of string serializeableAnnotations as JSONs};
        /// </summary>
        private void ResponseToRequestAnnotationsListOfIDsEvent(object[] data)
        {


        }

        /// <summary>
        ///Processes the Master client respone to request for annotaion json's by IDs.
        /// Expected Data = { photonView.ViewID, List of string serializeableAnnotations as JSONs};
        /// </summary>
        private void ProcessResponseToRequestAnnotationsListOfIDsEvent(object[] data)
        {

        }

        private void makeAnnotation(string JsonOfSerializedAnnotation)
        {

        }
    }
}
