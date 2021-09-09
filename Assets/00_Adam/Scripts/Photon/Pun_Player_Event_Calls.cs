using UnityEngine;
using Photon.Pun;


namespace Photon_IATK
{
    //Pun calls for the player class only
    public class Pun_Player_Event_Calls
    {
        public static void Event_setNickName()
        {
            PhotonView photonView;
            Photon_Player photon_Player;
            if (HelperFunctions.getLocalPlayer(out photonView, out photon_Player, System.Reflection.MethodBase.GetCurrentMethod())) { photon_Player.RequestNicknameChangeEvent(); }
        }

        public static void Event_showHideControllerModels()
        {
            PhotonView photonView;
            Photon_Player photon_Player;
            if (HelperFunctions.getLocalPlayer(out photonView, out photon_Player, System.Reflection.MethodBase.GetCurrentMethod())) { photon_Player.RequestHideControllerModelsEvent(); }
        }

        public static void Event_HideExtras()
        {
            PhotonView photonView;
            Photon_Player photon_Player;
            if (HelperFunctions.getLocalPlayer(out photonView, out photon_Player, System.Reflection.MethodBase.GetCurrentMethod())) { photon_Player.RequestHideExtrasEvent(); }
        }

    }
}