using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{
    public class Btn_Functions_For_In_Scene_Scripts : MonoBehaviour
    {

        #region Private variables

        private PhotonView photonView;
        private bool isMine;
        public static Btn_Functions_For_In_Scene_Scripts Instance;

        #endregion

        #region Private Functions

        private void Awake()
        {
            if (Instance == null)
            {
                Debug.Log(GlobalVariables.green + "Setting Btn_Functions_For_In_Scene_Scripts.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());
                Instance = this;
            }
            else
            {
                if (Instance == this) return;

                Debug.Log(GlobalVariables.green + "Destroying then setting Btn_Functions_For_In_Scene_Scripts.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());

                Destroy(Instance.gameObject);
                Instance = this;
            }
        }

        private bool checkSetViewOwnership()
        {
            if (!PhotonNetwork.IsConnected)
            {
                isMine = true;
                return true;
            }

            if (!this.TryGetComponent<PhotonView>(out photonView))
            {
                photonView = this.gameObject.AddComponent<PhotonView>();

                isMine = photonView.IsMine;
                return photonView.IsMine;
            }

            Debug.Log(GlobalVariables.red + "Current View error" + GlobalVariables.endColor + " : " + "checkSetViewOwnership()" + " : " + this.GetType());
            return photonView.IsMine;
        }

        private bool GetOrAddComponent<T>(out T component) where T : Component
        {
            component = null;

            if (!checkSetViewOwnership())
            {
                Debug.Log(GlobalVariables.red + "Current View is not mine" + GlobalVariables.endColor + " : " + "GetOrAddComponent()" + " : " + this.GetType());
                return false;
            }

            component = FindObjectOfType(typeof(T)) as T;

            if (component != null)
            {
                Debug.Log(GlobalVariables.green + "Found " + component.GetType() + " , returning it. " + GlobalVariables.endColor + " : " + "GetOrAddComponent()" + " : " + this.GetType());
                return true;
            }
            else
            {
                component = gameObject.AddComponent<T>() as T;
                Debug.Log(GlobalVariables.green + "Attaching " + component.GetType() + " and returning it. " + GlobalVariables.endColor + " : " + "GetOrAddComponent()" + " : " + this.GetType());
                return true;
            }
        }

        private T GetOrAddComponent<T>() where T : Component
        {
            T component;
            GetOrAddComponent<T>(out component);
            if (component == null)
            {
                Debug.Log(GlobalVariables.red + "Failed to get compenent: " + component.GetType() + GlobalVariables.endColor + " : GetOrAddComponent<T>(), " + this.GetType());
            }
            return component;
        }

        #endregion

        #region Btn_Calls

        #region sceneManager
        public void sceneManager_Load_01_SetupMenu()
        {
            GetOrAddComponent<MRTK_Scene_Manager>().load_01_SetupMenu();
        }

        public void sceneManager_load_02_EnterPID()
        {
            GetOrAddComponent<MRTK_Scene_Manager>().load_02_EnterPID();
        }

        public void sceneManager_load_03_VuforiaSetup()
        {
            GetOrAddComponent<MRTK_Scene_Manager>().load_03_Vuforia_Setup();
        }
        #endregion

        #region Photon
        public void Lobby_Connect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                GetOrAddComponent<Lobby>().Connect();
            }
        }
        #endregion

        #region Logs

        public void showDebugLog()
        {
            if (GameObject.FindGameObjectWithTag("DebugLog") != null)
            {
                GameObject menuObj = GameObject.FindGameObjectWithTag("DebugLog").transform.GetChild(0).gameObject;
                GameObject.FindGameObjectWithTag("DebugLog").transform.GetChild(0).gameObject.SetActive(!menuObj.activeSelf);
            }
            else
            {
                Debug.Log(GlobalVariables.red + "No DebugLog in Scene " + GlobalVariables.endColor + " : " + "showDebugLog()" + " : " + this.GetType());
            }
        }

        public void showPhotonLog()
        {
            if (GameObject.FindGameObjectWithTag("PhotonLog") != null)
            {
                GameObject menuObj = GameObject.FindGameObjectWithTag("PhotonLog").transform.GetChild(0).gameObject;
                GameObject.FindGameObjectWithTag("PhotonLog").transform.GetChild(0).gameObject.SetActive(!menuObj.activeSelf);
            }
            else
            {
                Debug.Log(GlobalVariables.red + "No PhotonLog in Scene " + GlobalVariables.endColor + " : " + "showPhotonLog()" + " : " + this.GetType());
            }
        }

        #endregion

        #endregion

    }
}
