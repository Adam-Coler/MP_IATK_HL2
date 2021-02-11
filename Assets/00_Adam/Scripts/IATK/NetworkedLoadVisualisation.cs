using UnityEngine;
using Photon.Pun;

namespace Photon_IATK
{

    public class NetworkedLoadVisualisation : MonoBehaviour
    {
        public bool isAutoLoad = false;
        public GameObject Prefab;

        private void Awake()
        {
            if (Prefab == null)
            {
                Prefab = Resources.Load<GameObject>("Vis");
            }
            if (!isAutoLoad) { return; }
            LoadVis();
        }

        public void LoadVis()
        {
            VisWrapperClass[] visWrappers = FindObjectsOfType<VisWrapperClass>();

            if (visWrappers.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cInstance + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Loading Visualisation", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                GeneralEventManager.instance.SendVisSceneInstantiateEvent();
                return;
            }

            AnnotationManagerSaveLoadEvents annotationManager;
            if (HelperFunctions.GetComponent<AnnotationManagerSaveLoadEvents>(out annotationManager, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                annotationManager.saveAnnotations();
            }

            GeneralEventManager.instance.SendDeleteAllObjectsWithComponentRequest(typeof(VisWrapperClass).AssemblyQualifiedName);

        }

    }
}
