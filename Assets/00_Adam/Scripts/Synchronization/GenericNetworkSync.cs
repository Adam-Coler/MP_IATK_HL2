using Photon.Pun;
using UnityEngine;

namespace Photon_IATK
{
    public class GenericNetworkSync : MonoBehaviourPun
    {
        [SerializeField] private bool isUser = default;

        private Camera mainCamera;

        private Vector3 startingLocalPosition;
        private Quaternion startingLocalRotation;


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
        }

        // private void FixedUpdate()
        private void Update()
        {
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
