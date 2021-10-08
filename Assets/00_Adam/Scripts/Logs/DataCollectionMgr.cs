using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Photon_IATK
{
    /// <summary>
    /// This takes to much cpu to run on the HL2, must be run locally
    /// </summary>
    public class DataCollectionMgr : MonoBehaviourPunCallbacks
    {
        private const string SessionFolderRoot = "csvDataLogs";
        private const string delim = ",";
        private const string nan = "null";

        public static DataCollectionMgr Instance;

        private static CSVWritter GeneralData;
        private string[] generalDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "Content"};

        private static CSVWritter GeneralVisData;
        private string[] generalVisDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "NewDimension" };

        private static CSVWritter AnnotationUpdatesData;
        private string[] AnnotationUpdatesDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "AnnotationID", "AnnotationType", "AxisX", "AxisY", "AxisZ", "AxisSize", "AxisColor", "AxisKey", "Content00", "Content01", "Content3" };

        private static CSVWritter GenericTransformSyncData;
        private string[] GenericTransformSyncDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "ObjName", "ObjTag", "PositionX", "PositionY", "PositionZ", "RotationX", "RotationY", "RotationZ", "ScaleX", "ScaleY", "ScaleZ", "InstanceID", "RotX", "RotY", "RotZ", "RotW", "AnnotationType", "AnnotationID" };

        //string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, name, tag, position.x.ToString(), position.y.ToString(), position.z.ToString(), rot.eulerAngles.x.ToString(), rot.eulerAngles.y.ToString(), rot.eulerAngles.z.ToString(), scale.x.ToString(), scale.y.ToString(), scale.z.ToString(), instanceID.ToString(), rot.x.ToString(), rot.y.ToString(), rot.z.ToString(), rot.w.ToString() };

        private void Awake()
        {

#if WINDOWS_UWP
                    Destroy(this);
#endif
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        private void Start()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }
            Instance = this;

            setupGeneralDataRecording();
            setupGeneralVisRecording();
            GenericTransformSyncDataRecording();
            AnnotationDataRecording();
        }
        public void logRowsTest()
        {
            GeneralData.AddRow(new string[] { "Caller", "Nickname" });
            GeneralData.FlushData();
        }

