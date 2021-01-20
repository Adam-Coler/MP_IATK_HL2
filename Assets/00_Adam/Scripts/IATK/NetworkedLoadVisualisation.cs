using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{

    public class NetworkedLoadVisualisation : MonoBehaviour
    {
        public bool isAutoLoad = false;
        public GameObject Prefab;

        public void InitializeVisualisationProgramatically()
        {
            GameObject vis;

            if (PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogFormat(GlobalVariables.cInstance + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Instantiateing IATK scatterplot online", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                 vis = PhotonNetwork.Instantiate("Vis", Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Not connected to Photon, loading offline", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                vis = Instantiate(Prefab, new Vector3(1.5f, 0, 0), Quaternion.identity);
            }
            attachToPlayspace(vis);
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
            VisWrapperClass[] visWrappers = FindObjectsOfType<VisWrapperClass>();

            if (visWrappers.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cInstance + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Loading Visualisation", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                InitializeVisualisationProgramatically();

                return;
            }

            foreach (VisWrapperClass loadedVis in visWrappers)
            {

                if (PhotonNetwork.IsConnectedAndReady)
                {
                    PhotonNetwork.Destroy(loadedVis.gameObject.GetComponent<PhotonView>());

                    Debug.LogFormat(GlobalVariables.cOnDestory + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Photon Destorying Instanced Vis, THERE CAN BE ONLY ONE!", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
                else
                {
                    Destroy(loadedVis.gameObject);

                    Debug.LogFormat(GlobalVariables.cOnDestory + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Unity Destorying Instanced Vis, THERE CAN BE ONLY ONE!", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                }
            }

        }

    }
}
