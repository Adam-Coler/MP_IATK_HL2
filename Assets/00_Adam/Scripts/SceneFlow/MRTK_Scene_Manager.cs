using UnityEngine;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit;
using Photon.Pun;
using UnityEngine.SceneManagement;

// loading levels is asynchoirnous so we get an error due to the interface functions being voids
#pragma warning disable CS4014

namespace Photon_IATK
{
    public class MRTK_Scene_Manager : MonoBehaviour
    {
        private static string _this = "MRTK_Scene_Manager";
        public bool isMine = false;
        private IMixedRealitySceneSystem sceneSystem = null;

        private void Awake()
        {
            getPhotonViewOfNetworkManager();
            sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        }

        public void getPhotonViewOfNetworkManager()
        {
            // this lets up use photonview.ismine to execute code on the local client only
            PhotonView photonView = null;
            GameObject NetworkManager = GameObject.FindGameObjectWithTag("NetworkManager");

            if (NetworkManager != null)
            {
                photonView = NetworkManager.GetPhotonView();
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "NetworkManager Found", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No NetworkManager Found", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

            if (photonView != null)
            {
                isMine = photonView.IsMine;
            }

            if (PhotonNetwork.IsConnected == false)
            {
                isMine = true;
                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Local View Set to True Offline", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
        }


        #region button interface

        public void load_01_SetupMenu()
        {
            Debug.LogFormat(GlobalVariables.cLevel + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Loading new level", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            _load_01_SetupMenu();
        }
        public void unload_01_SetupMenu()
        {
            Debug.LogFormat(GlobalVariables.cLevel + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Unloading level", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            _unload_01_SetupMenu();
        }


        public void load_02_EnterPID()
        {
            Debug.LogFormat(GlobalVariables.cLevel + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Loading new level", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            _load_02_EnterPID();
        }
        public void unload_02_EnterPID()
        {
            Debug.LogFormat(GlobalVariables.cLevel + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Unloading level", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            _unload_02_EnterPID();
        }

        public void load_03_Vuforia_Setup()
        {
            Debug.LogFormat(GlobalVariables.cLevel + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Loading new level", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            _load_03_Vuforia_Setup();
        }

        public void unload_03_Vuforia_Setup()
        {
            Debug.LogFormat(GlobalVariables.cLevel + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Unloading level", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            _unload_03_Vuforia_Setup();
        }



        #endregion

        #region Loaders

        private bool checkIfSceneIsLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneName)
                {
                    Debug.LogFormat(GlobalVariables.cLevel + "{0} is already loaded, nothing loading." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", sceneName, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    return false;
                }
            }
            return true;
        }

        private async System.Threading.Tasks.Task _load_01_SetupMenu()
        {
            if (isMine && checkIfSceneIsLoaded("01_SetupMenu"))
            {
                await sceneSystem.LoadContent("01_SetupMenu", LoadSceneMode.Single);
            }
        }

        private async System.Threading.Tasks.Task _unload_01_SetupMenu()
        {
            if (isMine)
            {
                await sceneSystem.UnloadContent("01_SetupMenu");
            }
        }

        private async System.Threading.Tasks.Task _load_02_EnterPID()
        {
            if (isMine)
            {
                await sceneSystem.LoadContent("02_EnterPID", LoadSceneMode.Single);
            }
        }

        private async System.Threading.Tasks.Task _unload_02_EnterPID()
        {
            if (isMine)
            {
                await sceneSystem.UnloadContent("02_EnterPID");
            }
        }

        private async System.Threading.Tasks.Task _load_03_Vuforia_Setup()
        {
            if (isMine)
            {
                await sceneSystem.LoadContent("03_Vuforia_Setup", LoadSceneMode.Single);
            }
        }

        private async System.Threading.Tasks.Task _unload_03_Vuforia_Setup()
        {
            if (isMine)
            {
                await sceneSystem.UnloadContent("03_Vuforia_Setup");
            }
        }


        #endregion

    }
}