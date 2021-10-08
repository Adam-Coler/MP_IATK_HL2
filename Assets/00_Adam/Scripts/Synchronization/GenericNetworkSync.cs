using Photon.Pun;
using UnityEngine;

namespace Photon_IATK
{
    public class GenericNetworkSync : MonoBehaviourPun //, IPunObservable
    {
        [SerializeField] private bool isUser = default;

        private Camera mainCamera;

        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;

        private Vector3 startingLocalPosition;
        private Quaternion startingLocalRotation;

        //void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    if (stream.IsWriting)
        //    {
        //        if (networkLocalPosition == transform.localPosition || networkLocalRotation == transform.localRotation) { return; };

        //        stream.SendNext(transform.localPosition);
        //        stream.SendNext(transform.localRotation);
        //    }
        //    else
        //    {
        //        networkLocalPosition = (Vector3)stream.ReceiveNext();
        //        networkLocalRotation = (Quaternion)stream.ReceiveNext();
        //    }
        //}

        private void Start()
        {
            mainCamera = Camera.main;

            if (isUser)
            {
                //parent the object to the table anchor
                //This script is attached to the PhotonPlayer

                //if there is an anchor attach the parent of the photonuser to it
                if (PlayspaceAnchor.Instance != null)
                {
                    transform.parent = FindObjectOfType<PlayspaceAnchor>().transform;
                    Debug.Log(GlobalVariables.green + "Parenting: " + this.gameObject.name + " in " + transform.parent.name + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
                } else
                {
                    Debug.Log(GlobalVariables.red + "No Playspace anchor exists, nothing parented" + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());
                }


                if (photonView.IsMine)
                {
                    Debug.Log(GlobalVariables.green + "Setting GenericNetworkManager.Instance.localUser " + GlobalVariables.endColor + " : " + "Start()" + " : " + this.GetType());

                    GenericNetworkManager.Instance.localUser = photonView;
                }
            } else
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
        private void Update()
        {
            //if (!photonView.IsMine)
            //{
            //    // if not the local user

            //    //get PhotonUser transform 
            //    var trans = transform;

            //    //move the users local position to the network position
            //    // the network position is the information sent from the other users local poisitons
            //    trans.localPosition = networkLocalPosition;
            //    trans.localRotation = networkLocalRotation;
            //}

            if (photonView.IsMine && isUser)
            {

                //if this is the local user, move them to the camera location
                //the camera is the playspace
                var trans = transform;
                var mainCameraTransform = mainCamera.transform;
                trans.position = mainCameraTransform.position;
                trans.rotation = mainCameraTransform.rotation;
            }
        }
    }
}
