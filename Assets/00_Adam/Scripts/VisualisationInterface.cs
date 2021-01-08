using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace Photon_IATK
{

    public class VisualisationInterface : MonoBehaviourPun, IPunObservable
    {

        private IATK.Visualisation visualisation;

        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;


        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.localPosition);
                stream.SendNext(transform.localRotation);
                //stream.SendNext(this.gameObject.transform.InverseTransformDirection(NewPoint));
            }
            else
            {
                networkLocalPosition = (Vector3)stream.ReceiveNext();
                networkLocalRotation = (Quaternion)stream.ReceiveNext();
            }
        }
        private void FixedUpdate()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                var trans = transform;

                trans.localPosition = networkLocalPosition;
                trans.localRotation = networkLocalRotation;

            }
        }

        void Awake()
        {
            //attach to playspace ancor
            attachToPlayspace();

            //rename
            this.gameObject.name = "Networked IATK Visualisation from visualisationInterface.cs";

            //set location
            this.gameObject.transform.localPosition = Vector3.zero;
            this.gameObject.transform.localRotation = Quaternion.identity;

            //get visualisation script
            getVisualisationScript();
        }

        private void attachToPlayspace()
        {
            if (PlayspaceAnchor.Instance != null)
            {
                this.transform.SetParent(PlayspaceAnchor.Instance.transform);
                Debug.Log(GlobalVariables.green + "Attaching visualisation to the playspace anchor " + GlobalVariables.endColor + " : " + "attachToPlayspace()" + " : " + this.GetType());
            } else
            {
                Debug.Log(GlobalVariables.red + "No playspace anchor found" + GlobalVariables.endColor + " : " + "attachToPlayspace()" + " : " + this.GetType());
            }
        }

        private void getVisualisationScript()
        {
            visualisation = this.gameObject.GetComponent<IATK.Visualisation>();
            if (visualisation == null)
            {
                Debug.Log(GlobalVariables.red + "No visualisation script found " + GlobalVariables.endColor + " : " + "getVisualisationScript()" + " : " + this.GetType());
            }
        }

    }
}
