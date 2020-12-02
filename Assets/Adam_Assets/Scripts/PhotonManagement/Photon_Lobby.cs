using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;
//using UnityEditor.Build.Reporting;
using UnityEngine.XR;


namespace Photon_IATK
{
    public class Photon_Lobby : MonoBehaviourPunCallbacks
    {
        #region Private Fields
        // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        string gameVersion = "2";

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;

        #endregion

        #region MonoBehaviour CallBacks

        // MonoBehaviour method called on GameObject by Unity during early initialization phase.
        void Awake()
        {
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;

        }

        // MonoBehaviour method called on GameObject by Unity during initialization phase.
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            setup();
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
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }
        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
            Debug.Log(this.GetType() + ": OnConnectedToMaster() was called by PUN");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            isConnecting = false;
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat(this.GetType() + " : OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(this.GetType() + ": OnJoinRandomFailed() was called by PUN. No random room available, Calling: PhotonNetwork.CreateRoom");
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = GlobalVariables.maxPlayers});
        }

        public override void OnJoinedRoom()
        {
            Debug.Log(this.GetType() + ": OnJoinedRoom() called");
            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel(GlobalVariables.scenes.Room01.ToString());
            }
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
            disableVR();
        }

#elif VIVE
        void setup()
        {
            enableVR();
        }
#else

        void setup()
        {

        }

#endif
        #endregion


    }
}