using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace Photon_IATK
{

    public class NetworkedLoadVisualisation : MonoBehaviour
    {
        //public void InitializeVisualisation()
        //{
        //    if (PhotonNetwork.IsConnected)
        //    {
        //        PhotonNetwork.Instantiate("VisualisationEmpty", Vector3.zero, Quaternion.identity);
        //        Debug.Log(GlobalVariables.yellow + "Instantiateing IATK scatterplot" + GlobalVariables.endColor + " : " + "InitializeVisualisation()" + " : " + this.GetType());
        //    } else
        //    {
        //        PhotonNetwork.Instantiate("VisualisationEmpty", Vector3.zero, Quaternion.identity);

        //        Debug.Log(GlobalVariables.red + "Not connected to Photon, nothing instantiated" + GlobalVariables.endColor + " : " + "InitializeVisualisation()" + " : " + this.GetType());
        //    }
        //}

        public bool isAutoLoad = false;

        public void InitializeVisualisationProgramatically()
        {

#if VIVE
            if (PhotonNetwork.IsConnected)
            {
                GameObject vis = PhotonNetwork.Instantiate("Vis", Vector3.zero, Quaternion.identity);

                attachToPlayspace(vis);

                Debug.Log(GlobalVariables.yellow + "Instantiateing IATK scatterplot" + GlobalVariables.endColor + " : " + "InitializeVisualisationProgramatically()" + " : " + this.GetType());
            }
            else
            {
                Debug.Log(GlobalVariables.red + "Not connected to Photon, nothing instantiated" + GlobalVariables.endColor + " : " + "InitializeVisualisationProgramatically()" + " : " + this.GetType());
            }
#endif
        }

        private void attachToPlayspace(GameObject obj)
        {
            if (PlayspaceAnchor.Instance != null)
            {
                obj.transform.SetParent(PlayspaceAnchor.Instance.transform);
                Debug.LogFormat(GlobalVariables.green + "Attaching {0} to the playspace anchor " + GlobalVariables.endColor + " : " + "attachToPlayspace()" + " : " + this.GetType(), obj.gameObject.name);

                obj.gameObject.transform.localPosition = Vector3.zero;
                obj.gameObject.transform.localRotation = Quaternion.identity;

                //obj.AddComponent<MakeVisualisation>();
            }
            else
            {
                Debug.LogFormat(GlobalVariables.red + "Can't attach {0} to the playspace anchor, No anchor found " + GlobalVariables.endColor + " : " + "attachToPlayspace()" + " : " + this.GetType(), obj.gameObject.name);
            }
        }

        private void Awake()
        {

            if (!isAutoLoad) { return; }

            Debug.Log(GlobalVariables.yellow + "Loading Visualisation in 4 seconds" + GlobalVariables.endColor + " : " + "Awake()" + " : " + this.GetType());

            Invoke("InitializeVisualisationProgramatically", 4);

            //https://github.com/MaximeCordeil/IATK
        }

    }
}
