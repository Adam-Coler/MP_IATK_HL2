using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Photon_IATK
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        #region Fields
        // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        private string gameVersion = "2";
    
        public static Lobby _Lobby;

        #endregion

        #region MonoBehaviour CallBacks

        // MonoBehaviour method called on GameObject by Unity during early initialization phase.
        void Awake()
        {
            if (_Lobby == null)
            {
                _Lobby = this;
                Debug.Log(GlobalVariables.green + "Lobby Set" + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());
            }
            else
            {
                if (_Lobby != this)
                {
                    Destroy(_Lobby.gameObject);
                    _Lobby = this;
                    Debug.Log(GlobalVariables.green + "Lobby Destoryed then Set" + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());
                }
            }

            setup();
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
                Debug.Log(GlobalVariables.green + "Connected, Joining random room" + GlobalVariables.endColor + " : " + "Connect()" + " : " + this.GetType());
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();

                Debug.Log(GlobalVariables.green + "Connecting using settings" + GlobalVariables.endColor + " : " + "Connect()" + " : " + this.GetType());
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

                Debug.Log(GlobalVariables.green + "Nickname found, setting nickname to " + PhotonNetwork.NickName + GlobalVariables.endColor + " : " + "OnConnectedToMaster()" + " : " + this.GetType());
            } else
            {
                Debug.Log(GlobalVariables.green + "No Nickname found, setting nickname to " + PhotonNetwork.AuthValues.UserId + GlobalVariables.endColor + " : " + "OnConnectedToMaster()" + " : " + this.GetType());

                PhotonNetwork.NickName = PhotonNetwork.AuthValues.UserId;
            }

            PhotonNetwork.JoinRandomRoom();

            Debug.Log(GlobalVariables.green + "Join Random Room Called" + GlobalVariables.endColor + " : " + "OnConnectedToMaster()" + " : " + this.GetType());
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat(this.GetType() + " : OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(GlobalVariables.green + "No random room available, Calling: PhotonNetwork.CreateRoom" + GlobalVariables.endColor + " : " + "OnJoinRandomFailed()" + " : " + this.GetType());
            var roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
            PhotonNetwork.CreateRoom("Room" + Random.Range(1, 3000), roomOptions);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log(GlobalVariables.green + "A player joined the room" + GlobalVariables.endColor + " : " + "OnJoinedRoom()" + " : " + this.GetType());
            Debug.Log(GlobalVariables.green + "Current room name: " + PhotonNetwork.CurrentRoom.Name + GlobalVariables.endColor + " : " + "OnJoinedRoom()" + " : " + this.GetType());
            Debug.Log(GlobalVariables.green + "Other players in room: " + PhotonNetwork.CountOfPlayersInRooms + GlobalVariables.endColor + " : " + "OnJoinedRoom()" + " : " + this.GetType());
            Debug.Log(GlobalVariables.green + "Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1) + GlobalVariables.endColor + " : " + "OnJoinedRoom()" + " : " + this.GetType());
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
            Debug.Log(this.GetType() + ": No Setup needed");
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