using System.Collections.Generic;
using UnityEngine;
using TMPro;
using IATK;
using System.Linq;
using Photon.Pun;

namespace Photon_IATK
{
    public class VisRPCMenuInterface : MonoBehaviour
    {

        public TMP_Dropdown xAxisDropdown;
        public TMP_Dropdown yAxisDropdown;
        public TMP_Dropdown zAxisDropdown;

        public TMP_Dropdown colorDimensionDropdown;
        public TMP_Dropdown sizeDimensionDropdown;

        private VisualizationRPC_Calls theVisualizationRPC_Calls;
        private string RPC_UID;

        // Start is called before the first frame update
        void Awake()
        {
            OnEnable();

            VisualizationRPC_Calls.RPCvisualisationUpdatedDelegate += UpdatedView;
            Debug.LogFormat(GlobalVariables.cRegister + "Registering {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "UpdatedView", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cCommon + "Vis view {0} updated." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            findAndStoreVisualizationRPC_Calls();

        }



        private void OnEnable()
        {
            //We need the dropdowns to be able to do anything to the RPCS
            if (xAxisDropdown == null || yAxisDropdown == null || zAxisDropdown == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "One or more axis dropdown menus not found.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            //Now we need to get the RPC interface assuming one VIS object
            findAndStoreVisualizationRPC_Calls();

        }

        private void findAndStoreVisualizationRPC_Calls()
        {
            //get all VisualizationRPC_Calls
            VisualizationRPC_Calls[] visualizationRPC_CallsCollection = GameObject.FindObjectsOfType<VisualizationRPC_Calls>();

            //If there are none or more than one we have a problem
            if (visualizationRPC_CallsCollection.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cError + "No VisualizationRPC_Calls found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            //Nothing has been setup
            if (theVisualizationRPC_Calls == null)
            {
                theVisualizationRPC_Calls = visualizationRPC_CallsCollection[0];

                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "New VisPRC referance set.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                setUpMenus();
                return;
            }

            //Check if a New vis is loaded
            if (theVisualizationRPC_Calls != null)
            {
                if (theVisualizationRPC_Calls != visualizationRPC_CallsCollection[0])
                {
                    theVisualizationRPC_Calls = visualizationRPC_CallsCollection[0];

                    Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "New VisRPC found, VisPRC referance updated.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    setUpMenus();
                    return;
                }
            }
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "The registered VisRPC is still valid.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }

        private void setUpMenus()
        {
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "Updating menu options.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }

        private void OnDisable()
        {
            OnDestroy();
        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Un-registering {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "UpdatedView", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            VisWrapperClass.visualisationUpdatedDelegate -= UpdatedView;
        }

    }
}