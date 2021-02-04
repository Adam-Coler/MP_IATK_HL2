using System;
using Photon.Pun;
using UnityEngine;

namespace Photon_IATK
{
    public class GenericNetworkManager : MonoBehaviour
    {
        public static GenericNetworkManager Instance;

        [HideInInspector] public string azureAnchorId = "";
        [HideInInspector] public PhotonView localUser;
        private bool isConnected;

        private void Awake()
        {
            if (Instance == null)
            {
                Debug.LogFormat(GlobalVariables.cCommon + "Setting GenericNetworkManager.Instance{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                Instance = this;
            }
            else
            {
                if (Instance != this)
                {
                    Debug.LogFormat(GlobalVariables.cCommon + "Destroying then setting GenericNetworkManager.Instance{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    Destroy(Instance.gameObject);

                    Instance = this;
                }
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            ConnectToNetwork();
        }

        // For future non PUN solutions
        private void StartNetwork(string ipAddress, string port)
        {
            throw new NotImplementedException();
        }

        private void ConnectToNetwork()
        {
            OnReadyToStartNetwork?.Invoke();
        }

        public static event Action OnReadyToStartNetwork;
    }
}
