using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Photon_IATK
{
    public class Photon_Player : MonoBehaviourPun
    {


        #region Public Fields

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public bool isMine = false;

        public Photon_Cammera_Manager _Cammera_Manager;
        public TMPro.TextMeshPro txtNickName;

        public Vector3 CurrentTransform;
        public Vector3 NewTransform;

        #endregion

        #region Private Fields
        private bool isSetup = false;
        #endregion

        #region MonoBehaviour CallBacks
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                Photon_Player.LocalPlayerInstance = this.gameObject;
                isMine = true;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
            txtNickName.text = photonView.Owner.NickName;
        }

        public void OnEnable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Photon_Player registering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

            setup();
            //unity has predefined tags "Player" is one
            this.tag = "Player";
        }

        private void OnDestroy()
        {

        }

#if UNITY_5_4_OR_NEWER
        public void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Photon_Player unregistering OnEvent.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }
#endif

        #endregion

        #region Events
        private void OnEvent(EventData photonEventData)
        {
            byte eventCode = photonEventData.Code;

            //Check that the event was one we made, photon reserves 0, 200+
            if (eventCode == 0 || eventCode > 199) { return; }

            object[] data = (object[])photonEventData.CustomData;

            //route the event
            switch (eventCode)
            {
                case GlobalVariables.PhotonRequestHideControllerModelsEvent:
                    showHideControllerModels();
                    Debug.Log("PhotonRequestHideControllerModelsEvent");
                    break;
                case GlobalVariables.PhotonRequestNicknameUpdateEvent:
                    setNickname();
                    Debug.Log("PhotonRequestNicknameUpdateEvent");
                    break;
                default:
                    break;
            }
        }
        public void RequestNicknameChangeEvent()
        {
            if (PhotonNetwork.IsConnected)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

                object[] content = new object[] { photonView.ViewID };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestNicknameUpdateEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
            else
            {
                setNickname();
            }
        }

        public void RequestHideControllerModelsEvent()
        {
            if (PhotonNetwork.IsConnected)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //Will not recived own message

                object[] content = new object[] { photonView.ViewID };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestHideControllerModelsEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
            else
            {
                showHideControllerModels();
            }
        }


        void setNickname()
        {
            txtNickName.text = photonView.Owner.NickName;

        }

        public void showHideControllerModels()
        {
            HelperFunctions.hideShowChildrenOfTag(GlobalVariables.gameControllerModelTag);

            Debug.LogFormat(GlobalVariables.cCommon + "{0}{1}{2}{3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", "Hiding Controller Models","","","", this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        #endregion
        #region Custom
#if DESKTOP
        private void setup()
        {
            //if (!photonView.IsMine)
            //{
            //    return;
            //}

            //Debug.LogFormat(GlobalVariables.cComponentAddition + "{0}{1}{3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", "Adding Desktop Controls", "", "", this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //this.gameObject.AddComponent<DESKTOP_Movement>();
                        //unity has predefined tags "Player" is one
            //if (this.gameObject.GetComponent<Photon_Cammera_Manager>() != null)
            //{
            //    _Cammera_Manager.trackedObj = this.gameObject;
            //    _Cammera_Manager.OnStartFollowing();
            //}
            //else
            //{
            //    Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            //}
        }
#elif VIVE
        private void setup(){
            Debug.Log(GlobalVariables.green + "VIVE Setup... " + GlobalVariables.endColor + this.GetType().Name.ToString());
            if (photonView.IsMine)
            {
                if (!isSetup)
                {
                    isSetup = true;

                    this.gameObject.AddComponent<PrimaryButtonWatcher>();
                    this.gameObject.AddComponent<PenButtonWatcher>();

                    LoadControllerModels loadControllerModelsLeft = this.gameObject.AddComponent<LoadControllerModels>();
                    loadControllerModelsLeft.isLeft = true;
                    loadControllerModelsLeft.setUp();

                    LoadControllerModels loadControllerModelsRight = this.gameObject.AddComponent<LoadControllerModels>();
                    loadControllerModelsRight.isLeft = false;
                    loadControllerModelsRight.setUp();

                    //this.gameObject.AddComponent<ButtonListeners>();

                }
            }
        }

        void Update()
        {
            if (photonView.IsMine || PhotonNetwork.IsConnected == false)
            {
                this.gameObject.transform.position = Camera.allCameras[0].transform.position;
                this.gameObject.transform.rotation = Camera.allCameras[0].transform.rotation;
            }
        }

#elif HL2
        private void setup()
        {
            Debug.Log(GlobalVariables.green + "HL2 Setup... " + GlobalVariables.endColor + this.GetType().Name.ToString());
            if (photonView.IsMine)
            {
                if (!isSetup)
                {
                    isSetup = true;
                    Microsoft.MixedReality.Toolkit.MixedRealityPlayspace.AddChild(this.gameObject.transform);
                }
            }
        }

        void Update()
        {
            //if (NewTransform != CurrentTransform)
            //{
            //    synchornizeOnMe();
            //}

            if (photonView.IsMine || PhotonNetwork.IsConnected == false)
            {
                this.gameObject.transform.position = Camera.allCameras[0].transform.position;
                this.gameObject.transform.rotation = Camera.allCameras[0].transform.rotation;
            }
        }

#else

        private void setup()
        {
            Debug.Log("ERROR IN PHOTON PLAYER CS NO SYSTEM SETUP");
        }

#endif
        #endregion

    }
}