using Photon.Pun;
using UnityEngine;

namespace Photon_IATK
{
    public class GenericNetworkSyncTrackedDevice : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] public bool isUser = default;

        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;

        private Vector3 startingLocalPosition;
        private Quaternion startingLocalRotation;

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.localPosition);
                stream.SendNext(transform.localRotation);
            }
            else
            {
                networkLocalPosition = (Vector3)stream.ReceiveNext();
                networkLocalRotation = (Quaternion)stream.ReceiveNext();
            }
        }

        private void Start()
        {

            if (PlayspaceAnchor.Instance != null)
            {
                transform.parent = FindObjectOfType<PlayspaceAnchor>().transform;
                Debug.Log(GlobalVariables.green + "Parenting: " + this.gameObject.name + " in " + transform.parent.name + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
            }
            else
            {
                Debug.Log(GlobalVariables.red + "No Playspace anchor exists, nothing parented" + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
            }

            if (isUser)
            {
                if (photonView.IsMine)
                {
                    Debug.Log(GlobalVariables.green + "Setting GenericNetworkManager.Instance.localUser " + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());

                    GenericNetworkManager.Instance.localUser = photonView;
                }
            }
            else
            {
                Debug.Log(GlobalVariables.red + "isUser set to false, not setting the view or the parent on this client " + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
            }


            var trans = transform;
            startingLocalPosition = trans.localPosition;
            startingLocalRotation = trans.localRotation;

            networkLocalPosition = startingLocalPosition;
            networkLocalRotation = startingLocalRotation;
        }

        // private void FixedUpdate()
        private void FixedUpdate()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                // if not the local user

                //get PhotonUser transform 
                var trans = transform;

                //move the users local position to the network position
                // the network position is the information sent from the other users local poisitons
                trans.localPosition = networkLocalPosition;
                trans.localRotation = networkLocalRotation;
            }
        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.blue + "Destorying Object" + GlobalVariables.endColor + ", OnDestroy() : " + this.GetType(), this.gameObject.name);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
