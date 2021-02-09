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
    // This class will store the information relvevent to annotations in this sytem
    //as well as handing converting itself to and from the annotation serizliation class

    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class Annotation : MonoBehaviourPun
    {
        public string myVisXAxis;
        public string myVisYAxis;
        public string myVisZAxis;
        public string myTextContent;
        public int myUniqueAnnotationNumber;

        private typesOfAnnotations _myAnnotationType;
        private bool wasObjectSetup = false;

        /// <summary>
        /// Setting this will force an update and a call to the master client for all content
        /// </summary>
        public typesOfAnnotations myAnnotationType { 
            get
            {
                return _myAnnotationType;
            }
            set
            {
                _myAnnotationType = value;
                if (!wasObjectSetup)
                {
                    _setAnnotationObject();
                }

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

        public Annotation(SerializeableAnnotation serializeableAnnotation){
            setUpFromSerializeableAnnotation(serializeableAnnotation);
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

            Debug.LogFormat(GlobalVariables.cRegister + "Annotation registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat(GlobalVariables.cRegister + "Annotation Requesting content from master.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                RequestContentFromMaster();
            }
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

            //set axis to that parent
            _setAxisNames();
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

            switch (eventCode)
            {
                case GlobalVariables.RequestEventAnnotationContent:
                    SendContentFromMaster(data);
                    break;
                case GlobalVariables.RespondEventWithContent:
                    ProcessRecivedContent(data);
                    break;
                default:
                    break;
            }

        }

        #region Content Updates
        private void RequestContentFromMaster()
        {

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
        public void SendContentFromMaster(object[] data)
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, MasterClient ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Raising Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestEventAnnotationCreation, "", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RespondEventWithContent, "Others", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, this.getJSONSerializedAnnotationString() };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

                PhotonNetwork.RaiseEvent(GlobalVariables.RespondEventWithContent, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        private void ProcessRecivedContent(object[] data)
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, Client ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestEventAnnotationCreation, "Loading content", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            String jsonSerializedAnnotation = (string)data[1];
            setUpFromSerializeableAnnotation(jsonSerializedAnnotation);
        }

        #endregion #Content Updates
        #endregion Events
        private void _setAnnotationObject()
        {
            wasObjectSetup = true;

            GameObject prefabGameObject;
            //this will add the visual representation to the annotation
            switch (myAnnotationType)
            {
                case typesOfAnnotations.TEST_TRACKER:
                    prefabGameObject = Resources.Load<GameObject>("Tracker");
                    break;
                default:
                    Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Loading this annotation type is not supported or the type is null.", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    return;
            }

            prefabGameObject = Instantiate(prefabGameObject, Vector3.zero, Quaternion.identity);

            prefabGameObject.transform.parent = this.transform;
            prefabGameObject.transform.localPosition = Vector3.zero;
            prefabGameObject.transform.localRotation = Quaternion.identity;


            if (myAnnotationType == typesOfAnnotations.TEST_TRACKER)
            {
                HelperFunctions.randomizeAttributes(this.gameObject);
            }
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

        public string getJSONSerializedAnnotationString()
        {
            return JsonUtility.ToJson(getSerializeableAnnotation(), GlobalVariables.JSONPrettyPrint);
        }

        public SerializeableAnnotation getSerializeableAnnotation()
        {
            SerializeableAnnotation serializeableAnnotation = new SerializeableAnnotation();

            serializeableAnnotation.myLocalXPosition = this.transform.localPosition.x;
            serializeableAnnotation.myLocalYPosition = this.transform.localPosition.y;
            serializeableAnnotation.myLocalZPosition = this.transform.localPosition.z;

            serializeableAnnotation.myLocalXRotation = this.transform.localRotation.x;
            serializeableAnnotation.myLocalYRotation = this.transform.localRotation.y;
            serializeableAnnotation.myLocalZRotation = this.transform.localRotation.z;
            serializeableAnnotation.myLocalWRotation = this.transform.localRotation.w;

            serializeableAnnotation.myLocalScaleX = this.transform.localScale.x;
            serializeableAnnotation.myLocalScaleY = this.transform.localScale.y;
            serializeableAnnotation.myLocalScaleZ = this.transform.localScale.z;

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
            Debug.LogFormat(GlobalVariables.cFileOperations + "{0}{1}" + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "Loading annotation from file", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            this.gameObject.tag = GlobalVariables.annotationTag;

            //Now we set up the annotation componenet
            isDeleted = serializeableAnnotation.isDeleted;
            myVisXAxis = serializeableAnnotation.myVisXAxis;
            myVisYAxis = serializeableAnnotation.myVisYAxis;
            myVisZAxis = serializeableAnnotation.myVisZAxis;
            myTextContent = serializeableAnnotation.myTextContent;
            myUniqueAnnotationNumber = serializeableAnnotation.myAnnotationNumber;

            myAnnotationType = (typesOfAnnotations)Enum.Parse(typeof(typesOfAnnotations), serializeableAnnotation.myAnnotationType, true);

            this.gameObject.transform.parent = myAnnotationCollectionParent.transform;

            _setAnnotationObject();

            Vector3 localPosition = new Vector3(serializeableAnnotation.myLocalXPosition, serializeableAnnotation.myLocalYPosition, serializeableAnnotation.myLocalZPosition);
            this.gameObject.transform.localPosition = localPosition;

            Quaternion localRotation = new Quaternion(serializeableAnnotation.myLocalXRotation, serializeableAnnotation.myLocalYRotation, serializeableAnnotation.myLocalZRotation, serializeableAnnotation.myLocalWRotation);
            this.gameObject.transform.localRotation = localRotation;

            Vector3 localScale = new Vector3(serializeableAnnotation.myLocalScaleX, serializeableAnnotation.myLocalScaleY, serializeableAnnotation.myLocalScaleZ);
            this.gameObject.transform.localScale = localScale;

            return this;
        }
    }
}

