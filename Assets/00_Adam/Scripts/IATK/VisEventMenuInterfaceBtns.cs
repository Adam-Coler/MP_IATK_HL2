using System.Collections.Generic;
using UnityEngine;
using TMPro;
using IATK;
using System.Linq;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.UI;

namespace Photon_IATK
{
    public class VisEventMenuInterfaceBtns : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI xAxis;
        public TMPro.TextMeshProUGUI yAxis;
        public TMPro.TextMeshProUGUI zAxis;
        public TMPro.TextMeshProUGUI SizeColorAxis;


        private VisualizationEvent_Calls theVisualizationEvent_Calls;

        public void changeXAxis(string newValue)
        {
            if (theVisualizationEvent_Calls.xDimension != newValue)
                theVisualizationEvent_Calls.RequestChangeXAxisEvent(newValue);
        }

        public void changeYAxis(string newValue)
        {
            if (theVisualizationEvent_Calls.yDimension != newValue)
                theVisualizationEvent_Calls.RequestChangeYAxisEvent(newValue);
        }

        public void changeZAxis(string newValue)
        {
            if (theVisualizationEvent_Calls.zDimension != newValue)
                theVisualizationEvent_Calls.RequestChangeZAxisEvent(newValue);
        }

        public void changeSizeAndColarDimension(string newValue)
        {
            if (theVisualizationEvent_Calls.sizeDimension != newValue)
            {
                theVisualizationEvent_Calls.RequestChangeSizeDimensionEvent(newValue);
                theVisualizationEvent_Calls.RequestChangeColorDimensionEvent(newValue);
            }
        }

        private void OnEnable()
        {

            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate += UpdatedView;
            VisWrapperClass.visualisationUpdatedDelegate += UpdatedView;

            Debug.LogFormat(GlobalVariables.cRegister + "Registering {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "UpdatedView", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            //Now we need to get the RPC interface assuming one VIS object and that the vis object is loaded before the menu
            findAndStoreVisualizationRPC_Calls();

            if (theVisualizationEvent_Calls != null)
            {
                string x = "Manufacturer";
                string y = "DietaryFiber";
                string z = "Sugars";

                if (theVisualizationEvent_Calls.yDimension == x)
                {
                    changeYAxis(y);
                }

                if (theVisualizationEvent_Calls.zDimension == x)
                {
                    changeZAxis(z);
                }
            }


            updateAllLabels();
        }


        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            Debug.LogFormat(GlobalVariables.cTest + "Vis view {0} updated." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            findAndStoreVisualizationRPC_Calls();
            updateAllLabels();
        }

        private void updateAllLabels()
        {
            xAxis.text = "X-axis\n" + theVisualizationEvent_Calls.xDimension;
            yAxis.text = "Y-axis\n" + theVisualizationEvent_Calls.yDimension;
            zAxis.text = "Z-axis\n" + theVisualizationEvent_Calls.zDimension;
            SizeColorAxis.text = "Size/Color\n" + theVisualizationEvent_Calls.colourDimension;
        }

        private void findAndStoreVisualizationRPC_Calls()
        {
            //get all VisualizationRPC_Calls
            VisualizationEvent_Calls[] visualizationRPC_CallsCollection = GameObject.FindObjectsOfType<VisualizationEvent_Calls>();

            //If there are none or more than one we have a problem
            if (visualizationRPC_CallsCollection.Length == 0)
            {
                Debug.LogFormat(GlobalVariables.cError + "No VisualizationRPC_Calls found." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            //Nothing has been setup
            if (theVisualizationEvent_Calls == null)
            {
                theVisualizationEvent_Calls = visualizationRPC_CallsCollection[0];

                Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "New VisPRC referance set.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            //Check if a New vis is loaded
            if (theVisualizationEvent_Calls != null)
            {
                if (theVisualizationEvent_Calls != visualizationRPC_CallsCollection[0])
                {
                    theVisualizationEvent_Calls = visualizationRPC_CallsCollection[0];

                    Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "New VisRPC found, VisPRC referance updated.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    return;
                }
            }
            Debug.LogFormat(GlobalVariables.cCommon + "{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "The registered VisRPC is still valid.", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
        }


        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Un-registering {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "UpdatedView", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            VisWrapperClass.visualisationUpdatedDelegate -= UpdatedView;
            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate -= UpdatedView;
        }

    }
}