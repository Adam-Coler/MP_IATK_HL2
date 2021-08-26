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
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;


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
        public string myVisSizeDimension;
        public string myVisColorDimension;
        public string myTextContent;
        public int myUniqueAnnotationNumber;

        public GameObject myObjectRepresentation;
        public Component myObjectComponenet;

        public typesOfAnnotations _myAnnotationType;

        private bool wasObjectSetup = false;
        public Vector3[] lineRenderPoints;
        public bool wasLoaded;

        public float myCreationTime;

        public MeanPlane.axisSelection axisSelection;
        public MeanPlane.summeryValueType summeryValueType;

        public List<float> myStartTimesofMoves;
        public List<float> myEndTimesofMoves;
        public List<Vector3> myLocations;
        public List<Quaternion> myRotations;
        public List<Vector3> myRelativeScales;

        private bool isDeleted = false;
        private bool isWaitingForContentFromMaster = false;

        private GameObject myVisParent;
        private GameObject myAnnotationCollectionParent;

        private Vector3 recivedRealtiveScale;
        private Vector3 myRelativeScale { 
            get
            {
                //Transform transform = this.transform;
                //GameObject vis;
                //if (!HelperFunctions.FindGameObjectOrMakeOneWithTag("Vis", out vis, false, System.Reflection.MethodBase.GetCurrentMethod())){ return Vector3.one; }


                //float outputX = vis.transform.localScale.x / transform.localScale.x;
                //float outputY = vis.transform.localScale.y / transform.localScale.y;
                //float outputZ = vis.transform.localScale.z / transform.localScale.z;

                //return (new Vector3 (outputX, outputY, outputZ ));

                return transform.localScale;
            }
            set
            {
                //Transform transform = this.transform;
                //GameObject vis;
                //if (!HelperFunctions.FindGameObjectOrMakeOneWithTag("Vis", out vis, false, System.Reflection.MethodBase.GetCurrentMethod())) { return; }

                //float XScale = vis.transform.localScale.x * value.x;
                //float YScale = vis.transform.localScale.y * value.y;
                //float ZScale = vis.transform.localScale.z * value.z;

                //transform.localScale = new Vector3 (XScale, YScale, ZScale);
                transform.localScale = value;
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
            LINERENDER,
            HIGHLIGHTCUBE,
            HIGHLIGHTSPHERE,
            DETAILSONDEMAND,
            TEXT,
            CENTRALITY
        }

        #endregion Varbiales

        #region Setup
        public Annotation(SerializeableAnnotation serializeableAnnotation){
            SetUpFromSerializeableAnnotation(serializeableAnnotation);
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
            if (myVisParent == null || myAnnotationCollectionParent == null) { _setupParentObjects(); }

            HelperFunctions.SetObjectLocalTransformToZero(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());
            //TODO

            //set axis to that parent
            _setAxisNames();

            RequestContentFromMaster();
        }

        public void ManipulationStarted() {
            //add the start time
            myStartTimesofMoves.Add(HelperFunctions.GetTime());
        }

        public void ManipulationEnded()
        {
            //Record all of the states to the arrays
            myEndTimesofMoves.Add(HelperFunctions.GetTime());
            myLocations.Add(this.transform.localPosition);
            myRotations.Add(this.transform.localRotation);
            myRelativeScales.Add(this.myRelativeScale);
        }

        private void _setAxisNames()
        {
            VisualizationEvent_Calls myParentsVisRPCClass = myVisParent.GetComponent<VisualizationEvent_Calls>();

            if (myParentsVisRPCClass == null)
            {
                myVisXAxis = "Fake X Axis Title";
                myVisYAxis = "Fake Y Axis Title";
                myVisZAxis = "Fake Z Axis Title";
                myVisColorDimension = "Fake Color Dimension";
                myVisSizeDimension = "Fake Size Dimension";
            }
            else
            {
                myVisXAxis = myParentsVisRPCClass.xDimension;
                myVisYAxis = myParentsVisRPCClass.yDimension;
                myVisZAxis = myParentsVisRPCClass.zDimension;
            }

            myVisSizeDimension = "none";
            if (myVisSizeDimension == "" || myVisSizeDimension == null)
            {
                myVisSizeDimension = myParentsVisRPCClass.sizeDimension;
            }

            myVisColorDimension = "none";
            if (myVisColorDimension == "" || myVisColorDimension == null)
            {
                myVisColorDimension = myParentsVisRPCClass.colourDimension;
            }
        }

        private void _setupParentObjects()
        {
            HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out myVisParent, true, System.Reflection.MethodBase.GetCurrentMethod());
            if (HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.annotationCollectionTag, out myAnnotationCollectionParent, true, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                this.transform.parent = myAnnotationCollectionParent.transform;
            }
            else
            {
                //destroy it over the network
            }
        }

        public void SetAnnotationObject()
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Setting up new annotaiton, ", "From File: ", wasLoaded, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (wasObjectSetup) { return; }

            wasObjectSetup = true;

            GameObject prefabGameObject;
            //this will add the visual representation to the annotation
            switch (myAnnotationType)
            {
                case typesOfAnnotations.TEST_TRACKER:
                    prefabGameObject = Resources.Load<GameObject>("Tracker");
                    prefabGameObject = Instantiate(prefabGameObject, Vector3.zero, Quaternion.identity);
                    break;
                case typesOfAnnotations.LINERENDER:
                    prefabGameObject = Resources.Load<GameObject>("LineDrawing");
                    prefabGameObject = Instantiate(prefabGameObject, Vector3.zero, Quaternion.identity);
                    break;
                case typesOfAnnotations.HIGHLIGHTCUBE:
                    prefabGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                case typesOfAnnotations.HIGHLIGHTSPHERE:
                    prefabGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                case typesOfAnnotations.DETAILSONDEMAND:
                    prefabGameObject = Resources.Load<GameObject>("DetailsOnDemand");
                    prefabGameObject = Instantiate(prefabGameObject, Vector3.zero, Quaternion.identity);
                    break;
                case typesOfAnnotations.TEXT:
                    prefabGameObject = Resources.Load<GameObject>("TextAnnotation");
                    prefabGameObject = Instantiate(prefabGameObject, Vector3.zero, Quaternion.identity);
                    break;
                case typesOfAnnotations.CENTRALITY:
                    prefabGameObject = Resources.Load<GameObject>("MeanMedianPlane");
                    prefabGameObject = Instantiate(prefabGameObject, Vector3.zero, Quaternion.identity);
                    break;
                default:
                    Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Loading this annotation type is not supported or the type is null.", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    wasObjectSetup = false;
                    return;
            }

            prefabGameObject.transform.parent = this.transform;
            prefabGameObject.transform.localPosition = Vector3.zero;
            prefabGameObject.transform.localRotation = Quaternion.identity;
            prefabGameObject.transform.localScale = Vector3.one;

            if (myAnnotationType == typesOfAnnotations.TEST_TRACKER && this.gameObject.transform.localPosition == Vector3.zero)
            {
                HelperFunctions.randomizeAttributes(this.gameObject);
            }

            myObjectRepresentation = prefabGameObject;

            _setupLineRender();
            _setupHighlight();
            _setupDetailsOnDemand();
            _setupText();
            _setupCentrality();

            ManipulationControls manipulationControls;
            if (HelperFunctions.GetComponent<ManipulationControls>(out manipulationControls, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                manipulationControls.enabled = true;
            }

            MoveToTopCorner moveToTopCorner = this.GetComponentInChildren<MoveToTopCorner>();
            if (moveToTopCorner != null)
            {
                moveToTopCorner.MoveToTop();
            }

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

            //Debug.Log("reciving event: " + eventCode);

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
                    AddPoint(data);
                    break;
                case GlobalVariables.RequestTextUpdate:
                    UpdateText(data);
                    break;
                case GlobalVariables.PhotonRequestAnnotationsDeleteOneEvent:
                    RespondToRequestDelete();
                    break;
                case GlobalVariables.RequestCentralityUpdate:
                    RespondToCentralityUpdate(data);
                    break;
                default:
                    break;
            }

        }

        public void RequestDelete()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "PhotonRequestAnnotationsDeleteOneEvent", "Master", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestAnnotationsDeleteOneEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

            PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestAnnotationsDeleteOneEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        private void RespondToRequestDelete()
        {
            this.isDeleted = true;

            if (PhotonNetwork.IsMasterClient)
            {
                //save all annoatations
                //need to not overwrite stuff anymore
                //save to deleted folder?
                Debug.LogFormat(GlobalVariables.cOnDestory + "Deleting: {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", gameObject.name, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                AnnotationManagerSaveLoadEvents.Instance.saveAnnotations();

                PhotonNetwork.Destroy(this.gameObject);

            }

            if (!PhotonNetwork.IsConnected)
            {
                Destroy(this.gameObject);
            }
        }

        #region Content Updates
        private void RequestContentFromMaster()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                isWaitingForContentFromMaster = true;

                Debug.LogFormat(GlobalVariables.cEvent + "Client ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestEventAnnotationContent", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.PhotonRequestAnnotationsListOfIDsEvent, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                object[] content = new object[] { photonView.ViewID };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

                PhotonNetwork.RaiseEvent(GlobalVariables.RequestEventAnnotationContent, content, raiseEventOptions, GlobalVariables.sendOptions);

                PhotonNetwork.SendAllOutgoingCommands();
            } else
            {
                ManipulationStarted();
                ManipulationEnded();
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

            object[] content = new object[] { photonView.ViewID, this.GetJSONSerializedAnnotationString() };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

                PhotonNetwork.RaiseEvent(GlobalVariables.RespondEventWithContent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        private void ProcessRecivedContent(object[] data)
        {
            if (!isWaitingForContentFromMaster) { return; }

            isWaitingForContentFromMaster = false;

            String jsonSerializedAnnotation = (string)data[1];

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, Client ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestEventAnnotationCreation, "Loading content", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", ", Content: ", jsonSerializedAnnotation, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (jsonSerializedAnnotation.Length < 2) { return; }
            SetUpFromSerializeableAnnotation(jsonSerializedAnnotation);

            ManipulationStarted();
            ManipulationEnded();
        }

        #endregion #Content Updates

        #endregion Events

        #region LineRender

        public bool isListeningForPenEvents = false;
        private void _setupLineRender()
        {

            Debug.LogFormat(GlobalVariables.cAlert + "_setupLineRender(): wasloaded: {0}, Type: {1}, Islistening for Pen events: {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", wasLoaded, myAnnotationType, isListeningForPenEvents, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (myAnnotationType != typesOfAnnotations.LINERENDER) { return; }

            myObjectComponenet = myObjectRepresentation.GetComponent<PhotonLineDrawing>();

            if (wasLoaded)
            {
                Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Adding points to loaded line annotation", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                var tmpComponenet = (PhotonLineDrawing)myObjectComponenet;
                tmpComponenet.AddPoints(lineRenderPoints);

                isListeningForPenEvents = false;
                tmpComponenet.bakeMesh();

                return;
            } 
            else 
            {
#if VIVE

                PenButtonEvents penButtonEvents;
                if (!HelperFunctions.GetComponent<PenButtonEvents>(out penButtonEvents, System.Reflection.MethodBase.GetCurrentMethod())) { return; }

                penButtonEvents.penTriggerPress.AddListener(_onPenTriggerPress);
                penButtonEvents.penTriggerPressedLocation.AddListener(_sendAddPointEvent);

                Debug.LogFormat(GlobalVariables.cRegister + "PenEvent listeners registered, Pen Events Name: {0}, Component attached to {1} parented in {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", penButtonEvents.name, myObjectComponenet.name, myObjectComponenet.transform.parent.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                isListeningForPenEvents = true;
#endif
                return;

            }

            Debug.LogFormat(GlobalVariables.cError + "_setupLineRender failed, Is loaded: {0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", wasLoaded, "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void _onPenTriggerPress(bool pressed)
        {
            if (!pressed)
            {
                PenButtonEvents penButtonEvents;
                if (!HelperFunctions.GetComponent<PenButtonEvents>(out penButtonEvents, System.Reflection.MethodBase.GetCurrentMethod())) { return; }

                penButtonEvents.penTriggerPress.RemoveListener(_onPenTriggerPress);
                penButtonEvents.penTriggerPressedLocation.RemoveListener(_sendAddPointEvent);

                Debug.LogFormat(GlobalVariables.cRegister + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "PenEvent listeners removed", " Pen Events Name:", penButtonEvents.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                isListeningForPenEvents = false;

                var tmpComponenet = (PhotonLineDrawing)myObjectComponenet;
                tmpComponenet.Simplify();

                lineRenderPoints = tmpComponenet.GetPoints();
            }
        }

        public bool isFirstPoint = true;
        Vector3 lastPoint;
        /// <summary>
        /// Called when the pen is pressed, point is the world coordianate point for the pen tip
        /// </summary>
        private void _sendAddPointEvent(Vector3 point)
        {
            if (firstPoint == Vector3.one || firstPoint == Vector3.zero)
            {
                Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "FirstPoint: ", point, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                firstPoint = point;
                this.transform.localPosition = point;
            }

            point = this.transform.InverseTransformPoint(point);

            float distPosition = Vector3.Distance(point, lastPoint);

            bool isDistanceMeaningful = distPosition > .01f;

            if (!isDistanceMeaningful){return;}

            lastPoint = point;

            string pointString = JsonUtility.ToJson(point);

            Debug.LogFormat(GlobalVariables.cEvent + "{0} Any ~ Sending point, MyViewID: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Raising Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "", photonView.ViewID, PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RespondEventWithContent, "all", " Point: ", pointString, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, pointString};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestAddPointEvent, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public Vector3 firstPoint = Vector3.one;
        public void AddPoint(object[] data)
        {
            if (isWaitingForContentFromMaster) { return; }

            string pointstring = (string)data[1];
            Vector3 point = JsonUtility.FromJson<Vector3>(pointstring);

            var tmpComponenet = (PhotonLineDrawing)myObjectComponenet;

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, Any ~ Adding point, MyView ID: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Raising Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.RequestEventAnnotationCreation, photonView.ViewID, PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RespondEventWithContent, "all", " Point: ", pointstring, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            if (isFirstPoint)
            {
                Debug.LogFormat(GlobalVariables.cAlert + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "FirstPoint: ", point, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                firstPoint = point;

                this.transform.localPosition = point;

                isFirstPoint = false;
                return;
            }

            tmpComponenet.addPoint(point);
        }



        #endregion LineRender

        #region Highlights

        private void _setupHighlight()
        {
            if (myAnnotationType != typesOfAnnotations.HIGHLIGHTCUBE && myAnnotationType != typesOfAnnotations.HIGHLIGHTSPHERE) { return; }

            Material newMat = Resources.Load("TransparentYellow", typeof(Material)) as Material;
            myObjectRepresentation.GetComponent<Renderer>().material = newMat;
            myObjectRepresentation.AddComponent<GrabFeedbackTarget>();

            myObjectRepresentation.transform.localScale = new Vector3(1f, 1f, 1f);
            gameObject.AddComponent<HighlightScript>();

            MeshFilter meshFilter = myObjectRepresentation.GetComponent<MeshFilter>();
            gameObject.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;

            Destroy(myObjectRepresentation.GetComponent<Collider>());

            

            this.transform.localScale = new Vector3(.1f, .1f, .1f);

            ManipulationControls manipulationControls;
            if (HelperFunctions.GetComponent<ManipulationControls>(out manipulationControls, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                manipulationControls.enabled = true;
                manipulationControls.isUniformScale = false;
            }
        }

        #endregion Highlights

        #region Text

        private TextAnnotationManager textManager;

        private void _setupText()
        {
            if (myAnnotationType != typesOfAnnotations.TEXT) { return; }
            textManager = myObjectRepresentation.GetComponent<TextAnnotationManager>();
            if (textManager != null)
            {
                textManager.myAnnotationParent = this;

                if (myTextContent != "")
                {
                    textManager.content.text = myTextContent;
                    textManager.placeholder.text = "";
                }
                    

            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}{1}" + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "No Text Manager Found.", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

            GameObject.Destroy(this.GetComponent<ManipulationControls>());

        }

        public void UpdateText(string text)
        {
            myTextContent = text;

            Debug.LogFormat(GlobalVariables.cEvent + "{0} Any ~ Sending Text, MyViewID: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Raising Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "", photonView.ViewID, PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RespondEventWithContent, "Others", " Text: ", text, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, text };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestTextUpdate, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public void UpdateText(object[] data)
        {
            if (myAnnotationType != typesOfAnnotations.TEXT) { return; }
            string text = (string)data[1];
            this.myTextContent = text;
            textManager.updateContent(text);

            Debug.LogFormat(GlobalVariables.cEvent + "{0} Any ~ Reciving Text, MyViewID: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, reciving Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "", photonView.ViewID, PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestTextUpdate, "Others", " Text: ", text, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        #endregion Text

        #region DetailsOnDemand

        private void _setupDetailsOnDemand()
        {
            if (myAnnotationType != typesOfAnnotations.DETAILSONDEMAND) { return; }

            myObjectRepresentation.transform.localScale = new Vector3(.75f, .75f, .75f);
            GameObject.Destroy(this.GetComponent<ManipulationControls>());
        }

        #endregion DetailsOnDemand

        #region Centrality

        MeanPlane meanPlane;
        private void _setupCentrality()
        {
            if (myAnnotationType != typesOfAnnotations.CENTRALITY) { return; }

            GameObject.Destroy(this.GetComponent<ManipulationControls>());
            GameObject.Destroy(this.GetComponent<BoxCollider>());
            GameObject.Destroy(this.GetComponent<MeshCollider>());
            GameObject.Destroy(this.GetComponent<GenericTransformSync>());

            meanPlane = myObjectRepresentation.GetComponent<MeanPlane>();
            myObjectComponenet = meanPlane;

            meanPlane.currentAxis = axisSelection;
            meanPlane.currentSummeryValueType = summeryValueType;

            meanPlane.myAnnotationParent = this;

            meanPlane.SetMaterial();
            meanPlane.SetMeanPlane();
        }

        public void UpdateCentrality(MeanPlane.summeryValueType measure, MeanPlane.axisSelection axis)
        {
            summeryValueType = measure;
            axisSelection = axis;

            Debug.LogFormat(GlobalVariables.cEvent + "{0} Any ~ Sending RequestCentralityUpdate: , MyViewID: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Raising Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "", photonView.ViewID, PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestCentralityUpdate, "Others", " Text: ", summeryValueType.ToString() + " , " + axisSelection.ToString(), Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, summeryValueType.ToString(), axisSelection.ToString() };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestCentralityUpdate, content, raiseEventOptions, GlobalVariables.sendOptions);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public void RespondToCentralityUpdate(object[] data)
        {
            if (myAnnotationType != typesOfAnnotations.CENTRALITY) { return; }

            axisSelection = (MeanPlane.axisSelection)Enum.Parse(typeof(MeanPlane.axisSelection), (string)data[2], true);

            summeryValueType = (MeanPlane.summeryValueType)Enum.Parse(typeof(MeanPlane.summeryValueType), (string)data[1], true);


            Debug.LogFormat(GlobalVariables.cEvent + "{0} Any ~ Reciving RequestCentralityUpdate: , MyViewID: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Recived Code: {5}, Recipents: {6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "", photonView.ViewID, PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestCentralityUpdate, "Others", " Text: ", summeryValueType.ToString() + " , " + axisSelection.ToString(), Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            meanPlane.SetMeanPlane(summeryValueType, axisSelection);
        }

        #endregion Centrality

        #region serialization

        public string GetJSONSerializedAnnotationString()
        {
            return JsonUtility.ToJson(GetSerializeableAnnotation(), GlobalVariables.JSONPrettyPrint);
        }

        public SerializeableAnnotation GetSerializeableAnnotation()
        {
            SerializeableAnnotation serializeableAnnotation = new SerializeableAnnotation();

            serializeableAnnotation.myVisXAxis = myVisXAxis;
            serializeableAnnotation.myVisYAxis = myVisYAxis;
            serializeableAnnotation.myVisZAxis = myVisZAxis;
            serializeableAnnotation.myVisColorDimension = myVisColorDimension;
            serializeableAnnotation.myVisSizeDimension = myVisSizeDimension;

            serializeableAnnotation.myLocalPosition = this.transform.localPosition;
            serializeableAnnotation.myLocalRotation = this.transform.localRotation;
            serializeableAnnotation.myRelativeScale = this.myRelativeScale;

            serializeableAnnotation.isDeleted = isDeleted;
            serializeableAnnotation.myAnnotationType = myAnnotationType.ToString();

            serializeableAnnotation.myAnnotationNumber = myUniqueAnnotationNumber;

            VisWrapperClass visWrapperClass;
            if (HelperFunctions.GetComponent<VisWrapperClass>(out visWrapperClass, System.Reflection.MethodBase.GetCurrentMethod())) { serializeableAnnotation.myDataSource = visWrapperClass.wrapperCSVDataSource.name; }
            
            serializeableAnnotation.myTextContent = myTextContent;
            serializeableAnnotation.myLineRenderPoints = lineRenderPoints;

            serializeableAnnotation.myCreationTime = myCreationTime;

            serializeableAnnotation.myStartTimesofMoves = myStartTimesofMoves;
            serializeableAnnotation.myEndTimesofMoves = myEndTimesofMoves;
            serializeableAnnotation.myLocations = myLocations;
            serializeableAnnotation.myRotations = myRotations;
            serializeableAnnotation.myRelativeScales = myRelativeScales;

            serializeableAnnotation.wasLoaded = wasLoaded;

            serializeableAnnotation.axisSelection = axisSelection.ToString();
            serializeableAnnotation.summeryValueType = summeryValueType.ToString();

            return serializeableAnnotation;
        }

        public Annotation SetUpFromSerializeableAnnotation(string JSONSerializedAnnotation)
        {
            SerializeableAnnotation serializeableAnnotation = JsonUtility.FromJson<SerializeableAnnotation>(JSONSerializedAnnotation);

            SetUpFromSerializeableAnnotation(serializeableAnnotation);

            return this;
        }
        public Annotation SetUpFromSerializeableAnnotation(SerializeableAnnotation serializeableAnnotation)
        {
            Debug.LogFormat(GlobalVariables.cFileOperations + "{0}{1}" + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "Loading annotation", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            this.gameObject.tag = GlobalVariables.annotationTag;

            myVisXAxis = serializeableAnnotation.myVisXAxis;
            myVisYAxis = serializeableAnnotation.myVisYAxis;
            myVisZAxis = serializeableAnnotation.myVisZAxis;
            myVisColorDimension = serializeableAnnotation.myVisColorDimension;
            myVisSizeDimension = serializeableAnnotation.myVisSizeDimension;

            this.transform.localPosition = serializeableAnnotation.myLocalPosition;
            this.transform.localRotation = serializeableAnnotation.myLocalRotation;
            this.myRelativeScale = serializeableAnnotation.myRelativeScale;

            isDeleted = serializeableAnnotation.isDeleted;
            myAnnotationType = (typesOfAnnotations)Enum.Parse(typeof(typesOfAnnotations), serializeableAnnotation.myAnnotationType, true);


            axisSelection = (MeanPlane.axisSelection)Enum.Parse(typeof(MeanPlane.axisSelection), serializeableAnnotation.axisSelection, true);

            summeryValueType = (MeanPlane.summeryValueType)Enum.Parse(typeof(MeanPlane.summeryValueType), serializeableAnnotation.summeryValueType, true);

            myUniqueAnnotationNumber = serializeableAnnotation.myAnnotationNumber;

            VisWrapperClass visWrapperClass;
            if (HelperFunctions.GetComponent<VisWrapperClass>(out visWrapperClass, System.Reflection.MethodBase.GetCurrentMethod())) { serializeableAnnotation.myDataSource = visWrapperClass.wrapperCSVDataSource.name; }

            myTextContent = serializeableAnnotation.myTextContent;
            lineRenderPoints = serializeableAnnotation.myLineRenderPoints;

            myCreationTime = serializeableAnnotation.myCreationTime;

            myStartTimesofMoves = serializeableAnnotation.myStartTimesofMoves;
            myEndTimesofMoves = serializeableAnnotation.myEndTimesofMoves;
            myLocations = serializeableAnnotation.myLocations;
            myRotations = serializeableAnnotation.myRotations;
            myRelativeScales = serializeableAnnotation.myRelativeScales;

            myLocations = serializeableAnnotation.myLocations;

            wasLoaded = serializeableAnnotation.wasLoaded;

            if (wasLoaded)
            {
                this.name = "Loaded_" + myAnnotationType + "_" + myUniqueAnnotationNumber;
            }
            else
            {
                this.name = "New_" + myAnnotationType + "_" + myUniqueAnnotationNumber;
            }

            SetAnnotationObject();
            return this;
        }

        #endregion serialization

    }
}

////Non-uniform scale
//BoundsControl boundsControl =  gameObject.AddComponent<Microsoft.MixedReality.Toolkit.UI.BoundsControl.BoundsControl>();
//boundsControl.ScaleHandlesConfig.ScaleBehavior = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.HandleScaleMode.NonUniform;
//boundsControl.HandleProximityEffectConfig.ProximityEffectActive = true;