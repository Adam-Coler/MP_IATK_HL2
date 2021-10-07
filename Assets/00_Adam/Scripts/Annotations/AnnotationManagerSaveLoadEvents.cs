using System.Collections.Generic;
using UnityEngine;
using System.IO;
using IATK;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using Photon.Compression;
using Photon.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System.Runtime.Serialization.Formatters.Binary;
using System;


namespace Photon_IATK
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class AnnotationManagerSaveLoadEvents : MonoBehaviourPun
    {
        public bool isWaitingForListOfAnnotationIDs = false;
        public int annotationsCreated = 0;
        public int lastMadeAnnotationPhotonViewID;
        public static AnnotationManagerSaveLoadEvents Instance;
        public bool isFirstLoad = true;

        #region Setup and Teardown

        private void Awake()
        {

            Instance = this;

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

            saveAnnotations();
            PushAllData();
        
            //send all annoations to next client if master

        }

        private void OnApplicationQuit()
        {
            saveAnnotations();
            PushAllData();
        }

        #endregion Setup and Teardown

        #region Delegates

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} updated. Name: " + PhotonNetwork.NickName + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (propertyType == AbstractVisualisation.PropertyType.DimensionChange || propertyType == AbstractVisualisation.PropertyType.VisualisationType) {
                if (isFirstLoad)
                {
                    Invoke("loadAnnotations", 1f);
                } else
                {
                    loadAnnotations();
                }
                
            }

        }

        private void UpdatedViewRequested(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} update requested." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}, I am the MasterClient: {5}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod(), PhotonNetwork.IsMasterClient);

            //Save annotations handled by other class

            if (propertyType == AbstractVisualisation.PropertyType.DimensionChange || propertyType == AbstractVisualisation.PropertyType.VisualisationType) { saveAnnotations(); }

            //Delete annotations without marking delete but as safe
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
                Debug.LogFormat(GlobalVariables.cCommon + "I am the MasterClient: {0}, Removing annotaitons." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                RespondToRequestAnnotationRemoval();
                //RequestAnnotationRemoval();
            }
        }

        #endregion Delegates

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
                    RespondToRequestAnnotationRemoval();
                    break;
                case GlobalVariables.RequestEventAnnotationFileSystemDeletion:
                    DeleteAnnotaitonFileSystem();
                    break;
                case GlobalVariables.SendEventNewAnnotationID:
                    setAnnotationIDEvent(data);
                    break;
                case GlobalVariables.RequestSaveAnnotation:
                    _saveAnnotations(data);
                    break;
                case GlobalVariables.RequestDisableFarInteraction:
                    _requestDisableFarInteractions();
                    break;
                case GlobalVariables.RequestEnableFarInteraction:
                    _requestEnableFarInteractions();
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

        public void RequestAnnotationCreationCentralityMetricPlane()
        {
            RequestAnnotationCreation(Annotation.typesOfAnnotations.CENTRALITY);
        }

        public void RequestAnnotationCreationHighlightCube()
        {
            RequestAnnotationCreation(Annotation.typesOfAnnotations.HIGHLIGHTCUBE);
        }

        public void RequestAnnotationCreationHighlightSphere()
        {
            RequestAnnotationCreation(Annotation.typesOfAnnotations.HIGHLIGHTSPHERE);
        }

        public void RequestAnnotationCreationDetailsOnDemand()
        {
            RequestAnnotationCreation(Annotation.typesOfAnnotations.DETAILSONDEMAND);
        }

        public void RequestAnnotationCreationText()
        {
            RequestAnnotationCreation(Annotation.typesOfAnnotations.TEXT);
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
            if (!PhotonNetwork.IsConnected || !HelperFunctions.FindGameObjectOrMakeOneWithTag("AnnotationCollection", out annotationCollection, false, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "You are offline or there is no annotationcollection.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestAnnotationCreation()", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestEventAnnotationCreation, "", ", annotationType: ", annotationType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, HelperFunctions.SerializeToByteArray(annotationType, System.Reflection.MethodBase.GetCurrentMethod()), PhotonNetwork.NickName, annotationsCreated+1 };

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
                annotationsCreated++;
                genericAnnotationObj = PhotonNetwork.InstantiateRoomObject("GenericAnnotation", Vector3.zero, Quaternion.identity);
                HelperFunctions.SetObjectLocalTransformToZero(genericAnnotationObj, System.Reflection.MethodBase.GetCurrentMethod());
                genericAnnotationObj.name = "NewAnnotation_" + annotationsCreated;
            } else
            {
                Debug.LogFormat(GlobalVariables.cError + "Offline annotations are disabled.{0} {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }


            Annotation annotation;
            if (HelperFunctions.GetComponentInChild<Annotation>(out annotation, genericAnnotationObj, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                annotation.myAnnotationType = annotationType;
                annotation.myUniqueAnnotationNumber = annotationsCreated;
                annotation.wasLoaded = false;
                annotation.SendContentFromMaster();
                annotation.SetAnnotationObject();
                lastMadeAnnotationPhotonViewID = annotation.photonView.ViewID;
                centerAnnotationOnSpawnPoint(annotation);
                sendAnnotationIDEvent();
            }

        }

        private void centerAnnotationOnSpawnPoint(Annotation annotation)
        {
            if (annotation.myAnnotationType == Annotation.typesOfAnnotations.LINERENDER) {
                Debug.LogFormat(GlobalVariables.cTest + "Not centering annotation, is linerender.{0} {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return; 
            } else
            {
                Debug.LogFormat(GlobalVariables.cTest + "Centering annotation Type is {0} {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", annotation.myAnnotationType.ToString(), "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

            GameObject spawnPoint;
            HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.spawnTag, out spawnPoint, false, System.Reflection.MethodBase.GetCurrentMethod());

            if (annotation.myAnnotationType == Annotation.typesOfAnnotations.CENTRALITY)
            {
                annotation.transform.rotation = spawnPoint.transform.rotation;
                annotation.transform.Rotate(new Vector3(-90f, 0, 0));

                Transform centerPoint;
                centerPoint = annotation.myObjectRepresentation.transform.GetChild(0);
                //Vector3 travelPathVector = centerPoint - fromTransform.position;
                annotation.transform.position = spawnPoint.transform.position;
                annotation.transform.position = annotation.transform.position + (annotation.transform.position - centerPoint.transform.position);

            } else
            {
                annotation.transform.position = spawnPoint.transform.position;
                annotation.transform.rotation = spawnPoint.transform.rotation;
            }
            

        }

        private void CreateAnnotation(SerializeableAnnotation serializeableAnnotation)
        {


            GameObject genericAnnotationObj;

            if (PhotonNetwork.IsConnected)
            {
                

                if (serializeableAnnotation.isDeleted)
                {
                    return;
                }

                annotationsCreated++;

                genericAnnotationObj = PhotonNetwork.InstantiateRoomObject("GenericAnnotation", Vector3.zero, Quaternion.identity);
                HelperFunctions.SetObjectLocalTransformToZero(genericAnnotationObj, System.Reflection.MethodBase.GetCurrentMethod());
                genericAnnotationObj.name = "LoadedAnnotation_" + serializeableAnnotation.myAnnotationNumber;
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "Offline annotations are disabled.{0} {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            Annotation annotation;
            if (HelperFunctions.GetComponentInChild<Annotation>(out annotation, genericAnnotationObj, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                annotation.SetUpFromSerializeableAnnotation(serializeableAnnotation);
                annotation.wasLoaded = true;
                annotation.SendContentFromMaster();
                annotation.SetAnnotationObject();

            }
            lastMadeAnnotationPhotonViewID = annotation.photonView.ViewID;
            sendAnnotationIDEvent();
        }

        /// <summary>
        /// Sends new annotation ID from master client to all clients
        /// Data Sent = object[] { photonView.ViewID, new annotation photonView.ViewID };
        /// Raises = GlobalVariables.SendEventNewAnnotationID
        /// Reciver = ReceiverGroup.Others
        /// </summary>
        private void sendAnnotationIDEvent()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "MasterClient ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "sendAnnotationIDEvent()", "Others", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.SendEventNewAnnotationID, "", ", annotation ID: ", lastMadeAnnotationPhotonViewID, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, lastMadeAnnotationPhotonViewID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            PhotonNetwork.RaiseEvent(GlobalVariables.SendEventNewAnnotationID, content, raiseEventOptions, GlobalVariables.sendOptions);
        }


        private void setAnnotationIDEvent(object[] data)
        {
            lastMadeAnnotationPhotonViewID = (int)data[1];

            Debug.LogFormat(GlobalVariables.cEvent + "Recived Code: {0}, Client ~ {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}{5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", GlobalVariables.SendEventNewAnnotationID, "Updating latest Annotation ID.", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, "", "", "Annotation View ID: ", lastMadeAnnotationPhotonViewID, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

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
            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestAnnotationDeletionAll()", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestEventAnnotationRemoval, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestEventAnnotationRemoval, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        private void RespondToRequestAnnotationRemoval()
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
        
        public void RequestAnnotationFileSystemDeletion()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestAnnotationFileSystemDeletion()", "All", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestEventAnnotationFileSystemDeletion, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestEventAnnotationFileSystemDeletion, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        private void DeleteAnnotaitonFileSystem()
        {

            Debug.LogFormat(GlobalVariables.cAlert + "Deleteing all saved annotations! {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            string mainFolderName = GlobalVariables.annotationSaveFolder;
            string mainFolderPath = Path.Combine(Application.persistentDataPath, mainFolderName);
            if (Directory.Exists(mainFolderPath)) { Directory.Delete(mainFolderPath, true); }
        }

        #endregion AnnotaitonRemoval

        #region AnnotationSaveing

        private void PushAllData()
        {
            Debug.LogFormat(GlobalVariables.cFileOperations + "PushAllData called{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (!PhotonNetwork.IsConnected || !PhotonNetwork.IsMasterClient || PhotonNetwork.PlayerList.Length <= 1) { return; }

            bool loadSuccessfull = false;
            var anno = _getAllAnnotationsAndConvertToSerializeableAnnotations(out loadSuccessfull);

            if (loadSuccessfull)
            {
                foreach(SerializeableAnnotation annotation in anno)
                {
                    string jsonFormatAnnotion = JsonUtility.ToJson(annotation, true);
                    object[] content = new object[] { photonView.ViewID, jsonFormatAnnotion };

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

                    PhotonNetwork.RaiseEvent(GlobalVariables.RequestSaveAnnotation, content, raiseEventOptions, GlobalVariables.sendOptionsReliable);
                }

                Debug.LogFormat(GlobalVariables.cFileOperations + "I am the MasterClient: {0}, Pushing all annotaitons to all users." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void _saveAnnotations(object[] data)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "Annotation reviced, saving now: {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            string jsonAnnotation = (string)data[1];
            SerializeableAnnotation serializeableAnnotation = JsonUtility.FromJson<SerializeableAnnotation>(jsonAnnotation);
            _saveAnnotations(serializeableAnnotation);
        }

        public void saveAnnotations()
        {
            Debug.LogFormat(GlobalVariables.cFileOperations + "saveAnnotations called{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (!PhotonNetwork.IsMasterClient) { return; }

            Debug.LogFormat(GlobalVariables.cCommon + "I am the MasterClient: {0}, Saving annotaitons. Vis Key: " + _getParentVisAxisKey() + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            bool saveWasSuccessfull = false;

            //find all annotations and convert to serilizable annotation
            List<SerializeableAnnotation> listOfAnnotations = _getAllAnnotationsAndConvertToSerializeableAnnotations(out saveWasSuccessfull);
            if (!saveWasSuccessfull) { return; };

            //save with axis title
            _saveAnnotations(listOfAnnotations);

            Debug.LogFormat(GlobalVariables.cCommon + "{0} {1} {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Annotations uccessfully saved", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private List<SerializeableAnnotation> _getAllAnnotationsAndConvertToSerializeableAnnotations(out bool wasSuccessfull)
        {
            int countOfAnnotationsFound = 0;

            List<SerializeableAnnotation> listOfAnnotations = new List<SerializeableAnnotation>();
            GameObject[] annotationHolderObjects = GameObject.FindGameObjectsWithTag("Annotation");

            if (annotationHolderObjects.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cAlert + "{0} {1} {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "No annotation holders found. Nothing saved", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                wasSuccessfull = false;
            }
            else
            {
                foreach (GameObject annotationHolder in annotationHolderObjects)
                {
                    Annotation annotation = annotationHolder.GetComponent<Annotation>();
                    if (annotation != null)
                    {
                        listOfAnnotations.Add(annotation.GetSerializeableAnnotation());
                        countOfAnnotationsFound++;
                    }
                }
            }

            Debug.LogFormat(GlobalVariables.cCommon + "{0} {1} {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", countOfAnnotationsFound, " Annotations Found.", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            wasSuccessfull = true;
            return listOfAnnotations;
        }

        private void _saveAnnotations(List<SerializeableAnnotation> listOfSerializeableAnnotations)
        {

            string subfolderPath = _getFolderPath();

            foreach (SerializeableAnnotation serializeableAnnotation in listOfSerializeableAnnotations)
            {
                serializeableAnnotation.wasLoaded = true;

                string filename = serializeableAnnotation.myAnnotationNumber.ToString("D3");
                filename += "_" + serializeableAnnotation.myAnnotationType.ToString();
                filename += "_" + _getParentVisAxisKey() + ".json";

                string jsonFormatAnnotion = JsonUtility.ToJson(serializeableAnnotation, true);

                string fullFilePath = Path.Combine(subfolderPath, filename);

                Debug.LogFormat(GlobalVariables.cFileOperations + "Saving {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", filename, fullFilePath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                System.IO.File.WriteAllText(fullFilePath, jsonFormatAnnotion);
            }

            Debug.LogFormat(GlobalVariables.cFileOperations + "Annotations saved for {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", _getParentVisAxisKey(), subfolderPath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void _saveAnnotations(SerializeableAnnotation serializeableAnnotation)
        {

            string subfolderPath = _getFolderPath();

            serializeableAnnotation.wasLoaded = true;

            string filename = serializeableAnnotation.myAnnotationNumber.ToString("D3");
            filename += "_" + serializeableAnnotation.myAnnotationType.ToString();
            filename += "_" + _getParentVisAxisKey() + ".json";

            string jsonFormatAnnotion = JsonUtility.ToJson(serializeableAnnotation, true);

            string fullFilePath = Path.Combine(subfolderPath, filename);

            Debug.LogFormat(GlobalVariables.cFileOperations + "Saving {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", filename, fullFilePath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            System.IO.File.WriteAllText(fullFilePath, jsonFormatAnnotion);

            Debug.LogFormat(GlobalVariables.cFileOperations + "Annotation saved for {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", _getParentVisAxisKey(), subfolderPath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private string _getParentVisAxisKey()
        {
            VisualizationEvent_Calls visualisationEvent_Calls;
            if (!HelperFunctions.GetComponent<VisualizationEvent_Calls>(out visualisationEvent_Calls, System.Reflection.MethodBase.GetCurrentMethod())) { return "NoVisEventCallsFound"; }

            return visualisationEvent_Calls.axisKey;
        }

        private string _getFolderPath()
        {
            //Annotations are saved per VisState in a folder with the names of that vis axis
            string mainFolderName = GlobalVariables.annotationSaveFolder;
            string mainFolderPath = Path.Combine(Application.persistentDataPath, mainFolderName);

#if UWP
            Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;

            mainFolderPath = Path.Combine(installedLocation, mainFolderName);
#endif


            //_checkAndMakeDirectory(mainFolderPath);

            string date = System.DateTime.Now.ToString("yyyyMMdd");
            //string parentVisAxisKey = _getParentVisAxisKey();
            //string subFolderName = date + "_" + parentVisAxisKey;
            string subFolderName = date;
            string subfolderPath = Path.Combine(mainFolderPath, subFolderName);
            _checkAndMakeDirectory(subfolderPath);

            return subfolderPath;
        }

        private void _checkAndMakeDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Debug.LogFormat(GlobalVariables.cFileOperations + "Makeing new folder{0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "", directory, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                Directory.CreateDirectory(directory);
            }
        }

#endregion AnnotationSaveing

#region AnnotationLoading

        public void loadAnnotations()
        {
            isFirstLoad = false;
            if (!PhotonNetwork.IsMasterClient) { return; }

            RespondToRequestAnnotationRemoval();

            Debug.LogFormat(GlobalVariables.cCommon + "I am the MasterClient: {0}, Loading annotaitons." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //get file path
            string getFolderPath = _getFolderPath();

            string[] filePaths = Directory.GetFiles(getFolderPath, "*.json");

            Debug.LogFormat(GlobalVariables.cFileOperations + "{0} .json annotation records found in {1}, {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", filePaths.Length, getFolderPath, "Loading annotations now.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            foreach (string jsonPath in filePaths)
            {
                //Load file
                if (jsonPath.Contains(_getParentVisAxisKey()))
                {
                    SerializeableAnnotation serializeableAnnotation = JsonUtility.FromJson<SerializeableAnnotation>(File.ReadAllText(jsonPath));
                    CreateAnnotation(serializeableAnnotation);
                }
            }

        }


#endregion AnnotationLoading

        public void requestDisableFarInteractions()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestDisableFarInteraction()", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestEventAnnotationRemoval, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestDisableFarInteraction, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        private void _requestDisableFarInteractions()
        {
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Any);

        }

        public void requestEnableFarInteractions()
        {
            Debug.LogFormat(GlobalVariables.cEvent + "Any ~ Calling: {0}, Receivers: {1}, My Name: {2}, I am the Master Client: {3}, Server Time: {4}, Sending Event Code: {5}{6}{7}{8}." + GlobalVariables.endColor + " {9}: {10} -> {11} -> {12}", "RequestEnableFarInteraction()", "MasterClient", PhotonNetwork.NickName, PhotonNetwork.IsMasterClient, PhotonNetwork.Time, GlobalVariables.RequestEventAnnotationRemoval, "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            PhotonNetwork.RaiseEvent(GlobalVariables.RequestEnableFarInteraction, content, raiseEventOptions, GlobalVariables.sendOptions);
        }

        private void _requestEnableFarInteractions()
        {
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.Default, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Any);
        }

        #endregion Events
    }
}