#region setup writters
        private async void setupGeneralDataRecording()
        {
            GeneralData = gameObject.AddComponent<CSVWritter>();
            GeneralData.Initalize(SessionFolderRoot, delim, "GeneralData", generalDataHeader);
            await GeneralData.MakeNewSession();
            GeneralData.StartNewCSV();
        }

        private async void setupGeneralVisRecording()
        {
            GeneralVisData = gameObject.AddComponent<CSVWritter>();
            GeneralVisData.Initalize(SessionFolderRoot, delim, "GeneralVisData", generalVisDataHeader);
            await GeneralVisData.MakeNewSession();
            GeneralVisData.StartNewCSV();
        }

        private async void GenericTransformSyncDataRecording()
        {
            GenericTransformSyncData = gameObject.AddComponent<CSVWritter>();
            GenericTransformSyncData.Initalize(SessionFolderRoot, delim, "GenericTransformSyncData", GenericTransformSyncDataHeader);
            await GenericTransformSyncData.MakeNewSession();
            GenericTransformSyncData.StartNewCSV();
        }

        private async void AnnotationDataRecording()
        {
            AnnotationUpdatesData = gameObject.AddComponent<CSVWritter>();
            AnnotationUpdatesData.Initalize(SessionFolderRoot, delim, "AnnotationUpdatesData", AnnotationUpdatesDataHeader);
            await AnnotationUpdatesData.MakeNewSession();
            AnnotationUpdatesData.StartNewCSV();
        }

        #endregion

        private void getViewIDs()
        {
        //public const byte RPC = 200;
        //public const byte SendSerialize = 201;
        //public const byte Instantiation = 202;
        //public const byte CloseConnection = 203;
        //public const byte Destroy = 204;
        //public const byte RemoveCachedRPCs = 205;
        //public const byte SendSerializeReliable = 206; // TS: added this but it's not really needed anymore
        //public const byte DestroyPlayer = 207; // TS: added to make others remove all GOs of a player
        //public const byte OwnershipRequest = 209;
        //public const byte OwnershipTransfer = 210;
        //public const byte VacantViewIds = 211;
        //public const byte OwnershipUpdate = 212;
        }

        
        #region Logging code

        private void logAxisChange(int callerPhotonViewID, byte eventCode, object[] data)
        {
            string newDimension = (string)data[1];
            string callerNickname = (string)data[2];

            string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, newDimension };

            GeneralVisData.AddRow(rowContent);

            //Debug.LogFormat(GlobalVariables.cDataCollection + "logAxisChange: caller {0}, EventCode: {1}, caller nickname: {2}, new dimension: {3}{4}{5}" + GlobalVariables.endColor +  " : {6} -> {7} -> {8}", callerPhotonViewID, eventCode.ToString(), callerNickname, newDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void logGeneralEvent(int callerPhotonViewID, byte eventCode, string callerNickname, string content1)
        {
 
            string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, content1};

            GeneralData.AddRow(rowContent);

            //Debug.LogFormat(GlobalVariables.cDataCollection + "logGeneralEvent: caller {0}, EventCode: {1}, caller nickname: {2}, content: {3}{4}{5}" + GlobalVariables.endColor + " : {6} -> {7} -> {8}", callerPhotonViewID, eventCode.ToString(), callerNickname, content1, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void logGeneralEvent(int callerPhotonViewID, byte eventCode, string nickName)
        {
            logGeneralEvent(callerPhotonViewID, eventCode, nickName, nan);
        }

        private void logGenericTransformSyncEvent(int callerPhotonViewID, byte eventCode, object[] data)
        {
            //object[] content = new object[] { photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale, this.photonView.GetInstanceID(), PhotonNetwork.NickName, this.name, this.tag, annotationType, annotationID };

            Vector3 position = (Vector3)data[1];
            Quaternion rot = (Quaternion)data[2];
            Vector3 scale = (Vector3)data[3];
            int instanceID = (int)data[4];
            string callerNickname = (string)data[5];
            string name = (string)data[6];
            string tag = (string)data[7];
            string annotationType = (string)data[8];
            string annotationID = (string)data[9];

            string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, name, tag, position.x.ToString(), position.y.ToString(), position.z.ToString(), rot.eulerAngles.x.ToString(), rot.eulerAngles.y.ToString(), rot.eulerAngles.z.ToString(), scale.x.ToString(), scale.y.ToString(), scale.z.ToString(), instanceID.ToString(), rot.x.ToString(), rot.y.ToString(), rot.z.ToString(), rot.w.ToString(), annotationType, annotationID};

            GenericTransformSyncData.AddRow(rowContent);
        }

        private void logAnnotationUpdateEvent(int callerPhotonViewID, byte eventCode, string callerNickname, int annotationID, string annotationType)
        {
            //private string[] AnnotationUpdatesDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "AnnotationID", "AnnotationType", "AxisX", "AxisY", "AxisZ", "AxisSize", "AxisColor", "Text" };
            if (visDataInterface == null) { getVisInterface(); }

            string[] visAxis = getVisAxisState();

            string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, annotationID.ToString(), annotationType, visAxis[0], visAxis[1], visAxis[2], visAxis[3], visAxis[4], visAxis[5], nan, nan, nan };

            AnnotationUpdatesData.AddRow(rowContent);
        }

        private void logAnnotationUpdateEvent(int callerPhotonViewID, byte eventCode, string callerNickname, int annotationID, string annotationType, string summaryvalue, string axisSelection)
        {
            if (visDataInterface == null) { getVisInterface(); }

            //private string[] AnnotationUpdatesDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "AnnotationID", "AnnotationType", "AxisX", "AxisY", "AxisZ", "AxisSize", "AxisColor", "Text" };

            string[] visAxis = getVisAxisState();

            string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, annotationID.ToString(), annotationType, visAxis[0], visAxis[1], visAxis[2], visAxis[3], visAxis[4], visAxis[5], summaryvalue, axisSelection, nan};

            AnnotationUpdatesData.AddRow(rowContent);
        }

        private void logAnnotationUpdateEvent(int callerPhotonViewID, byte eventCode, string callerNickname, int annotationID, string annotationType, Vector3 point)
        {
            if (visDataInterface == null) { getVisInterface(); }

            //private string[] AnnotationUpdatesDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "AnnotationID", "AnnotationType", "AxisX", "AxisY", "AxisZ", "AxisSize", "AxisColor", "Text" };

            string[] visAxis = getVisAxisState();

            string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, annotationID.ToString(), annotationType, visAxis[0], visAxis[1], visAxis[2], visAxis[3], visAxis[4], visAxis[5], point.x.ToString(), point.y.ToString(), point.z.ToString() };

            AnnotationUpdatesData.AddRow(rowContent);
        }

        private void logAnnotationUpdateEvent(int callerPhotonViewID, byte eventCode, string callerNickname, int annotationID, string annotationType, string text)
        {
            if (visDataInterface == null) { getVisInterface(); }

            //private string[] AnnotationUpdatesDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "AnnotationID", "AnnotationType", "AxisX", "AxisY", "AxisZ", "AxisSize", "AxisColor", "Text" };
            string[] visAxis = getVisAxisState();

            string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, annotationID.ToString(), annotationType, visAxis[0], visAxis[1], visAxis[2], visAxis[3], visAxis[4], visAxis[5], text, nan, nan };

            AnnotationUpdatesData.AddRow(rowContent);

        }
        #endregion
        
        private VisualizationEvent_Calls visDataInterface;
        private void getVisInterface()
        {
            GameObject vis = GameObject.FindGameObjectWithTag(GlobalVariables.visTag);
            if (vis != null)
            {
                vis.TryGetComponent<VisualizationEvent_Calls>(out visDataInterface);
            }
        }

        private string[] getVisAxisState()
        {
            if (visDataInterface != null)
            {
                return new string[] {visDataInterface.xDimension, visDataInterface.yDimension, visDataInterface.zDimension, visDataInterface.sizeDimension, visDataInterface.colourDimension, visDataInterface.axisKey};
            } else
            {
                return new string[] { nan, nan, nan, nan, nan, nan };
            }
        }

        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Debug.LogError(eventCode);

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { Debug.Log("EVENTCODE" + " " + eventCode); return; }

            object[] data = (object[])photonEventData.CustomData;
            int callerPhotonViewID = (int)data[0];

            switch (eventCode)
            {
                // Vis change events
                case (GlobalVariables.PhotonChangeX_AxisEvent):
                //object[] content = new object[] { photonView.ViewID, newAxisDimension, PhotonNetwork.NickName };
                case (GlobalVariables.PhotonChangeY_AxisEvent):
                //object[] content = new object[] { photonView.ViewID, newAxisDimension, PhotonNetwork.NickName };
                case (GlobalVariables.PhotonChangeZ_AxisEvent):
                //object[] content = new object[] { photonView.ViewID, newAxisDimension, PhotonNetwork.NickName };
                case (GlobalVariables.PhotonChangeColorDimensionEvent):
                //object[] content = new object[] { photonView.ViewID, newAxisDimension, PhotonNetwork.NickName };
                case (GlobalVariables.PhotonChangeSizeDimensionEvent):
                //object[] content = new object[] { photonView.ViewID, newAxisDimension, PhotonNetwork.NickName };
                    logAxisChange(callerPhotonViewID, eventCode, data);
                    break;

                // genreal events
                case (GlobalVariables.PhotonVisSceneInstantiateEvent):
                    logGeneralEvent(callerPhotonViewID, eventCode, (string)data[1]);
                    Invoke("getVisInterface", 2);
                    break;
                //object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };
                case (GlobalVariables.RequestGrabEvent):
                //object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };
                case (GlobalVariables.RequestReleaseEvent):
                //object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };
                case (GlobalVariables.RequestGrabHandleEvent):
                //object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };
                case (GlobalVariables.RequestGrabScaleEvent):
                //object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };
                case (GlobalVariables.RequestReleaseHandleEvent):
                //object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };
                case (GlobalVariables.RequestDisableFarInteraction):
                //object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };
                case (GlobalVariables.RequestEnableFarInteraction):
                //object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName };.
                case (GlobalVariables.PhotonRequestNicknameUpdateEvent):
                //object[] content = new object[] { photonView.ViewID, photonView.Owner.NickName };
                    logGeneralEvent(callerPhotonViewID, eventCode, (string)data[1]);
                    break;
                case (GlobalVariables.PhotonDeleteAllObjectsWithComponentEvent):
                    //object[] data = new object[] { photonView.ViewID, className, PhotonNetwork.NickName };
                    string tmp = (string)data[2];
                    tmp = tmp.Replace(delim, "~");
                    logGeneralEvent(callerPhotonViewID, eventCode, (string)data[1], tmp);
                    break;
                case (GlobalVariables.PhotonDeleteSingleObjectsWithViewIDEvent):
                    //{ photonView.ViewID, obj.GetComponent<PhotonView>().ViewID, PhotonNetwork.NickName, obj.name };
                    logGeneralEvent(callerPhotonViewID, eventCode, (string)data[2], (string)data[4]);
                    break;
                case (GlobalVariables.PhotonMoveEvent):
                    logGenericTransformSyncEvent(callerPhotonViewID, eventCode, data);
                    break;
                //object[] content = new object[] { photonView.ViewID, myTransform.localPosition, myTransform.localRotation, myTransform.localScale, this.photonView.GetInstanceID(), "GenericTransformSync", PhotonNetwork.NickName, this.name, this.tag };

                //annoation events
                case (GlobalVariables.RequestEventAnnotationCreation):
                    //object[] content = new object[] { photonView.ViewID, HelperFunctions.SerializeToByteArray(annotationType, System.Reflection.MethodBase.GetCurrentMethod()), PhotonNetwork.NickName, annotationID};
                    Annotation.typesOfAnnotations annotationType = HelperFunctions.DeserializeFromByteArray<Annotation.typesOfAnnotations>((byte[])data[1], System.Reflection.MethodBase.GetCurrentMethod());
                    int annotationID = (int)data[3];

                    logAnnotationUpdateEvent(callerPhotonViewID, eventCode, (string)data[2], annotationID, annotationType.ToString());

                    break;
                case (GlobalVariables.RequestCentralityUpdate):
                    //object[] content = new object[] { photonView.ViewID, summeryValueType.ToString(), axisSelection.ToString(), PhotonNetwork.NickName, myUniqueAnnotationNumber };

                    logAnnotationUpdateEvent(callerPhotonViewID, eventCode, (string)data[3], (int)data[4], Annotation.typesOfAnnotations.CENTRALITY.ToString(), (string)data[1], (string)data[2]);

                    break;
                case (GlobalVariables.RequestAddPointEvent):
                    //object[] content = new object[] { photonView.ViewID, pointString, PhotonNetwork.NickName, myUniqueAnnotationNumber };

                    Vector3 point = (Vector3)data[1];

                    logAnnotationUpdateEvent(callerPhotonViewID, eventCode, (string)data[3], (int)data[4], Annotation.typesOfAnnotations.LINERENDER.ToString(), point);

                    break;
                case (GlobalVariables.RequestLineCompleation):
                    //object[] content = new object[] { photonView.ViewID, PhotonNetwork.NickName, myUniqueAnnotationNumber};
                    logAnnotationUpdateEvent(callerPhotonViewID, eventCode, (string)data[1], (int)data[3], Annotation.typesOfAnnotations.LINERENDER.ToString());
                    break;
                case (GlobalVariables.RequestTextUpdate):
                    //object[] content = new object[] { photonView.ViewID, text, PhotonNetwork.NickName, myUniqueAnnotationNumber};
                    logAnnotationUpdateEvent(callerPhotonViewID, eventCode, (string)data[2], (int)data[3], Annotation.typesOfAnnotations.LINERENDER.ToString(), (string)data[1]);
                    
                    break;
                case (GlobalVariables.PhotonSendGazeEvent):
                    Debug.LogError("Gaze data: " + (Vector3)data[3] + " " + (Vector3)data[4] + " " + (string)data[5]);
                    break;
                default:
                    break;
            }
        }

    }
}
