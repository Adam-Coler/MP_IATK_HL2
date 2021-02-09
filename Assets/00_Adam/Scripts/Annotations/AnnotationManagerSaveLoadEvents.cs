using System.Collections.Generic;
using UnityEngine;
using System.IO;
using IATK;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using Photon.Compression;
using Photon.Utilities;

using System.Runtime.Serialization.Formatters.Binary;
using System;


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

        private Dictionary<int, string> annotationsInScene = new Dictionary<int, string> { };

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
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} updated. Name: " + PhotonNetwork.NickName + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void UpdatedViewRequested(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} update requested." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //Save annotations handled by other class
            //Delete annotations without marking delete but as safe
        }


        #region Events

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
                case GlobalVariables.RequestEventAnnotationCreation:
                    RespondToRequestAnnotationCreation(data);
                    break;
                case GlobalVariables.RequestEventAnnotationRemoval:
                    RespondToRequestAnnotationRemoval(data);
                    break;
                default:
                    break;
            }
        }

        #region AnnotationCreation

        public void RequestAnnotationCreationTestTracker()
        {
            RequestAnnotationCreation(Annotation.typesOfAnnotations.TEST_TRACKER);
        }

        /// <summary>
        /// Sends request to master client to room instantiate an annotation object
        /// Data Sent = object[] { photonView.ViewID, annotationType.ToString() };
        /// Raises = GlobalVariables.RequestEventAnnotationCreation
        /// Reciver = ReceiverGroup.MasterClient
        /// </summary>
        public void RequestAnnotationCreation(Annotation.typesOfAnnotations annotationType)
        {
            GameObject annotationCollection;
            if (!PhotonNetwork.IsConnected || !HelperFunctions.FindGameObjectOrMakeOneWithTag( "AnnotationCollection", out annotationCollection, false, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "You are offline or there is no annotationcollection.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestAnnotationCreation()", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestEventAnnotationCreation, "", ", annotationType: ", annotationType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, HelperFunctions.SerializeToByteArray(annotationType, System.Reflection.MethodBase.GetCurrentMethod()) };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestEventAnnotationCreation, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        /// <summary>
        /// Creates a new annotation using PhotonNetwork.InstantiateRoomObject
        /// Reciver = ReceiverGroup.MasterClient
        /// </summary>
        private void RespondToRequestAnnotationCreation(object[] data)
        {
            Annotation.typesOfAnnotations annotationType = HelperFunctions.DeserializeFromByteArray<Annotation.typesOfAnnotations>((Byte[])data[1], System.Reflection.MethodBase.GetCurrentMethod());

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, MasterClient ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestEventAnnotationCreation, "Creating an annotation.", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "Annotation Type: ", annotationType.ToString(), Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            CreateAnnotation(annotationType);
        }

        private void CreateAnnotation(Annotation.typesOfAnnotations annotationType)
        {
            GameObject genericAnnotationObj;

            if (PhotonNetwork.IsConnected)
            {
                genericAnnotationObj = PhotonNetwork.InstantiateRoomObject("GenericAnnotation", Vector3.zero, Quaternion.identity);
            }
            else
            {
                GameObject prefab = (GameObject)Resources.Load("GenericAnnotation");
                genericAnnotationObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }
                
            Annotation annotation;
            if(HelperFunctions.GetComponentInChild<Annotation>(out annotation, genericAnnotationObj, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                annotation.myAnnotationType = annotationType;
                annotation.SendContentFromMaster(new object[] { });
            }

        }
        #endregion Annotation Creation

        #region AnnotaitonRemoval

        /// <summary>
        /// Sends request to master client to remove and not mark deleted on all annotaitons
        /// Data Sent = object[] { photonView.ViewID};
        /// Raises = GlobalVariables.RequestEventAnnotationRemoval
        /// Reciver = ReceiverGroup.MasterClient
        /// </summary>
        public void RequestAnnotationRemoval()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogFormat(GlobalVariables.cAlert + "Annotations deleted offline{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            }

            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestAnnotationDeletionAll()", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestEventAnnotationRemoval, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestEventAnnotationRemoval, content, raiseEventOptions, GlobalVariables.sendOptions);
        }


        private void RespondToRequestAnnotationRemoval(object[] data)
        {

            var annotations = GameObject.FindGameObjectsWithTag("Annotation");

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, MasterClient ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestEventAnnotationRemoval, "removing all annotations.", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "Annotation Count: ", annotations.Length, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            foreach (GameObject annotation in annotations)
            {

                Debug.LogFormat(GlobalVariables.cOnDestory + "Destorying {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", annotation.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                if (PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.Destroy(annotation);
                }
                else
                {
                    Destroy(annotation);
                }
            }
        }

        #endregion AnnotaitonRemoval

        #endregion Events
    }
}
