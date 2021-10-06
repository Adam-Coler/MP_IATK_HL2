using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using System.Globalization;
using ExitGames.Client.Photon;

namespace Photon_IATK
{
    public class MovePlayspace : MonoBehaviour
    {
        //trigger sending values
        //trigger reciving values
        //trigger update

        public TMP_Dropdown playerDropDown;

        public TMP_InputField posX_TMP;
        public TMP_InputField posY_TMP;
        public TMP_InputField posZ_TMP;
        public TMP_InputField rotX_TMP;
        public TMP_InputField rotY_TMP;
        public TMP_InputField rotZ_TMP;

        public TMP_InputField nickname;

        public MovePlayspaceInterface mpi;

        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;

        private Player[] players;

        private void OnEnable()
        {
            mpi = MovePlayspaceInterface.Instance;

            if (mpi == null)
            {
                Destroy(this.gameObject);
            }

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

            listDataDimensions.Add(new TMP_Dropdown.OptionData() { text = "Undefined" });
            listDataDimensions.Add(new TMP_Dropdown.OptionData() { text = "FindPlayers" });

            if (players != null) {
                foreach (Player player in players)
                {
                    listDataDimensions.Add(new TMP_Dropdown.OptionData() { text = player.NickName });
                }
            }


            playerDropDown.AddOptions(listDataDimensions);
        }

        public void setPlayerName()
        {
            Debug.Log("button pressed");
            if (playerDropDown.options[playerDropDown.value].text == "Undefined")
            {
                getplayers();
            }
            else if (playerDropDown.options[playerDropDown.value].text == "FindPlayers")
            {

            }
            else
            {
                mpi.requestPlayerNameChange(players[playerDropDown.value - 2], nickname.text);
                nickname.text = "";
                getplayers();
            }

        }

        public void getplayers()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GetPlayers called{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "", "", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            players = PhotonNetwork.PlayerList;
            setAxisDropdowns();
        }

        public void dropDownChanged()
        {
            if (playerDropDown.options[playerDropDown.value].text == "Undefined")
            {

            } else if (playerDropDown.options[playerDropDown.value].text == "FindPlayers")
            {
                getplayers();
            } else
            {
                mpi.requestPlayspaceTransform(players[playerDropDown.value - 2]);
            }
        }

        public void requestTrackerUpdate()
        {
            Debug.Log("button pressed");
            if (playerDropDown.options[playerDropDown.value].text == "Undefined")
            {
                getplayers();
            }
            else if (playerDropDown.options[playerDropDown.value].text == "FindPlayers")
            {

            }
            else
            {
                mpi.requestTrackerUpdate(players[playerDropDown.value - 2]);
            }
        }

        public void updateValues()
        {
            float.TryParse(posX_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out posX);
            float.TryParse(posY_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out posY);
            float.TryParse(posZ_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out posZ);
            float.TryParse(rotX_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out rotX);
            float.TryParse(rotY_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out rotY);
            float.TryParse(rotZ_TMP.text, NumberStyles.Any, CultureInfo.CurrentCulture, out rotZ);

            if (playerDropDown.options.Count >= 3) {
                string requestedID = players[playerDropDown.value - 2].UserId;
                mpi.SendUpdatePlayspaceTransform(posX, posY, posZ, rotX, rotY, rotZ, players[playerDropDown.value - 2]);

                Debug.LogFormat(GlobalVariables.cCommon + "Values updated, Position: X {0}, Y {1}, Z {2}. Rotation: X {3}, Y {4}, Z {5}, Requsted ID: {6}, Requestor ID: {7}." + GlobalVariables.endColor + " {8}: {9} -> {10} -> {11}", posX, posY, posZ, rotX, rotY, rotZ, requestedID, PhotonNetwork.LocalPlayer.UserId, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


            }



            Debug.LogFormat(GlobalVariables.cCommon + "Values updated, Position: X {0}, Y {1}, Z {2}. Rotation: X {3}, Y {4}, Z {5}, Requsted ID: {6}, Requestor ID: {7}." + GlobalVariables.endColor + " {8}: {9} -> {10} -> {11}", posX, posY, posZ, rotX, rotY, rotZ, "Null", PhotonNetwork.LocalPlayer.UserId, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


        }

    }
}
