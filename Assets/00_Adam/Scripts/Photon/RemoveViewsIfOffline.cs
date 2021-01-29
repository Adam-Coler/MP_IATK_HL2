using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{
    public class RemoveViewsIfOffline : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            if (PhotonNetwork.IsConnected) { return; }
            //removes photon views if not connected

            HelperFunctions.RemoveComponent<PhotonView>(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());
            HelperFunctions.RemoveComponent<PhotonAnimatorView>(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());
            HelperFunctions.RemoveComponent<PhotonTransformView>(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());
            HelperFunctions.RemoveComponent<RandomizePhotonViewID>(this.gameObject, System.Reflection.MethodBase.GetCurrentMethod());
        }
        
    }
}
