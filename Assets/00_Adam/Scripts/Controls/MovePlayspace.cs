using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

namespace Photon_IATK
{
    public class MovePlayspace : MonoBehaviourPunCallbacks
    {
        //trigger sending values
        //trigger reciving values
        //trigger update

        //get connected people
        //add new people

        public TMP_Dropdown playerDropDown;

        private Player[] players;

        private void Awake()
        {
            getplayers();
        }

        private void getplayers()
        {
            players = PhotonNetwork.PlayerList;
            setAxisDropdowns();
        }

        private void clearDropdownOptions()
        {
            playerDropDown.ClearOptions();
        }

        private void setAxisDropdowns()
        {
            clearDropdownOptions();

            List<TMP_Dropdown.OptionData> listDataDimensions = new List<TMP_Dropdown.OptionData>();

            foreach (Player player in players)
            {
                listDataDimensions.Add(new TMP_Dropdown.OptionData() { text = player.NickName });
            }

            playerDropDown.AddOptions(listDataDimensions);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            getplayers();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            getplayers();
        }

    }
}
