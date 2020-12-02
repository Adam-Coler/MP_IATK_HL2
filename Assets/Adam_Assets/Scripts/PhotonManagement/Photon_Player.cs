using UnityEngine;
using Photon.Pun;
//using Valve.VR;
using Microsoft.MixedReality.Toolkit;

namespace Photon_IATK
{
    public class Photon_Player : MonoBehaviourPunCallbacks
    {


        #region Public Fields

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [SerializeField]
        public Photon_Cammera_Manager _Cammera_Manager;

        [SerializeField]
        public TMPro.TextMeshPro txtNickName;

        [SerializeField]
        public GameObject VivePrefab;

        [SerializeField]
        public MixedRealityToolkitConfigurationProfile configurationProfile;
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
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
#endif

        #endregion

        #region Custom
#if DESKTOP
        private void setup()
        {
            this.gameObject.AddComponent<DESKTOP_Movement>();
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
                    leftController.AddComponent<SteamVR_Behaviour_Pose>();
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

#else

        private void setup()
        {
            Debug.Log("ERROR IN PHOTON PLAYER CS NO SYSTEM SETUP");
        }

#endif
        #endregion

    }
}