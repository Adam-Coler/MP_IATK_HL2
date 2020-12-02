using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Photon_IATK
{
    public class btnFunctions : MonoBehaviour
    {
        // cached transform of the target
        private int currentView = 0;
        GameObject localPlayer;
        Photon_Player localPhoton;
        public TMPro.TextMeshProUGUI FollowedPlayerLog;

        public void exitGame()
        {
            Debug.Log(GlobalVariables.red + "Exiting Game..." + GlobalVariables.endColor + " : " + this.GetType());
        #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
                 Application.OpenURL(webplayerQuitURL);
        #else
                 Application.Quit();
        #endif
        }

        public void localCamera()
        {
            if (localPlayer == null)
            {
                setLocalPlayer();
            }
            Debug.Log(GlobalVariables.green + "Local Camera" + GlobalVariables.endColor + " : " + this.GetType());

            localPhoton._Cammera_Manager.trackedObj = localPlayer;
            FollowedPlayerLog.text = GlobalVariables.green + "Cammera Target: " + localPlayer.GetPhotonView().Owner.NickName + GlobalVariables.endColor;
        }

        public void nextCamera()
        {
            if (localPlayer == null)
            {
                setLocalPlayer();
            }
            Debug.Log(GlobalVariables.green + "Next Camera" + GlobalVariables.endColor + " : " + this.GetType());
            currentView += 1;
            logFoundObjects();
        }

        public void previousCamera()
        {
            if (localPlayer == null)
            {
                setLocalPlayer();
            }
            Debug.Log(GlobalVariables.green + "Previous Camera" + GlobalVariables.endColor + " : " + this.GetType());
            currentView -= 1;
            logFoundObjects();
        }

        private void logFoundObjects()
        {
            //Debug.Log("Searching..." + this.GetType().ToString());
            var tmp = (GameObject.FindGameObjectsWithTag("Player"));
            //Debug.Log("Found " + tmp.Length + " players, " + this.GetType().ToString());

            if (currentView > tmp.Length - 1)
            {
                currentView = 0;
            } else if (currentView < 0)
            {
                currentView = tmp.Length - 1;
            }

            PhotonView photon = tmp[currentView].GetComponent<PhotonView>();
            if (photon != null)
            {
                FollowedPlayerLog.text = GlobalVariables.green + "Cammera Target: " + photon.Owner.NickName + GlobalVariables.endColor;
                localPhoton._Cammera_Manager.trackedObj = tmp[currentView];
            }
        }

        private void setLocalPlayer()
        {
            Debug.Log(GlobalVariables.red + "Attempting to assign Local Player..." + GlobalVariables.endColor + " : " + this.GetType().ToString());
            var tmp = (GameObject.FindGameObjectsWithTag("Player"));
            foreach(GameObject obj in tmp)
            {
                PhotonView photon = obj.GetComponent<PhotonView>();
                Photon_Player photonPlayer = obj.GetComponent<Photon_Player>();
                Debug.Log(photon.name);
                if (photon != null & photonPlayer != null)
                {
                    if (photon.IsMine)
                    {
                        Debug.Log(GlobalVariables.red + "Local Player Assigned" + GlobalVariables.endColor + " : " + this.GetType().ToString());
                        localPlayer = obj;
                        localPhoton = photonPlayer;
                        return;
                    }
                }

            }
            Debug.Log(GlobalVariables.red + "No Local Player Assigned!!!" + GlobalVariables.endColor + " : " + this.GetType().ToString());
        }

    }
}
