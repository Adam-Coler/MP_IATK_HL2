using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Reflection;

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

            if (PhotonNetwork.IsConnected)
            {
                GameObject vis = PhotonNetwork.Instantiate("Vis", Vector3.zero, Quaternion.identity);

                attachToPlayspace(vis);

                Debug.LogFormat(GlobalVariables.yellow + "Instantiateing IATK scatterplot" + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
            }
            else
            {
                Debug.LogFormat(GlobalVariables.red + "Not connected to Photon, nothing instantiated" + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
            }
        }

        private void attachToPlayspace(GameObject obj)
        {
            if (PlayspaceAnchor.Instance != null)
            {
                obj.transform.SetParent(PlayspaceAnchor.Instance.transform);
                Debug.LogFormat(GlobalVariables.green + "Attaching to the playspace anchor" + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());

                obj.gameObject.transform.localPosition = Vector3.zero;
                obj.gameObject.transform.localRotation = Quaternion.identity;

                //obj.AddComponent<MakeVisualisation>();
            }
            else
            {
                Debug.LogFormat(GlobalVariables.red + "Can't attach to the playspace anchor, No anchor found" + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
            }
        }

        private void Awake()
        {
            if (!isAutoLoad) { return; }
            LoadVis();
        }

        public void LoadVis()
        {


            if (FindObjectOfType<IATK.Visualisation>() == null)
            {
                Debug.LogFormat(GlobalVariables.yellow + "Loading Visualisation in 3 seconds" + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());

                Invoke("InitializeVisualisationProgramatically", 3);
            }
            else
            {
                Debug.LogFormat(GlobalVariables.red + "A Visualisation already exists, nothing loaded" + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), MethodBase.GetCurrentMethod());
                return;
            }

        }

    }
}
