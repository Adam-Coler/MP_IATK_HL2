﻿using System.Collections.Generic;
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
    // This class will store the information relvevent to annotations in this sytem
    //as well as handing converting itself to and from the annotation serizliation class
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class Annotation : MonoBehaviourPun
    {
        #region Variables

        public string myVisXAxis;
        public string myVisYAxis;
        public string myVisZAxis;
        public string myTextContent;
        public int myUniqueAnnotationNumber;

        public GameObject myObjectRepresentation;
        public Component myObjectComponenet;

        private typesOfAnnotations _myAnnotationType;
        private bool wasObjectSetup = false;
        public bool isFirstUpdate = true;


        private Vector3 recivedRealtiveScale;
        private Vector3 myRelativeScale { 
            get
            {
                Transform transform = this.transform;
                GameObject vis;
                if (!HelperFunctions.FindGameObjectOrMakeOneWithTag("Vis", out vis, false, System.Reflection.MethodBase.GetCurrentMethod())){ return Vector3.one; }


                float outputX = vis.transform.localScale.x / transform.localScale.x;
                float outputY = vis.transform.localScale.y / transform.localScale.y;
                float outputZ = vis.transform.localScale.z / transform.localScale.z;

                return (new Vector3 (outputX, outputY, outputZ ));
            }
            set
            {
                Transform transform = this.transform;
                GameObject vis;
                if (!HelperFunctions.FindGameObjectOrMakeOneWithTag("Vis", out vis, false, System.Reflection.MethodBase.GetCurrentMethod())) { return; }

                float XScale = vis.transform.localScale.x * value.x;
                float YScale = vis.transform.localScale.y * value.y;
                float ZScale = vis.transform.localScale.z * value.z;

                transform.localScale = new Vector3 (XScale, YScale, ZScale);
            }
        }

        public typesOfAnnotations myAnnotationType { 
            get
            {
                return _myAnnotationType;
            }
            set
            {
                _myAnnotationType = value;
            }
        }

        public enum typesOfAnnotations {
            TEST_TRACKER,
            LINERENDER
        }

        private bool isDeleted = false;
        private bool isWaitingForContentFromMaster = false;

        private GameObject myVisParent;
        private GameObject myAnnotationCollectionParent;

        #endregion Varbiales

        #region Setup
        public Annotation(SerializeableAnnotation serializeableAnnotation){
            setUpFromSerializeableAnnotation(serializeableAnnotation);
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

        private void Awake()
        {
            Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "New annotation loaded", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //attach to or make parents
            if (myVisParent == null || myAnnotationCollectionParent == null) { setupParentObjects(); }

            HelperFunctions.SetObjectLocalTransformToZero(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());

            //set axis to that parent
            _setAxisNames();

            RequestContentFromMaster();

            isFirstUpdate = true;
        }

        private void _setAxisNames()
        {
            VisualizationEvent_Calls myParentsVisRPCClass = myVisParent.GetComponent<VisualizationEvent_Calls>();

            if (myParentsVisRPCClass == null)
            {
                myVisXAxis = "Fake X Axis Title";
                myVisYAxis = "Fake Y Axis Title";
                myVisZAxis = "Fake Z Axis Title";
            }
            else
            {
                myVisXAxis = myParentsVisRPCClass.xDimension;
                myVisYAxis = myParentsVisRPCClass.yDimension;
                myVisZAxis = myParentsVisRPCClass.zDimension;
            }
        }

        private void setupParentObjects()
        {
            HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out myVisParent, true, System.Reflection.MethodBase.GetCurrentMethod());
            if (HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.annotationCollectionTag, out myAnnotationCollectionParent, true, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                this.transform.parent = myAnnotationCollectionParent.transform;
            }
            else
            {
                //destroy it over hte network
            }
        }

        public void _setAnnotationObject()
        {
            if (wasObjectSetup) { return; }

            wasObjectSetup = true;

            GameObject prefabGameObject;
            //this will add the visual representation to the annotation
            switch (myAnnotationType)
            {
                case typesOfAnnotations.TEST_TRACKER:
                    prefabGameObject = Resources.Load<GameObject>("Tracker");
                    break;
                case typesOfAnnotations.LINERENDER:
                    prefabGameObject = Resources.Load<GameObject>("LineDrawing");
                    break;
                default:
                    Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Loading this annotation type is not supported or the type is null.", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    wasObjectSetup = false;
                    return;
            }

            prefabGameObject = Instantiate(prefabGameObject, Vector3.zero, Quaternion.identity);

            prefabGameObject.transform.parent = this.transform;
            prefabGameObject.transform.localPosition = Vector3.zero;
            prefabGameObject.transform.localRotation = Quaternion.identity;
            prefabGameObject.transform.localScale = Vector3.one;

            if (myAnnotationType == typesOfAnnotations.TEST_TRACKER && this.gameObject.transform.localPosition == Vector3.zero)
            {
                HelperFunctions.randomizeAttributes(this.gameObject);
            }

            myObjectRepresentation = prefabGameObject;

            setupLineRenderListeners();
        }
        #endregion Setup

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

            switch (eventCode)
            {
                case GlobalVariables.RequestEventAnnotationContent:
                    SendContentFromMaster();
                    break;
                case GlobalVariables.RespondEventWithContent:
                    ProcessRecivedContent(data);
                    break;
                case GlobalVariables.RequestAddPointEvent:
                    addPoint(data);
                    break;
                default:
                    break;
            }

        }

        #region Content Updates
        private void RequestContentFromMaster()
        {
            isWaitingForContentFromMaster = true;
            //request content
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat(GlobalVariables.cEvent + "Client ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestEventAnnotationContent", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestAnnotationsListOfIDsEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                object[] content = new object[] { photonView.ViewID };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

                PhotonNetwork.RaiseEvent(GlobalVariables.RequestEventAnnotationContent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
        }

        /// <summary>
        /// Sends request to master client to room instantiate an annotation object
        /// Data Sent = object[]  { photonView.ViewID, this.getJSONSerializedAnnotationString() };
        /// Raises = GlobalVariables.RequestEventAnnotationCreation
        /// Reciver = ReceiverGroup.MasterClient
        /// </summary>
        public void SendContentFromMaster()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, MasterClient ~ Sending Content{1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Raising Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestEventAnnotationCreation, "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RespondEventWithContent, "Others", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, this.getJSONSerializedAnnotationString() };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

                PhotonNetwork.RaiseEvent(GlobalVariables.RespondEventWithContent, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        private void ProcessRecivedContent(object[] data)
        {
            if (!isWaitingForContentFromMaster) { return; }

            isWaitingForContentFromMaster = false;
            String jsonSerializedAnnotation = (string)data[1];

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, Client ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestEventAnnotationCreation, "Loading content", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", ", Content: ", jsonSerializedAnnotation, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (jsonSerializedAnnotation.Length < 2) { return; }
            setUpFromSerializeableAnnotation(jsonSerializedAnnotation);


        }

        #endregion #Content Updates



        #endregion Events

        #region LineRender

        public bool isListeningForPenEvents = false;
        private void setupLineRenderListeners()
        {

            if(myAnnotationType != typesOfAnnotations.LINERENDER) { return; }

            myObjectComponenet = myObjectRepresentation.GetComponent<PhotonLineDrawing>();

#if VIVE
            PenButtonEvents penButtonEvents;
            if (!HelperFunctions.GetComponent<PenButtonEvents>(out penButtonEvents, System.Reflection.MethodBase.GetCurrentMethod())) { return; }

            penButtonEvents.penTriggerPress.AddListener(onPenTriggerPress);
            penButtonEvents.penTriggerPressedLocation.AddListener(sendAddPointEvent);

            Debug.LogFormat(GlobalVariables.cRegister + "PenEvent listeners registered, Pen Events Name: {0}, Component attached to {1} parented in {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", penButtonEvents.name, myObjectComponenet.name, myObjectComponenet.transform.parent.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

        isListeningForPenEvents = true;
#endif
        }

    private void onPenTriggerPress(bool pressed)
        {
            if (!pressed)
            {
                PenButtonEvents penButtonEvents;
                if (!HelperFunctions.GetComponent<PenButtonEvents>(out penButtonEvents, System.Reflection.MethodBase.GetCurrentMethod())) { return; }

                penButtonEvents.penTriggerPress.RemoveListener(onPenTriggerPress);
                penButtonEvents.penTriggerPressedLocation.RemoveListener(sendAddPointEvent);

                Debug.LogFormat(GlobalVariables.cRegister + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "PenEvent listeners removed", " Pen Events Name:", penButtonEvents.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                isListeningForPenEvents = false;
            }
        }


        Vector3 lastPoint;
        public void sendAddPointEvent(Vector3 point)
        {
            float distPosition = Vector3.Distance(point, lastPoint);

            bool isDistanceMeaningful = distPosition > .01f;


            if (!isDistanceMeaningful)
            {
                return;
            }

            lastPoint = point;

            point = this.transform.InverseTransformPoint(point);
            //point = HelperFunctions.PRA(point);

            string pointString = JsonUtility.ToJson(point);

            Debug.LogFormat(GlobalVariables.cEvent + "{0}, Any ~ Sending point, MyViewID: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Raising Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", " ViewID: ", photonView.ViewID, PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RespondEventWithContent, "all", " Point: ", pointString, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, pointString};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestAddPointEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public void addPoint(object[] data)
        {

            string pointstring = (string)data[1];
            Vector3 point = JsonUtility.FromJson<Vector3>(pointstring);

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, Any ~ Adding point, MyView ID: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Raising Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestEventAnnotationCreation, photonView.ViewID, PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RespondEventWithContent, "all", " Point: ", pointstring, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            addPoint(point);
        }
        public void addPoint(Vector3 newPoint)
        {

            if (isFirstUpdate)
            {
                //Vector3 pointToAdd = this.transform.InverseTransformPoint(newPoint);
                this.transform.localPosition = newPoint;
                isFirstUpdate = false;
                
            }

            //newPoint = this.transform.InverseTransformPoint(newPoint);
            //newPoint = HelperFunctions.PRA(this.gameObject);
            var tmpComponenet = (PhotonLineDrawing)myObjectComponenet;
            tmpComponenet.addPoint(newPoint);
        }

#endregion LineRender


        #region serialization

        public string getJSONSerializedAnnotationString()
        {
            return JsonUtility.ToJson(getSerializeableAnnotation(), GlobalVariables.JSONPrettyPrint);
        }

        public SerializeableAnnotation getSerializeableAnnotation()
        {
            SerializeableAnnotation serializeableAnnotation = new SerializeableAnnotation();

            serializeableAnnotation.myLocalRotation = this.transform.localRotation;
            serializeableAnnotation.myLocalPosition = this.transform.localPosition;
            serializeableAnnotation.myRelativeScale = this.myRelativeScale;

            serializeableAnnotation.isDeleted = isDeleted;

            serializeableAnnotation.myVisXAxis = myVisXAxis;
            serializeableAnnotation.myVisYAxis = myVisYAxis;
            serializeableAnnotation.myVisZAxis = myVisZAxis;
            serializeableAnnotation.myTextContent = myTextContent;
            serializeableAnnotation.myAnnotationNumber = myUniqueAnnotationNumber;

            serializeableAnnotation.myAnnotationType = myAnnotationType.ToString();

            return serializeableAnnotation;
        }

        public Annotation setUpFromSerializeableAnnotation(string JSONSerializedAnnotation)
        {
            SerializeableAnnotation serializeableAnnotation = JsonUtility.FromJson<SerializeableAnnotation>(JSONSerializedAnnotation);

            setUpFromSerializeableAnnotation(serializeableAnnotation);

            return this;
        }
        public Annotation setUpFromSerializeableAnnotation(SerializeableAnnotation serializeableAnnotation)
        {
            Debug.LogFormat(GlobalVariables.cFileOperations + "{0}{1}" + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "Loading annotation", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            this.gameObject.tag = GlobalVariables.annotationTag;

            //Now we set up the annotation componenet
            isDeleted = serializeableAnnotation.isDeleted;
            myTextContent = serializeableAnnotation.myTextContent;
            myUniqueAnnotationNumber = serializeableAnnotation.myAnnotationNumber;

            myAnnotationType = (typesOfAnnotations)Enum.Parse(typeof(typesOfAnnotations), serializeableAnnotation.myAnnotationType, true);

            this.gameObject.transform.parent = myAnnotationCollectionParent.transform;
            this.gameObject.transform.localPosition = serializeableAnnotation.myLocalPosition;
            this.gameObject.transform.localRotation = serializeableAnnotation.myLocalRotation;
            this.gameObject.transform.localScale = serializeableAnnotation.myLocalScale;
            this.myRelativeScale = serializeableAnnotation.myRelativeScale;

            _setAnnotationObject();
            return this;
        }

        #endregion serialization

    }
}

