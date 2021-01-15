using UnityEngine;
using Photon.Pun;

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

        private GameObject instancedVis;
        private bool isLoadedOffline = false;

        public GameObject Prefab;

        public void InitializeVisualisationProgramatically()
        {

            if (PhotonNetwork.IsConnected)
            {
                Debug.LogFormat(GlobalVariables.cInstance + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Instantiateing IATK scatterplot online", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                instancedVis = PhotonNetwork.Instantiate("Vis", Vector3.zero, Quaternion.identity);
                instancedVis.gameObject.name = "ScatterplotVis_Online";

                setInstanceLoadType(false);
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Not connected to Photon, loading offline", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                instancedVis = Instantiate(Prefab, new Vector3(1.5f, 0, 0), Quaternion.identity);
                instancedVis.gameObject.name = "ScatterplotVis_Offline";

                setInstanceLoadType(true);
            }
        }

        private void setInstanceLoadType(bool _isLoadedOffline)
        {
            InstanceVis instanceComponenet = instancedVis.GetComponent<InstanceVis>() as InstanceVis;

            if (instanceComponenet == null)
            {
                Debug.LogFormat(GlobalVariables.cComponentAddition + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Instanced Visualization has no InstanceVis.CS component, adding componenet now.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                instanceComponenet = instancedVis.AddComponent<InstanceVis>();
            }

            instanceComponenet.isLoadedOffline = _isLoadedOffline;
            isLoadedOffline = _isLoadedOffline;
        }

        private void attachToPlayspace(GameObject obj)
        {
            if (PlayspaceAnchor.Instance != null)
            {
                obj.transform.SetParent(PlayspaceAnchor.Instance.transform);

                Debug.LogFormat(GlobalVariables.cCommon + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Attaching to the playspace anchor", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                obj.gameObject.transform.localPosition = Vector3.zero;
                obj.gameObject.transform.localRotation = Quaternion.identity;

                //obj.AddComponent<MakeVisualisation>();
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Can't attach to the playspace anchor, No anchor found", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
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

                int seconds = 4;
                
                Debug.LogFormat(GlobalVariables.cInstance + "Loading Visualisation in {0} seconds." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", seconds, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                Invoke("InitializeVisualisationProgramatically", seconds);
            }
            else
            {
                if (instancedVis != null)
                {
                    if (PhotonNetwork.IsConnected && !isLoadedOffline)
                    {
                        PhotonNetwork.Destroy(instancedVis);

                        Debug.LogFormat(GlobalVariables.cOnDestory + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Photon Destorying Instanced Vis", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    }
                    else if (!PhotonNetwork.IsConnected || isLoadedOffline)
                    {
                        Destroy(instancedVis);

                        Debug.LogFormat(GlobalVariables.cOnDestory + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Unity Destorying Instanced Vis", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                    }
                }
            }

        }

    }
}
