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

                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Parenting in playspaceAnchor", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "No Playspace anchor exists, nothing parented", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

            if (isUser)
            {
                if (photonView.IsMine)
                {
                    Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Setting GenericNetworkManager.Instance.localUser", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    GenericNetworkManager.Instance.localUser = photonView;
                }
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "isUser set to false, not setting the view or the parent on this client", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
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
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
  
            Debug.LogFormat(GlobalVariables.cOnDestory + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Destorying Object", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }
    }
}
