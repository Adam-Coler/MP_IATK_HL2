using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK { 

    public class RandomizePhotonViewID : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Awake()
        {
            Photon.Pun.PhotonView myView = this.GetComponent<Photon.Pun.PhotonView>();

            if (myView != null)
            {
                myView.ViewID = Mathf.RoundToInt(Random.value * 2147483647);
                Debug.LogFormat(GlobalVariables.green + "Photon View ID set: {0} " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType(), myView.ViewID);
            }

        }
    }
}

