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
        public int annotationsCreated = 0;


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
        
            //send all annoations to next client if master

        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} updated. Name: " + PhotonNetwork.NickName + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (propertyType == AbstractVisualisation.PropertyType.DimensionChange || propertyType == AbstractVisualisation.PropertyType.VisualisationType) { loadAnnotations(); }

        }

        private void UpdatedViewRequested(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} update requested." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //Save annotations handled by other class

            if (propertyType == AbstractVisualisation.PropertyType.DimensionChange) { saveAnnotations(); }

            //Delete annotations without marking delete but as safe
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
                Debug.LogFormat(GlobalVariables.cCommon + "I am the MasterClient: {0}, Removing annotaitons." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                RespondToRequestAnnotationRemoval();
            }
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
                    RespondToRequestAnnotationRemoval();
                    break;
                case GlobalVariables.RequestEventAnnotationFileSystemDeletion:
                    DeleteAnnotaitonFileSystem();
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
            if (!PhotonNetwork.IsConnected || !HelperFunctions.FindGameObjectOrMakeOneWithTag("AnnotationCollection", out annotationCollection, false, System.Reflection.MethodBase.GetCurrentMethod()))
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
                annotationsCreated++;
                genericAnnotationObj = PhotonNetwork.InstantiateRoomObject("GenericAnnotation", Vector3.zero, Quaternion.identity);
                HelperFunctions.SetObjectLocalTransformToZero(genericAnnotationObj, System.Reflection.MethodBase.GetCurrentMethod());
                genericAnnotationObj.name = "NewAnnotation";
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
                annotation.SendContentFromMaster();
                annotation._setAnnotationObject();
            }

            annotation.photonView.GetInstanceID();
        }

        private void CreateAnnotation(SerializeableAnnotation serializeableAnnotation)
        {
            GameObject genericAnnotationObj;

            if (PhotonNetwork.IsConnected)
            {
                annotationsCreated++;
                genericAnnotationObj = PhotonNetwork.InstantiateRoomObject("GenericAnnotation", Vector3.zero, Quaternion.identity);
                HelperFunctions.SetObjectLocalTransformToZero(genericAnnotationObj, System.Reflection.MethodBase.GetCurrentMethod());
                genericAnnotationObj.name = "LoadedAnnotation";
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "Offline annotations are disabled.{0} {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            Annotation annotation;
            if (HelperFunctions.GetComponentInChild<Annotation>(out annotation, genericAnnotationObj, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                annotation.setUpFromSerializeableAnnotation(serializeableAnnotation);
                annotation.SendContentFromMaster();
                annotation._setAnnotationObject();
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

        public void saveAnnotations()
        {
            if (!PhotonNetwork.IsMasterClient) { return; }

            Debug.LogFormat(GlobalVariables.cCommon + "I am the MasterClient: {0}, Saving annotaitons." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

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
                        listOfAnnotations.Add(annotation.getSerializeableAnnotation());
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
                string filename = serializeableAnnotation.myAnnotationNumber.ToString("D3");
                filename += "_" + serializeableAnnotation.myAnnotationType.ToString() + ".json";

                string jsonFormatAnnotion = JsonUtility.ToJson(serializeableAnnotation, true);

                string fullFilePath = Path.Combine(subfolderPath, filename);
                Debug.LogFormat(GlobalVariables.cFileOperations + "Saving {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", filename, fullFilePath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                System.IO.File.WriteAllText(fullFilePath, jsonFormatAnnotion);
            }

            Debug.LogFormat(GlobalVariables.cFileOperations + "Annotations saved for {0}, full path: {1} " + GlobalVariables.endColor + " {2}: {3} -> {4} -> {5}", _getParentVisAxisKey(), subfolderPath, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private string _getParentVisAxisKey()
        {
            VisualizationEvent_Calls visualisationEvent_Calls;
            if (!HelperFunctions.GetComponent<VisualizationEvent_Calls>(out visualisationEvent_Calls, System.Reflection.MethodBase.GetCurrentMethod())) { return "EmmulatedVisObject"; }

            return visualisationEvent_Calls.axisKey;
        }

        private string _getFolderPath()
        {
            //Annotations are saved per VisState in a folder with the names of that vis axis
            string mainFolderName = GlobalVariables.annotationSaveFolder;
            string mainFolderPath = Path.Combine(Application.persistentDataPath, mainFolderName);
            //_checkAndMakeDirectory(mainFolderPath);

            string date = System.DateTime.Now.ToString("yyyyMMdd");
            string parentVisAxisKey = _getParentVisAxisKey();
            string subFolderName = date + "_" + parentVisAxisKey;
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
            if (!PhotonNetwork.IsMasterClient) { return; }

            Debug.LogFormat(GlobalVariables.cCommon + "I am the MasterClient: {0}, Loading annotaitons." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.IsMasterClient, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //get file path
            string getFolderPath = _getFolderPath();

            string[] filePaths = Directory.GetFiles(getFolderPath, "*.json");

            Debug.LogFormat(GlobalVariables.cFileOperations + "{0} .json annotation records found in {1}, {2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", filePaths.Length, getFolderPath, "Loading annotations now.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            foreach (string jsonPath in filePaths)
            {
                //Load file
                SerializeableAnnotation serializeableAnnotation = JsonUtility.FromJson<SerializeableAnnotation>(File.ReadAllText(jsonPath));
                CreateAnnotation(serializeableAnnotation);
            }

        }


        #endregion AnnotationLoading

        #endregion Events
    }
}

