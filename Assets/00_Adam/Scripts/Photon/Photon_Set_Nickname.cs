using UnityEngine;
using Photon.Pun;


namespace Photon_IATK
{
    [RequireComponent(typeof(TMPro.TMP_InputField))]
    public class Photon_Set_Nickname : MonoBehaviour
    {


        public TMPro.TMP_InputField InputFeild;

        static string playerNamePrefKey = GlobalVariables.PlayerPrefsKeys.ParticipantID.ToString();

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

        public void SetPlayerName()
        {
            string value = InputFeild.text;
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.Log(GlobalVariables.red + "Nickname Null" + GlobalVariables.endColor + " : " + "SetPlayerName()" + " : " + this.GetType());
                return;
            }

            PhotonNetwork.NickName = value;
            Debug.Log(GlobalVariables.green + "NicknameSet: " + value + GlobalVariables.endColor + " : " + "SetPlayerName()" + " : " + this.GetType());

            PlayerPrefs.SetString(playerNamePrefKey, value);

            if (PhotonNetwork.IsConnected)
            {
                Pun_RPC_Calls.rpc_setNickName();
                Debug.Log(GlobalVariables.green + "Nickname PunRPC Called " + GlobalVariables.endColor + " : " + "SetPlayerName()" + " : " + this.GetType());
            }
        }
    }
}
