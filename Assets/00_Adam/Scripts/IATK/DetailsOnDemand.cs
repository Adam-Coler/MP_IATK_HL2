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

            float xF;
            string x = labels[0].ToString();

            if (float.TryParse(x, out xF))
            {
                x = string.Format("{0:0.##}", xF);
            }

            float yF;
            string y = labels[1].ToString();

            if (float.TryParse(y, out yF))
            {
                y = string.Format("{0:0.##}", yF);
            }

            float zF;
            string z = labels[2].ToString();

            if (float.TryParse(z, out zF))
            {
                z = string.Format("{0:0.##}", zF);
            }

            closestPointText.text = "Closest Point\n";
            closestPointText.text += "X: " + x + "\n";
            closestPointText.text += "Y: " + y + "\n";
            closestPointText.text += "Z: " + z + "\n";
        }

        private void setMainText()
        {
            object[] labels = visDataInterface.GetLabelsForAxisLocations(transform.position);

            float xF;
            string x = labels[0].ToString();

            if (float.TryParse(x, out xF))
            {
                x = string.Format("{0:0.##}", xF);
            }

            float yF;
            string y = labels[1].ToString();

            if (float.TryParse(y, out yF))
            {
                y = string.Format("{0:0.##}", yF);
            }

            float zF;
            string z = labels[2].ToString();

            if (float.TryParse(z, out zF))
            {
                z = string.Format("{0:0.##}", zF);
            }

            mainText.text = "Location\n";
            mainText.text += "X: " + x + "\n";
            mainText.text += "Y: " + y + "\n";
            mainText.text += "Z: " + z + "\n";
        }

        private void setIndicatorText()
        {
            object[] labels = visDataInterface.GetLabelsForAxisLocations(transform.position);

            float xF;
            string x = labels[0].ToString();

            if (float.TryParse(x, out xF))
            {
                x = string.Format("{0:0.##}", xF);
            }

            float yF;
            string y = labels[1].ToString();

            if (float.TryParse(y, out yF))
            {
                y = string.Format("{0:0.##}", yF);
            }

            float zF;
            string z = labels[2].ToString();

            if (float.TryParse(z, out zF))
            {
                z = string.Format("{0:0.##}", zF);
            }

            xText.text = x.ToString();
            yText.text = y.ToString();
            zText.text = z.ToString();
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