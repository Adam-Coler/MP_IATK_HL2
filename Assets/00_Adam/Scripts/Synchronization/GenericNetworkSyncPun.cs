using Photon.Pun;
using UnityEngine;

namespace Photon_IATK
{
    /// <summary>
    /// This class might update position better than events on tracked contorllers where object ownsership is not a concern
    /// </summary>
    public class GenericNetworkSyncPun : MonoBehaviourPun, IPunObservable
    {

        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (networkLocalPosition == transform.localPosition || networkLocalRotation == transform.localRotation) { return; }

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

            HelperFunctions.ParentInSharedPlayspaceAnchor(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());

            var trans = transform;
            trans.localPosition = networkLocalPosition;
            trans.localRotation = networkLocalRotation;
        }

        // private void FixedUpdate()
        private void Update()
        {
            if (!photonView.IsMine)
            {
                var trans = transform;
                trans.localPosition = networkLocalPosition;
                trans.localRotation = networkLocalRotation;
            }
        }
    }
}
