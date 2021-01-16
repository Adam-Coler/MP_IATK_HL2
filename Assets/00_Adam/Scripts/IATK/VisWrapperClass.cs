using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using IATK;

namespace Photon_IATK
{
    [ExecuteInEditMode]
    public class VisWrapperClass : Visualisation
    {
        public delegate void OnVisualisationUpdated(AbstractVisualisation.PropertyType propertyType);
        public static OnVisualisationUpdated visualisationUpdatedDelegate;

        private void Awake()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "VisWrapper registering OnUpdateViewAction." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            OnUpdateViewAction += UpdatedView;
        }

        private void OnDestroy()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "Un-registering {0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "UpdatedView", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            OnUpdateViewAction -= UpdatedView;
        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            if (visualisationUpdatedDelegate != null)
                visualisationUpdatedDelegate(propertyType);
        }

        public void updateVisPropertiesSafe()
        {
            AbstractVisualisation theVisObject = this.theVisualizationObject;

            if (theVisObject == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "The Visualisation obect is null." + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            if (theVisObject.X_AXIS == null || theVisObject.Y_AXIS == null || theVisObject.Z_AXIS == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "The Visualisation is missing an axis. X == null: " + (theVisObject.X_AXIS == null) + " , Y == null: " + (theVisObject.Y_AXIS == null) + " , Z == null: " + (theVisObject.Z_AXIS == null) + GlobalVariables.endColor + " {0}: {1} -> {2} -> {3}", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            this.updateProperties();

        }

        private void setPropertiesToUndefined()
        {
            //IATK does not initalized these so they cause an error when we manually load a photon instanced graph.
            string undefined = "Undefined";

            this.colourDimension = undefined;
            this.sizeDimension = undefined;
            this.linkingDimension = undefined;
            this.originDimension = undefined;
            this.destinationDimension = undefined;
            this.graphDimension = undefined;
            this.colorPaletteDimension = undefined;
        }

    }
}
