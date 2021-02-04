using UnityEngine;
using Photon.Pun;


namespace Photon_IATK
{
    //Pun calls for the player class only
    public class Pun_Player_RPC_Calls
    {
        public static void rpc_setNickName()
        {
            PhotonView localView;
            if (HelperFunctions.getLocalPlayer(out localView, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                localView.RPC("setNickname", RpcTarget.All);
            }
        }

        public static void rpc_showHideControllerModels()
        {
            PhotonView localView;
            if (HelperFunctions.getLocalPlayer(out localView, System.Reflection.MethodBase.GetCurrentMethod()))
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

}