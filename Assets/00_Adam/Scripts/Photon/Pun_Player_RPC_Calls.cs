using UnityEngine;
using Photon.Pun;


namespace Photon_IATK
{
    //Pun calls for the player class only
    public class Pun_Player_RPC_Calls
    {
        public static void rpc_setNickName()
        {
            getLocalPlayerPhotonView glppv = new getLocalPlayerPhotonView();
            PhotonView localView;
            if (glppv.getLocalPlayer(out localView))
            {
                localView.RPC("setNickname", RpcTarget.All);
            }
        }

        public static void rpc_showHideControllerModels()
        {
            getLocalPlayerPhotonView glppv = new getLocalPlayerPhotonView();
            PhotonView localView;
            if (glppv.getLocalPlayer(out localView))
            {
                localView.RPC("showHideControllerModels", RpcTarget.All);
            }

            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Photon_Player photon_Player;
                if (HelperFunctions.GetComponent<Photon_Player>(out photon_Player, System.Reflection.MethodBase.GetCurrentMethod()))
                {
                    photon_Player.showHideControllerModels();
                }
            }
        }

        public static bool isLocal()
        {
            bool isLocal = false;

            if (PhotonNetwork.LocalPlayer.UserId == "1") {
                isLocal = true;
            }

            return isLocal;
        }
    }


    class getLocalPlayerPhotonView
    {
        public bool getLocalPlayer(out PhotonView photonView)
        {
            // Start is called before the first frame update
            var tmp = (GameObject.FindGameObjectsWithTag("Player"));
            foreach (GameObject obj in tmp)
            {
                PhotonView photon = obj.GetComponent<PhotonView>();
                Photon_Player photonPlayer = obj.GetComponent<Photon_Player>();
                Debug.Log(photon.name);
                if (photon != null & photonPlayer != null)
                {
                    if (photon.IsMine)
                    {
                        photonView = photon;
                        return true;
                    }
                }

            }

            Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "NO LOCAL PHOTON VIEW FOUND", Time.realtimeSinceStartup, "Static: Pun_Player_RPC_Calls", this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            photonView = null;
            return false;
        }


    }

}