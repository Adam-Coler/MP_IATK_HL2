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
                Debug.Log(GlobalVariables.green + "Setting GenericNetworkManager.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());
                Instance = this;
            }
            else
            {
                if (Instance != this)
                {
                    Debug.Log(GlobalVariables.green + "Destroying then setting GenericNetworkManager.Instance " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());

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
