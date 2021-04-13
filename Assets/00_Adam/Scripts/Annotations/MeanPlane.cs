using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon_IATK
{
    public class MeanPlane : MonoBehaviour
    {
        public GameObject plane;
        public Material[] materials;

        public VisDataInterface visDataInterface;

        public enum axisSelection { xAxis, yAxis, zAxis };
        public axisSelection currentAxis = axisSelection.zAxis;
        private axisSelection lastAxisSelection;

        private void Update()
        {
            if (currentAxis != lastAxisSelection)
            {
                lastAxisSelection = currentAxis;
                SetMeanPlane();
            }
        }




        private void Awake()
        {
            GameObject vis;
            if (!HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out vis, false, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "No Vis tag object found.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            if (!HelperFunctions.GetComponent<VisDataInterface>(out visDataInterface, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "No VisDataInterface found.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            Invoke("SetMeanPlane", 3f);

        }

        private void SetMeanPlane()
        {
            Vector3 meanLocation;
            object meanValue;

            visDataInterface.getMeanLocation((int)currentAxis, out meanValue, out meanLocation);

            var visRotation = visDataInterface.GetVisRotation();

            this.gameObject.transform.position = meanLocation;
            this.gameObject.transform.localScale = visDataInterface.GetVisScale() / 10;
            this.gameObject.transform.eulerAngles = visRotation;

            switch (currentAxis)
            {
                case axisSelection.xAxis:
                    transform.RotateAround(this.transform.position, transform.forward, -90f);
                    break;
                case axisSelection.yAxis:
                    transform.RotateAround(this.transform.position, transform.right, -90f);
                    break;
                case axisSelection.zAxis:
                    break;
            }
            this.GetComponent<Renderer>().material = materials[(int)currentAxis];
        }


        public void setCurrentAxisToX()
        {
            currentAxis = axisSelection.xAxis;
        }

        public void setCurrentAxisToY()
        {
            currentAxis = axisSelection.yAxis;
        }

        public void setCurrentAxisToZ()
        {
            currentAxis = axisSelection.zAxis;
        }
    }
}

