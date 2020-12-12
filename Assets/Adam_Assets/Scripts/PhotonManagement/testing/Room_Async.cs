using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


namespace Photon_IATK { 
    public class Room_Async : MonoBehaviourPunCallbacks, IInRoomCallbacks

    { 
    public static Room_Async Room;

    [SerializeField] private GameObject photonUserPrefab = default;

    // private PhotonView pv;
    private Player[] photonPlayers;
    private int playersInRoom;
    private int myNumberInRoom;
    
    // private GameObject module;
    // private Vector3 moduleLocation = Vector3.zero;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        }

    private void Awake()
    {
        if (Room == null)
        {
            Room = this;
        }
        else
        {
            if (Room != this)
            {
                Destroy(Room.gameObject);
                Room = this;
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
        PhotonNetwork.NickName = myNumberInRoom.ToString();

        StartGame();
    }

    private void StartGame()
    {
        CreatePlayer();
        if (!PhotonNetwork.IsMasterClient) return;
    }

    private void CreatePlayer()
    {
        var player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);
        
        var tracker = PhotonNetwork.Instantiate("Tracker", Vector3.zero, Quaternion.identity);
            //Microsoft.MixedReality.Toolkit.MixedRealityPlayspace.AddChild(tracker.transform);
        }

        // private void CreateMainLunarModule()
        // {
        //     module = PhotonNetwork.Instantiate(roverExplorerPrefab.name, Vector3.zero, Quaternion.identity);
        //     pv.RPC("Rpc_SetModuleParent", RpcTarget.AllBuffered);
        // }
        //
        // [PunRPC]
        // private void Rpc_SetModuleParent()
        // {
        //     Debug.Log("Rpc_SetModuleParent- RPC Called");
        //     module.transform.parent = TableAnchor.Instance.transform;
        //     module.transform.localPosition = moduleLocation;
        // }

        #region CUSTOM



#if DESKTOP


#elif HL2


#elif VIVE

#else

#endif
        #endregion

    }
}
