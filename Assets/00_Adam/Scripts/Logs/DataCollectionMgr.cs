using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Photon_IATK
{
    public class DataCollectionMgr : MonoBehaviourPunCallbacks
    {
        private const string SessionFolderRoot = "csvDataLogs";
        private const string delim = ",";

        public static DataCollectionMgr Instance;

        private static CSVWritter GeneralData;
        private string[] generalDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "Content" };

        private static CSVWritter GeneralVisData;
        private string[] generalVisDataHeader = new string[] { "Caller", "EventCode", "CallerNickname", "NewDimension" };

        private void Awake()
        {
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
        }

        public void logRowsTest()
        {
            GeneralData.AddRow(new string[] { "Caller", "Nickname" });
            GeneralData.FlushData();
        }

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

        private void logAxisChange(int callerPhotonViewID, byte eventCode, object[] data)
        {
            string newDimension = (string)data[1];
            string callerNickname = (string)data[2];

            string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, newDimension };

            GeneralVisData.AddRow(rowContent);

            Debug.LogFormat(GlobalVariables.cDataCollection + "logAxisChange: caller {0}, EventCode: {1}, caller nickname: {2}, new dimension: {3}{4}{5}" + GlobalVariables.endColor +  " : {6} -> {7} -> {8}", callerPhotonViewID, eventCode.ToString(), callerNickname, newDimension, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void logGeneralEvent(int callerPhotonViewID, byte eventCode, string callerNickname, string content)
        {
 
            string[] rowContent = new string[] { callerPhotonViewID.ToString(), eventCode.ToString(), callerNickname, content };

            GeneralData.AddRow(rowContent);

            Debug.LogFormat(GlobalVariables.cDataCollection + "logGeneralEvent: caller {0}, EventCode: {1}, caller nickname: {2}, content: {3}{4}{5}" + GlobalVariables.endColor + " : {6} -> {7} -> {8}", callerPhotonViewID, eventCode.ToString(), callerNickname, content, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void logGeneralEvent(int callerPhotonViewID, byte eventCode, string nickName)
        {
            logGeneralEvent(callerPhotonViewID, eventCode, nickName, "null");
        }

        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

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
                case (GlobalVariables.RequestEventAnnotationCreation):
                    //object[] content = new object[] { photonView.ViewID, HelperFunctions.SerializeToByteArray(annotationType, System.Reflection.MethodBase.GetCurrentMethod()), PhotonNetwork.NickName };
                    Annotation.typesOfAnnotations annotationType = HelperFunctions.DeserializeFromByteArray<Annotation.typesOfAnnotations>((byte[])data[1], System.Reflection.MethodBase.GetCurrentMethod());
                    logGeneralEvent(callerPhotonViewID, eventCode, (string)data[2], annotationType.ToString());
                    break;
                case (GlobalVariables.PhotonDeleteAllObjectsWithComponentEvent):
                    //object[] data = new object[] { photonView.ViewID, className, PhotonNetwork.NickName };
                    string tmp = (string)data[2];
                    tmp = tmp.Replace(delim, "~");
                    logGeneralEvent(callerPhotonViewID, eventCode, tmp, (string)data[1]);
                    break;
                case (GlobalVariables.PhotonDeleteSingleObjectsWithViewIDEvent):
                    //{ photonView.ViewID, obj.GetComponent<PhotonView>().ViewID, PhotonNetwork.NickName, obj.name };
                    logGeneralEvent(callerPhotonViewID, eventCode, (string)data[2], (string)data[4]);
                    break;

                default:
                    break;
            }
        }

    }
}

namespace Photon_IATK
{
    class tmp
    {


        public void tmp2()
        {
            switch (1)
            {
                //Movement 1-9
                case (GlobalVariables.PhotonMoveEvent):
                case (GlobalVariables.PhotonRequestTransformEvent):
                case (GlobalVariables.PhotonRespondToRequestTransformEvent):

                // new annotation, send id event next
                case (GlobalVariables.RequestEventAnnotationCreation):
                    //object[] content = new object[] { photonView.ViewID, HelperFunctions.SerializeToByteArray(annotationType, System.Reflection.MethodBase.GetCurrentMethod()), PhotonNetwork.NickName };
                    //Annotation.typesOfAnnotations annotationType = HelperFunctions.DeserializeFromByteArray<Annotation.typesOfAnnotations>((Byte[])data[1], System.Reflection.MethodBase.GetCurrentMethod());

                // just asks for content
                //case (GlobalVariables.RequestEventAnnotationContent):

                // this is good
                case (GlobalVariables.RespondEventWithContent):
                //object[] content = new object[] { photonView.ViewID, this.GetJSONSerializedAnnotationString(), PhotonNetwork.NickName };

                // destroys all annotations
                //case (GlobalVariables.RequestEventAnnotationRemoval):

                //One offs 90-99

                //Pen 
                case (GlobalVariables.RequestAddPointEvent):
                case (GlobalVariables.RequestLineCompleation):

                //Text Annotation
                case (GlobalVariables.RequestTextUpdate):

                //Centrality
                case (GlobalVariables.RequestCentralityUpdate):
                    break;
                default:
                    break;
            }
        }
    }
}

