using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit;
using Photon.Pun;
using Photon.Realtime;
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
                Debug.Log(GlobalVariables.red + "NetworkManager Found" + GlobalVariables.endColor + " : " + "Awake()" + " : " + _this);
            }
            else
            {
                Debug.Log(GlobalVariables.red + "No NetworkManager Found" + GlobalVariables.endColor + " : " + "Awake()" + " : " + _this);
            }

            if (photonView != null)
            {
                isMine = photonView.IsMine;
            }

            if (PhotonNetwork.IsConnected == false)
            {
                isMine = true;
                Debug.Log(GlobalVariables.green + "Local View Set to True Offline" + GlobalVariables.endColor + " : " + "Awake()" + " : " + _this);
            }
        }


        #region button interface

        public void load_01_SetupMenu()
        {
            Debug.Log(GlobalVariables.purple + "Loading new level" + GlobalVariables.endColor + " : " + "load_01_SetupMenu()" + " : " + _this);
            _load_01_SetupMenu();
        }
        public void unload_01_SetupMenu()
        {
            Debug.Log(GlobalVariables.purple + "Unloading level" + GlobalVariables.endColor + " : " + "unload_01_SetupMenu()" + " : " + _this);
            _unload_01_SetupMenu();
        }


        public void load_02_EnterPID()
        {
            Debug.Log(GlobalVariables.purple + "Loading new level" + GlobalVariables.endColor + " : " + "load_02_EnterPID()" + " : " + _this);
            _load_02_EnterPID();
        }
        public void unload_02_EnterPID()
        {
            Debug.Log(GlobalVariables.purple + "Unloading level" + GlobalVariables.endColor + " : " + "unload_02_EnterPID()" + " : " + _this);
            _unload_02_EnterPID();
        }

        #endregion

        #region Loaders

        private async System.Threading.Tasks.Task _load_01_SetupMenu()
        {
            if (isMine)
            {
                Debug.Log(GlobalVariables.green + "01_SetupMenu Loaded" + GlobalVariables.endColor + " : " + "loadPIDEntrySceneAsync()" + " : " + _this);
                await sceneSystem.LoadContent("01_SetupMenu", LoadSceneMode.Single);
            }
        }

        private async System.Threading.Tasks.Task _unload_01_SetupMenu()
        {
            if (isMine)
            {
                Debug.Log(GlobalVariables.green + "01_SetupMenu Unloaded" + GlobalVariables.endColor + " : " + "loadPIDEntrySceneAsync()" + " : " + _this);
                await sceneSystem.UnloadContent("01_SetupMenu");
            }
        }

        private async System.Threading.Tasks.Task _load_02_EnterPID()
        {
            if (isMine)
            {
                Debug.Log(GlobalVariables.green + "02_EnterPID Loaded" + GlobalVariables.endColor + " : " + "loadPIDEntrySceneAsync()" + " : " + _this);
                await sceneSystem.LoadContent("02_EnterPID", LoadSceneMode.Single);
            }
        }

        private async System.Threading.Tasks.Task _unload_02_EnterPID()
        {
            if (isMine)
            {
                Debug.Log(GlobalVariables.green + "02_EnterPID Unloaded" + GlobalVariables.endColor + " : " + "loadPIDEntrySceneAsync()" + " : " + _this);
                await sceneSystem.UnloadContent("02_EnterPID");
            }
        }

        #endregion

    }
}