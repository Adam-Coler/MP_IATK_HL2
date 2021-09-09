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

            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
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
            int photonViewID = (int)data[0];

            if (photonViewID != photonView.ViewID) { return; }

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
                case GlobalVariables.PhotonRequestHideExtrasEvent:
                    showHideExtras();
                    Debug.Log("PhotonRequestHideExtrasEvent");
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

        public void RequestHideExtrasEvent()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.LogFormat(GlobalVariables.cEvent + "Calling RequestHideExtrasEvent, to: {0}, code: {1}{2}{3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", "others", GlobalVariables.PhotonRequestHideExtrasEvent, "", "", this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; //Will not recived own message

                object[] content = new object[] { photonView.ViewID };

                PhotonNetwork.RaiseEvent(GlobalVariables.PhotonRequestHideExtrasEvent, content, raiseEventOptions, GlobalVariables.sendOptions);
            }
            else
            {
                showHideExtras();
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

        public void showHideExtras()
        {
            GameObject[] extras = GameObject.FindGameObjectsWithTag(GlobalVariables.ExtraTag);

            foreach (GameObject extra in extras)
            {
                bool currentState = extra.transform.GetChild(0).gameObject.activeSelf;

                if (currentState)
                {
                    extra.transform.GetChild(0).gameObject.SetActive(false);

                    Debug.LogFormat(GlobalVariables.cCommon + "Hiding {0}, current state: {1}, settting to: {2}, parent: {3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", extra.name, "True", "False", extra.transform.parent.name, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                } else
                {
                    extra.transform.GetChild(0).gameObject.SetActive(true);

                    Debug.LogFormat(GlobalVariables.cCommon + "Hiding {0}, current state: {1}, settting to: {2}, parent: {3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", extra.name, "False", "True", extra.transform.parent.name, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
                
            }
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

                    Debug.LogFormat(GlobalVariables.cComponentAddition + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Adding Left Controller", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    LoadControllerModels loadControllerModelsLeft = this.gameObject.AddComponent<LoadControllerModels>();
                    loadControllerModelsLeft.isLeft = true;
                    loadControllerModelsLeft.setUp();

                    Debug.LogFormat(GlobalVariables.cComponentAddition + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Adding Right Controller", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

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