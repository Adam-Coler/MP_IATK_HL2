using System.Collections;
using UnityEngine.Events;
using System.Collections;
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

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            if (visualisationUpdatedDelegate != null)
                visualisationUpdatedDelegate(propertyType);
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
