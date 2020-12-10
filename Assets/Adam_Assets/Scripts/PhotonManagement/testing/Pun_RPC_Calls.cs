using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;



namespace Photon_IATK
{
    public class Pun_RPC_Calls
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
            Debug.LogError("NO LOCAL PHOTON VIEW FOUND");
            return null;
        }


    }

}