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

            removeComponent<PhotonView>();
            removeComponent<PhotonAnimatorView>();
            removeComponent<PhotonTransformView>();
            removeComponent<RandomizePhotonViewID>();
        }

        private void removeComponent<T>() where T : Component
        {
            T component = this.gameObject.GetComponent<T>();
            if (component == null) { return; }

            Debug.LogFormat(GlobalVariables.cOnDestory + "{0}{1}{2}." + GlobalVariables.endColor + " {3}: {4} -> {5} -> {6}", "Destorying ", component, "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            Destroy(component);

        }
    }
}
