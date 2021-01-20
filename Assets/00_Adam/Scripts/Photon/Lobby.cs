using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Reflection;

namespace Photon_IATK
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        #region Fields
        // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        private string gameVersion = "3";

        public bool isAutoConnect = false;

        public static Lobby _Lobby;

        #endregion

        #region MonoBehaviour CallBacks

        // MonoBehaviour method called on GameObject by Unity during early initialization phase.
        void Awake()
        {

            if (_Lobby == null)
            {
                _Lobby = this;

                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Lobby Set", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
            else
            {
                if (_Lobby != this)
                {
                    Destroy(_Lobby.gameObject);
                    _Lobby = this;
                    Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Lobby Destoryed then Set", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
            }

            setup();

#if VIVE
            isAutoConnect = true;
#endif

#if DESKTOP
            isAutoConnect = true;
#endif

            if (isAutoConnect) { 
                Connect();
                Debug.LogFormat(GlobalVariables.cAlert + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Lobby AutoConnect", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            };

            DontDestroyOnLoad(gameObject.transform.root);
        }



        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Connected, Joining random room", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();

                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Connecting using settings", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
        }
        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            var randomUserId = Random.Range(0, 999999);
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = randomUserId.ToString();


            //PlayerPrefs.SetString(playerNamePrefKey, value);
            if (PlayerPrefs.HasKey(GlobalVariables.PlayerPrefsKeys.ParticipantID.ToString()))
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString(GlobalVariables.PlayerPrefsKeys.ParticipantID.ToString());

                Debug.LogFormat(GlobalVariables.cCommon + "Nickname found, setting nickname to {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.NickName, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            } else
            {
                Debug.LogFormat(GlobalVariables.cCommon + "No Nickname found, setting nickname to {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", PhotonNetwork.AuthValues.UserId, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                PhotonNetwork.NickName = PhotonNetwork.AuthValues.UserId;
            }

            PhotonNetwork.JoinRandomRoom();

            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Join Random Room Called", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogFormat(GlobalVariables.cError + "OnDisconnected() was called by PUN with reason: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", cause, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No random room available, Calling: PhotonNetwork.CreateRoom", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            var roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
            PhotonNetwork.CreateRoom("Room" + Random.Range(1, 3000), roomOptions);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log(GlobalVariables.cCommon + "A player joined the room" + GlobalVariables.endColor + " : " + "OnJoinedRoom()" + " : " + this.GetType());
            Debug.Log(GlobalVariables.cCommon + "Current room name: " + PhotonNetwork.CurrentRoom.Name + GlobalVariables.endColor + " : " + "OnJoinedRoom()" + " : " + this.GetType());
            Debug.Log(GlobalVariables.cCommon + "Other players in room: " + PhotonNetwork.CountOfPlayersInRooms + GlobalVariables.endColor + " : " + "OnJoinedRoom()" + " : " + this.GetType());
            Debug.Log(GlobalVariables.cCommon + "Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1) + GlobalVariables.endColor + " : " + "OnJoinedRoom()" + " : " + this.GetType());
        }

        public void OnCancelButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        static void enableVR()
        {
            UnityEngine.XR.XRSettings.enabled = true;
            UnityEngine.XR.XRSettings.LoadDeviceByName("OpenVR");
        }

        static void disableVR()
        {
            UnityEngine.XR.XRSettings.enabled = false;
            UnityEngine.XR.XRSettings.LoadDeviceByName("None");
        }



        #region CUSTOM



#if DESKTOP
        void setup()
        {
            disableVR();
        }

#elif HL2
        void setup()
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No setup needed", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

#elif VIVE
        void setup()
        {
            enableVR();
        }
#else

        void setup()
        {
            Debug.Log(this.GetType() + ": ERROR! No directive set");
        }

#endif
    #endregion


}
}