

namespace Photon_IATK
{
    public class GlobalVariables
    {
        public static byte maxPlayers = 5;

        public static bool JSONPrettyPrint = true;

        public static string red = "<color=#ff0000>";
        public static string green = "<color=#00ff00>";
        public static string purple = "<color=#ff00ff>";
        public static string yellow = "<color=#ffff00>";
        public static string blue = "<color=#0000FF>";
        public static string orange = "<color=#FC6A03>";


        public static string cLevel = purple;
        public static string cError = red;
        public static string cCommon = green;
        public static string cInstance = yellow;
        public static string cOnDestory = blue;
        public static string cSingletonSetting = green;
        public static string cComponentAddition = green;
        public static string cPRC = yellow;
        public static string cRegister = yellow;
        public static string cAlert = orange;
        public static string cTest = orange;
        public static string cSerialize = orange;
        public static string cFileOperations = orange;
        public static string cEvent = yellow;
        public static string cJSON = purple;

        public static string endColor = "</color>";
        public static string newLine = "\n";

        public static string Desktop = "DESKTOP";
        public static string Vive = "VIVE";
        public static string HL2 = "HL2";

        public static string debugLogTag = "DebugLog";
        public static string photonLogTag = "PhotonLog";
        public static string visTag = "Vis";
        public static string annotationTag = "Annotation";
        public static string annotationCollectionTag = "AnnotationCollection";
        public static string dataDrivenAnnotationTag = "DataDrivenAnnotations";
        public static string annotationSaveFolder = "AnnotationFiles";
        public static string visInterfaceMenuTag = "VisMenu";
        public static string gameControllerModelTag = "GameController";

        public enum allSymbols
        {
            DESKTOP,
            VIVE,
            HL2
        };

        public enum PlayerPrefsKeys
        {
            ParticipantID
        };

        public static int photonSendRate = 60;
        public static int iPunObservableUpdateRate = photonSendRate;
        public static ExitGames.Client.Photon.SendOptions sendOptions = ExitGames.Client.Photon.SendOptions.SendUnreliable;

        //Movement 1-9
        public const byte PhotonMoveEvent = 1;
        public const byte PhotonRequestTransformEvent = 2;
        public const byte PhotonRespondToRequestTransformEvent = 3;

        //Instancing 10 - 19
        public const byte PhotonVisSceneInstantiateEvent = 10;
        public const byte PhotonDeleteAllObjectsWithComponentEvent = 11;
        public const byte PhotonDeleteSingleObjectsWithViewIDEvent = 12;

        //VisUpdates 20 - 39
        public const byte PhotonChangeX_AxisEvent = 20;
        public const byte PhotonChangeY_AxisEvent = 21;
        public const byte PhotonChangeZ_AxisEvent = 22;
        public const byte PhotonChangeSizeDimensionEvent = 23;
        public const byte PhotonChangeColorDimensionEvent = 24;
        public const byte PhotonRequestStateEvent = 25;
        public const byte PhotonRequestStateEventResponse = 26;

        //Player 40 - 49
        public const byte PhotonRequestHideControllerModelsEvent = 40;
        public const byte PhotonRequestNicknameUpdateEvent = 41;

        //Annotations 50 - 69
        public const byte PhotonRequestAnnotationsSaveAllEvent = 50;
        public const byte PhotonRequestAnnotationsDeleteAllEvent = 51;
        public const byte PhotonRequestAnnotationsDeleteOneEvent = 52;
        public const byte PhotonRequestAnnotationsLoadAllEvent = 53;
        public const byte SendEventNewAnnotationID = 54;

        public const byte PhotonRequestAnnotationsListOfIDsEvent = 55;
        //public const byte PhotonResponseRequestAnnotationsListOfIDsEventNONE_FOUNDEvent = 55;
        //public const byte PhotonResponseToRequestAnnotationsListOfIDsEvent = 56;
        //public const byte PhotonRequestAnnotationsByListOfIDsEvent = 57;
        //public const byte PhotonResponseToRequestAnnotationsByListOfIDsEvent = 58;

        //Annotations 2 70 - 79
        public const byte RequestEventAnnotationCreation = 70;
        public const byte RequestEventAnnotationContent = 71;
        public const byte RespondEventWithContent = 72;

        public const byte RequestEventAnnotationRemoval = 80;
        public const byte RequestEventAnnotationFileSystemDeletion = 81;
        //public const byte RespondToRequestAnnotationDeletionAll = 81;

        //Pen 90-99
        public const byte RequestAddPointEvent = 90;

        //Other 100-109
        public const byte PhotonRequestLatencyCheckEvent = 100;
        public const byte PhotonRequestLatencyCheckResponseEvent = 101;

        //public const byte PhotonEvent = 6;



        //Debug.LogFormat(GlobalVariables.cRegister + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


        //Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, "Static: Pun_Player_RPC_Calls", this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
    }
}
