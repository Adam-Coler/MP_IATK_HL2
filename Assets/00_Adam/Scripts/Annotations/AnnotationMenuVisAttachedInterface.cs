using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class AnnotationMenuVisAttachedInterface : MonoBehaviour
    {
        public AnnotationManagerSaveLoadEvents annotationMGR;

        private int searchTries = 0;

        private void Awake()
        {
            if(!HelperFunctions.GetComponent<AnnotationManagerSaveLoadEvents>(out annotationMGR, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "No AnnotationManagerSaveLoadEvents found.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public void RequestAnnotationCreationTestTracker()
        {
            annotationMGR.RequestAnnotationCreationTestTracker();
        }

        public void RequestAnnotationCreationCentralityMetricPlane()
        {
            annotationMGR.RequestAnnotationCreationCentralityMetricPlane();
        }

        public void RequestAnnotationCreationHighlightCube()
        {
            annotationMGR.RequestAnnotationCreationHighlightCube();
        }

        public void RequestAnnotationCreationHighlightSphere()
        {
            annotationMGR.RequestAnnotationCreationHighlightSphere();
        }

        public void RequestAnnotationCreationDetailsOnDemand()
        {
            annotationMGR.RequestAnnotationCreationDetailsOnDemand();
        }

        public void RequestAnnotationCreationText()
        {
            annotationMGR.RequestAnnotationCreationText();
        }

    }
}
