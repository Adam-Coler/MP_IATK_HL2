using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK { 

    public class RandomizePhotonViewID : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Awake()
        {
            Debug.LogFormat(GlobalVariables.green + "Photon View Tracked Loaded on {0}" + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType(), this.gameObject.name);

            Photon.Pun.PhotonView myView = this.GetComponent<Photon.Pun.PhotonView>();

            if (myView != null)
            {
                myView.ViewID = Mathf.RoundToInt(Random.value * 2147483647);
                Debug.LogFormat(GlobalVariables.green + "Photon View ID set: {0}, on {1} " + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType(), myView.ViewID, this.gameObject.name);
            }

        }
    }
}

