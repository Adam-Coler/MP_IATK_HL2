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
            PhotonView localView = glppv.getLocalPlayer();
            if (localView != null)
            {
                glppv.getLocalPlayer().RPC("setNickname", RpcTarget.All);
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
        public PhotonView getLocalPlayer()
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
                        return photon;
                    }
                }

            }

            Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "NO LOCAL PHOTON VIEW FOUND", Time.realtimeSinceStartup, "Static: Pun_Player_RPC_Calls", this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            return null;
        }


    }

}