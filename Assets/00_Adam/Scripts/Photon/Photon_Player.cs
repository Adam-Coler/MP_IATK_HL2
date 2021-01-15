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

        public override void OnEnable()
        {
            setup();
            //unity has predefined tags "Player" is one
            this.tag = "Player";
        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.cOnDestory + "Destroying: {0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            PhotonNetwork.Destroy(this.gameObject);
        }

#if UNITY_5_4_OR_NEWER
        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
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


            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

        }



        [PunRPC]
        void setOrgin(PhotonMessageInfo info)
        {
            Transform transform = this.transform;

            Debug.LogFormat(GlobalVariables.cPRC + "PUN RPC call, Sender:{0}, View: {1}, SentServerTime: {3}" + GlobalVariables.endColor + " {4}: {5} -> {6} -> {7}", info.Sender, info.photonView, info.SentServerTime, this.name, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
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

                    this.gameObject.AddComponent<ButtonListeners>();

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