using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Utilities;

namespace Photon_IATK
{

    public class SpawnTextInputPhoton : MonoBehaviourPunCallbacks
    {
        public GameObject myPrefab;
        public void spawnTextInput()
        {
            if (PhotonNetwork.IsConnected)
                {
                if (!photonView.IsMine)
                {
                    PhotonNetwork.Instantiate("MRTK_InputFeild", new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    Debug.Log(GlobalVariables.green + "Connected, Spawning a text input feild" + GlobalVariables.endColor + " : " + this.GetType());
                }
             } else
            {
                Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            }
        }

    }
}
