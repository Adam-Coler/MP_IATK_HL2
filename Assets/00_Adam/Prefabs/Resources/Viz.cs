

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


namespace Photon_IATK
{
    public class Viz : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        //[SerializeField] private GameObject viz_1;
   
        //public static Viz _Viz;

        /*void Awake()
        {
            if (_Viz == null)
            {
                _Viz = this;
            }
            else
            {
                if (_Viz != this)
                {
                    Destroy(_Viz.gameObject);
                    _Viz = this;
                }
            }
        }*/

        private void Start()
        {
            //Debug.Log(GlobalVariables.blue + "VIZ: Start() " + GlobalVariables.endColor);
            //if (PhotonNetwork.IsConnected)
            //{
            //    Debug.Log("Viz: Start() connected");
            //}
            //else
            //{
            //    Debug.Log(GlobalVariables.blue + "Viz: Start() connecting using settings" + GlobalVariables.endColor);
            //    PhotonNetwork.ConnectUsingSettings();
            //}
        }

        //void Start()
        public override void OnJoinedRoom()
        {
            //Debug.Log("Viz: OnJoinRoom() success");
            //base.OnJoinedRoom();
            //vizFinal();
        }

        public void vizFinal() {
            if (!PhotonNetwork.IsMasterClient) { return; }

            if (true)
            {
                Debug.Log(GlobalVariables.red + "Viz: " + GlobalVariables.endColor + GlobalVariables.yellow + "Master is connected and instaciating prefab to Photon network" + GlobalVariables.endColor);
                //PhotonNetwork.Instantiate("viz_1", new Vector3(-.1f, -.1f, .5f), Quaternion.identity, 1);

                PhotonNetwork.Instantiate("viz_1", new Vector3(-.1f, -.1f, .5f), Quaternion.identity);
            }
            else
            {
                Debug.Log(GlobalVariables.red + "Viz: " + GlobalVariables.endColor + GlobalVariables.yellow + "Non-master player has entered room" + GlobalVariables.endColor);
            }
        }
    }
}
