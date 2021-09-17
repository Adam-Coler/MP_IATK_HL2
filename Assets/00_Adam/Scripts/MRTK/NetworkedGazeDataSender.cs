using Photon.Pun;
using UnityEngine;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Photon_IATK
{
    public class NetworkedGazeDataSender : MonoBehaviourPun, IPunObservable
    {

        public Material beamColor;
        public bool isBeam = true;
        private GameObject beam;


        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (PlayspaceAnchor.Instance == null) { return; }

                Vector3 orgin = CoreServices.InputSystem.GazeProvider.GazeOrigin;
                Vector3 direction = CoreServices.InputSystem.GazeProvider.GazeDirection;

                Vector3 newPoint1 = PlayspaceAnchor.Instance.transform.InverseTransformPoint(orgin);
                Vector3 newPoint =  PlayspaceAnchor.Instance.transform.InverseTransformPoint(orgin + (direction * .1f));

                stream.SendNext(newPoint1);
                stream.SendNext(newPoint);
            }
            else
            {
                Vector3 orgin = (Vector3)stream.ReceiveNext();
                Vector3 orginPlus = (Vector3)stream.ReceiveNext();
                updateBeam(orgin, orginPlus);
            }
        }

        private void Awake()
        {
            setUp();
        }

        public void setUp()
        {
            HelperFunctions.ParentInSharedPlayspaceAnchor(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());

            if (beam != null) { Destroy(beam); }

            if (isBeam)
            {

                beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                beam.name = "Gaze beam";
                beam.transform.localScale = new Vector3(.005f, 1f, .005f);
                beam.transform.parent = this.transform;

                Renderer rend = beam.GetComponent<Renderer>();
                if (rend != null && beamColor != null)
                {
                    rend.material = beamColor;
                }

                Collider col = beam.GetComponent<Collider>();
                if (col != null && beamColor != null)
                {
                    Destroy(col);
                }

            }
        }

        //void UpdateBeam()
        //{
        //    if (beam != null)
        //    {
        //        if (PlayspaceAnchor.Instance == null) { return; }

        //        Vector3 orgin = CoreServices.InputSystem.GazeProvider.GazeOrigin;
        //        Vector3 direction = CoreServices.InputSystem.GazeProvider.GazeDirection;


        //        Vector3 newPoint1 = PlayspaceAnchor.Instance.transform.InverseTransformPoint(orgin);
        //        Vector3 newPoint = PlayspaceAnchor.Instance.transform.InverseTransformPoint(orgin + (direction * .1f));

        //        updateBeam(newPoint, newPoint1);
        //    } else
        //    {
        //        setUp();
        //    }

        //}

        private void updateBeam(Vector3 orgin, Vector3 orginPlus)
        {
            //Debug.Log("reciving beam: " + orgin + " to " + orginPlus);
            if (beam == null) { return; }

            //Debug.Log("making beam: " + orgin + " to " + orginPlus);
            Vector3 dir = orginPlus - orgin;

            Quaternion q = Quaternion.FromToRotation(transform.up, dir);
            //Quaternion rot = q * beam.transform.rotation;

            beam.transform.position = orgin + (beam.transform.localScale.y * 1 * dir.normalized);
            beam.transform.rotation = q;
        }



    }
}
