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


        public bool isConnecting = false;
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

#if VIVE
            isAutoConnect = true;
#endif

#if DESKTOP
            isAutoConnect = true;
#endif

            if (isAutoConnect)
            {
                Connect();
                Debug.LogFormat(GlobalVariables.cAlert + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Lobby AutoConnect", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            };

            DontDestroyOnLoad(gameObject.transform.root);
        }



        #endregion

        #region Public Methods
        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
            Debug.LogFormat(GlobalVariables.cAlert + "Disconnecting from Photon{0}{1}{3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", "","","", this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Start the connection process.
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Connecting using settings", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                isConnecting = true;
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
            Debug.LogFormat(GlobalVariables.cAlert + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Already connected doing nothing...", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }
        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {

            isConnecting = false;

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
            }
            else
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

            var roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = 10 , CleanupCacheOnLeave = true};
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

        void OnConnectionFail(DisconnectCause cause)
        {
            Debug.LogFormat(GlobalVariables.cError + "OnConnectionFail {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", cause.ToString(), Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        public override void OnJoinedLobby()
        {
            isConnecting = false;
            Debug.LogFormat(GlobalVariables.cCommon + "OnJoinedLobby{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", " Success", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }


        void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            Debug.LogFormat(GlobalVariables.cError + "OnFailedToConnectToPhoton {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", cause.ToString(), Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

    }
}