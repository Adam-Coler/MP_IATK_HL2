using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using Microsoft.MixedReality.Toolkit.UI;

namespace Photon_IATK
{
    public class ToggleSwapSelectionMatchVis : MonoBehaviour
    {
        public InteractableToggleCollection toggles;
        public AbstractVisualisation.PropertyType property;
        public List<string> labels;

        public VisualizationEvent_Calls theVisualizationEvent_Calls;

        private void OnEnable()
        {
            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate += UpdatedView;

            if (theVisualizationEvent_Calls == null)
            {
                VisualizationEvent_Calls[] visualizationRPC_CallsCollection = GameObject.FindObjectsOfType<VisualizationEvent_Calls>();
                if (theVisualizationEvent_Calls == null)
                {
                    theVisualizationEvent_Calls = visualizationRPC_CallsCollection[0];
                }
            }
        }

        private void OnDisable()
        {
            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate -= UpdatedView;
        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            if (theVisualizationEvent_Calls == null) { return; }

            string dimension = "Undefined";

            switch (property)
            {
                case (AbstractVisualisation.PropertyType.X):
                    dimension = theVisualizationEvent_Calls.getVisDimension(AbstractVisualisation.PropertyType.X);
                    break;
                case (AbstractVisualisation.PropertyType.Y):
                    dimension = theVisualizationEvent_Calls.getVisDimension(AbstractVisualisation.PropertyType.Y);
                    break;
                case (AbstractVisualisation.PropertyType.Z):
                    dimension = theVisualizationEvent_Calls.getVisDimension(AbstractVisualisation.PropertyType.Z);
                    break;
                case (AbstractVisualisation.PropertyType.Colour):
                    dimension = theVisualizationEvent_Calls.getVisDimension(AbstractVisualisation.PropertyType.Colour);
                    break;
                case (AbstractVisualisation.PropertyType.Size):
                    dimension = theVisualizationEvent_Calls.getVisDimension(AbstractVisualisation.PropertyType.Z);
                    break;
                default:
                    dimension = "Undefined";
                    break;
            }

            Debug.LogFormat(GlobalVariables.cTest + "Toggle Swap Vis view {0} updated. Label contained:" + labels.Contains(dimension) + GlobalVariables.endColor + " { 1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            if (labels.Contains(dimension))
            {
                Debug.LogFormat(GlobalVariables.cTest + "Toggle Swap Vis view {0} " + labels.Contains(dimension) + " at index " + labels.IndexOf(dimension) + "." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", propertyType, Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                toggles.SetSelection(labels.IndexOf(dimension));
            }


        }

    }
}