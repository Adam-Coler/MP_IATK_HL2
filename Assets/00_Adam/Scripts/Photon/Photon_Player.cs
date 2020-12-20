using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
namespace Photon_IATK
{
    public class Photon_Player : MonoBehaviourPunCallbacks
    {


        #region Public Fields

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public bool isMine = false;

        [SerializeField]
        public Photon_Cammera_Manager _Cammera_Manager;

        [SerializeField]
        public TMPro.TextMeshPro txtNickName;

        [SerializeField]
        public GameObject VivePrefab;

        public Vector3 CurrentTransform;
        public Vector3 NewTransform;

        #endregion

        #region Private Fields
        private bool isSetup = false;
        #endregion

        #region Private Methods
#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif
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

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

#endif
        }

        public override void OnEnable()
        {
            setup();

            //unity has predefined tags "Player" is one
            this.tag = "Player";
        }

        public void synchornizeOnMe()
        {
            //if (!photonView.IsMine) { return; };

            //Photon_Player[] players = FindObjectsOfType<Photon_Player>();

            //foreach (Photon_Player player in players)
            //{
            //    if (!player.photonView.IsMine)
            //    {

            //        var transform = player.gameObject.transform.position;
            //        var rotation = player.gameObject.transform.rotation;
            //        MixedRealityPlayspace.Position = transform;
            //        MixedRealityPlayspace.Rotation = rotation;

            //        Debug.Log(GlobalVariables.purple + "Setting Transform " + GlobalVariables.endColor + GlobalVariables.green + "T: " + transform + GlobalVariables.endColor + " " + GlobalVariables.red + " R: " + rotation + GlobalVariables.endColor);
            //    }
            //}

        }

        /// <summary>
        /// </summary>
        //void Update()
        //{
        //    if (photonView.IsMine || PhotonNetwork.IsConnected == false)
        //    {

        //    }
        //}

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            //if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            //{
            //    transform.position = new Vector3(0f, 0f, 0f);
            //}

        }

#if UNITY_5_4_OR_NEWER
        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
#endif

        #endregion

        #region PunRPC
        [PunRPC]
        void setNickname(PhotonMessageInfo info)
        {
            txtNickName.text = photonView.Owner.NickName;
            // the photonView.RPC() call is the same as without the info parameter.
            // the info.Sender is the player who called the RPC.
            Debug.LogFormat("Info: {0} {1} {2}", info.Sender, info.photonView, info.SentServerTime);
        }



        [PunRPC]
        void setOrgin(PhotonMessageInfo info)
        {
            Transform transform = this.transform;


            Debug.LogFormat("Info: {0} {1} {2}", info.Sender, info.photonView, info.SentServerTime);
        }


        #endregion
        #region Custom
#if DESKTOP
        private void setup()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            this.gameObject.AddComponent<DESKTOP_Movement>();
                        //unity has predefined tags "Player" is one
            if (this.gameObject.GetComponent<Photon_Cammera_Manager>() != null)
            {
                _Cammera_Manager.trackedObj = this.gameObject;
                _Cammera_Manager.OnStartFollowing();
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

            this.gameObject.AddComponent<EventSystem>();
            this.gameObject.AddComponent<StandaloneInputModule>();

        }
#elif VIVE
        private void setup(){
            Debug.Log(GlobalVariables.green + "VIVE Setup... " + GlobalVariables.endColor + this.GetType().Name.ToString());
            if (photonView.IsMine)
            {
                if (!isSetup)
                {
                    isSetup = true;
                    GameObject leftController;
                    leftController = PhotonNetwork.Instantiate("ViveLeftController", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                    leftController.GetComponent<GenericNetworkSyncTrackedDevice>().isUser = true;
                }
            }

                        //unity has predefined tags "Player" is one
            if (this.gameObject.GetComponent<Photon_Cammera_Manager>() != null)
        {
            if (photonView.IsMine)
            {
                _Cammera_Manager.trackedObj = this.gameObject;
                _Cammera_Manager.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
        }

        void Update()
        {
                synchornizeOnMe();

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