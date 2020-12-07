using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

namespace Photon_IATK
{
    [RequireComponent(typeof(TMPro.TMP_InputField))]
    public class Photon_Set_Nickname : MonoBehaviour
    {


        public TMPro.TMP_InputField InputFeild;

        const string playerNamePrefKey = "PlayerName";

        // Start is called before the first frame update
        void Start()
        {
            string defaultName = string.Empty;
            if (InputFeild != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    InputFeild.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

    }




}
