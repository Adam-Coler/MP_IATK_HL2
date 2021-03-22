using UnityEngine;
using IATK;

namespace Photon_IATK
{
    public class DetailsOnDemand : MonoBehaviour
    {
        public VisDataInterface visDataInterface;

        public GameObject xIndicator;
        public GameObject yIndicator;
        public GameObject zIndicator;
        public GameObject closestPointIndicator;

        public TMPro.TextMeshPro mainText;
        public TMPro.TextMeshPro xText;
        public TMPro.TextMeshPro yText;
        public TMPro.TextMeshPro zText;
        public TMPro.TextMeshPro closestPointText;

        private void Awake()
        {
            matchToCurrentVis();
        }

        private void matchToCurrentVis()
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
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;

                setIndicators();
                setClosestPoint();
                setMainText();
                setIndicatorText();

            }
        }

        private void setClosestPoint()
        {
            closestPointIndicator.transform.position = visDataInterface.GetClosestPoint(this.transform.position);
            object[] labels = visDataInterface.GetLabelsForClosestPoint(transform.position);

            closestPointText.text = "Closest Point\n";
            closestPointText.text += "X: " + labels[0] + "\n";
            closestPointText.text += "Y: " + labels[1] + "\n";
            closestPointText.text += "Z: " + labels[2] + "\n";
        }

        private void setMainText()
        {
            object[] labels = visDataInterface.GetLabelsForAxisLocations(transform.position);
            mainText.text = "Location\n";
            mainText.text += "X: " + labels[0] + "\n";
            mainText.text += "Y: " + labels[1] + "\n";
            mainText.text += "Z: " + labels[2] + "\n";
        }

        private void setIndicatorText()
        {
            object[] labels = visDataInterface.GetLabelsForAxisLocations(transform.position);
            xText.text = labels[0].ToString();
            yText.text = labels[1].ToString();
            zText.text = labels[2].ToString();
        }


        private void setIndicators()
        {
            xIndicator.transform.position = visDataInterface.GetClosestPointOnAxis(1, transform.position);
            xIndicator.transform.rotation = visDataInterface.xAxisRotation;

            yIndicator.transform.position = visDataInterface.GetClosestPointOnAxis(2, transform.position);
            yIndicator.transform.rotation = visDataInterface.yAxisRotation;

            zIndicator.transform.position = visDataInterface.GetClosestPointOnAxis(3, transform.position);
            zIndicator.transform.rotation = visDataInterface.zAxisRotation;
        }
    }
}