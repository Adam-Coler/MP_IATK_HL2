using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


namespace Photon_IATK { 
    public class Room : MonoBehaviourPunCallbacks, IInRoomCallbacks

    { 
    public static Room _Room;
    [SerializeField] private GameObject photonUserPrefab = default;

    private Player[] photonPlayers;
    private int playersInRoom;
    private int myNumberInRoom;
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        }

    private void Awake()
    {
        if (_Room == null)
        {
            _Room = this;
        }
        else
        {
            if (_Room != this)
            {
                Destroy(_Room.gameObject);
                _Room = this;
            }
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        // Allow prefabs not in a Resources folder
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            if (photonUserPrefab != null) pool.ResourceCache.Add(photonUserPrefab.name, photonUserPrefab);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        StartGame();
    }

    private void StartGame()
    {
        CreatePlayer();
        if (!PhotonNetwork.IsMasterClient) return;
    }

    private void CreatePlayer()
    {

            Debug.Log(GlobalVariables.green + "Photon Instantisateing the player" + GlobalVariables.endColor + " : " + "CreatePlayer()" + " : " + this.GetType());


            var player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);

            Debug.Log(GlobalVariables.green + "Photon Instantisateing the Room Orgin Tracker" + GlobalVariables.endColor + " : " + "CreatePlayer()" + " : " + this.GetType());

            var tracker = PhotonNetwork.Instantiate("Tracker", Vector3.zero, Quaternion.identity);
            tracker.name = "Room Orgin";

            Debug.Log(GlobalVariables.green + "Photon Instantisateing the Finger Tracker" + GlobalVariables.endColor + " : " + "CreatePlayer()" + " : " + this.GetType());
            this.gameObject.AddComponent<TrackerToFinger>();

        }

        #region CUSTOM



#if DESKTOP


#elif HL2


#elif VIVE

#else

#endif
        #endregion

    }
}
