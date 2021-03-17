using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

namespace Photon_IATK
{

    public class DetailsOnDemand : MonoBehaviour
    {
        //public text
        private TMPro.TextMeshPro textMeshPro;
        private GameObject vis;

        private VisWrapperClass visWrapperClass;
        private CSVDataSource csv;

        private Axis xAxis;
        private Axis yAxis;
        private Axis zAxis;

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
            if (!HelperFunctions.FindGameObjectOrMakeOneWithTag(GlobalVariables.visTag, out vis, false, System.Reflection.MethodBase.GetCurrentMethod()))
            {
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.cError + " {1}: {2} -> {3} -> {4}", "No Vis tags Found", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return;
            }

            visWrapperClass = vis.GetComponent<VisWrapperClass>();
            if (visWrapperClass == null)
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "visWrapperClass is null", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            DataSource dataSource = visWrapperClass.dataSource;
            if (dataSource == null)
                Debug.LogFormat(GlobalVariables.cError + "{0}." + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "csvDataSource is null", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            csv = (CSVDataSource)dataSource;

            foreach (Axis axis in vis.GetComponentsInChildren<IATK.Axis>())
            {
                switch (axis.AxisDirection){
                    case 1:
                        xAxis = axis;
                        break;
                    case 2:
                        yAxis = axis;
                        break;
                    case 3:
                        zAxis = axis;
                        break;
                }
            }
        }

        private AxisInfo xAxisInfo;
        private AxisInfo yAxisInfo;
        private AxisInfo zAxisInfo;

        private void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;

                xAxisInfo = setlabel(xAxis, xIndicator);
                yAxisInfo = setlabel(yAxis, yIndicator);
                zAxisInfo = setlabel(zAxis, zIndicator);

                setLabels();

                setClosestPoint();
            }
        }

        private Vector3[] csvItems;
        private void setClosestPoint()
        {
            //populate array of normalized points in vis
            csvItems = new Vector3[csv.DataCount - 1];
            for (int i = 0; i < csv.DataCount - 1; i++)
            {
                float x = csv[xAxis.AttributeName].Data[i];
                float y = csv[yAxis.AttributeName].Data[i];
                float z = csv[zAxis.AttributeName].Data[i];
                csvItems[i] = new Vector3(x, y, z);
            }

            Vector3 pointToSearchFrom = new Vector3 (xAxisInfo.normValue, yAxisInfo.normValue, zAxisInfo.normValue);

            Vector3 closestPoint = Vector3.one;
            float closestDist = 99f;
            foreach (Vector3 dataPoint in csvItems)
            {
                float dist = Vector3.Distance(pointToSearchFrom, dataPoint);
                if (dist < closestDist)
                {
                    closestPoint = dataPoint;
                    closestDist = dist;
                }
            }

            var clostestPointX = csv.getOriginalValuePrecise(closestPoint.x, xAxis.AttributeName);
            var clostestPointY = csv.getOriginalValuePrecise(closestPoint.y, yAxis.AttributeName);
            var clostestPointZ = csv.getOriginalValuePrecise(closestPoint.z, zAxis.AttributeName);


            closestPointText.text = "Closest Point\n";
            closestPointText.text += "X: " + clostestPointX + "\n";
            closestPointText.text += "Y: " + clostestPointY + "\n";
            closestPointText.text += "Z: " + clostestPointZ + "\n";

            //get location of new point
            Vector3 closestPointWorldLocation = Vector3.zero;

            closestPointWorldLocation.x = LerpByDistance(xAxis.minNormaliserObject.position, xAxis.maxNormaliserObject.position, closestPoint.x).x;
            closestPointWorldLocation.y = LerpByDistance(yAxis.minNormaliserObject.position, yAxis.maxNormaliserObject.position, closestPoint.y).y;
            closestPointWorldLocation.z = LerpByDistance(zAxis.minNormaliserObject.position, zAxis.maxNormaliserObject.position, closestPoint.z).z;

            closestPointIndicator.transform.position = closestPointWorldLocation;
        }

        private void setLabels()
        {
            xText.text = xAxisInfo.labelText();
            yText.text = yAxisInfo.labelText();
            zText.text = zAxisInfo.labelText();

            mainText.text = "Location\n";
            mainText.text += "X: " + xAxisInfo.axisLocation + "\n";
            mainText.text += "Y: " + yAxisInfo.axisLocation + "\n";
            mainText.text += "Z: " + zAxisInfo.axisLocation + "\n";
        }

        private AxisInfo setlabel(Axis axis, GameObject indicator)
        {
            AxisInfo outAxisInfo = new AxisInfo();

            DataSource.DimensionData.Metadata metaData = csv[axis.AttributeName].MetaData;
            Transform maxNormaliserTransform = axis.maxNormaliserObject;
            Transform minNormaliserTransform = axis.minNormaliserObject;

            switch (axis.AxisDirection)
            {
                case 1:
                    indicator.transform.position = new Vector3(this.transform.position.x, indicator.transform.position.y, indicator.transform.position.z);
                    break;
                case 2:
                    indicator.transform.position = new Vector3(indicator.transform.position.x, this.transform.position.y, indicator.transform.position.z);
                    break;
                case 3:
                    indicator.transform.position = new Vector3(indicator.transform.position.x, indicator.transform.position.y, this.transform.position.z);
                    break;
            }

            indicator.transform.rotation = axis.transform.rotation;
            indicator.transform.position = ClosestPoint(minNormaliserTransform.position, maxNormaliserTransform.position, this.transform.position);

            Vector3 minDelta = minNormaliserTransform.position - indicator.transform.position;
            float minDistance = Mathf.Sqrt(minDelta.x * minDelta.x + minDelta.y * minDelta.y + minDelta.z * minDelta.z);
            float axisValue = (metaData.maxValue - metaData.minValue) * minDistance + metaData.minValue;

            var normVal = csv.normaliseValue(axisValue, metaData.minValue, metaData.maxValue, 0f, 1f);
            var closestPointValue = csv.valueClosestTo(metaData.categories, normVal);
            var closestPointOriginalValue = csv.getOriginalValuePrecise(closestPointValue, axis.AttributeName);
            var originalValue = csv.getOriginalValuePrecise(normVal, axis.AttributeName);

            outAxisInfo.axisDirection = axis.AxisDirection;
            outAxisInfo.axisLocation = originalValue;
            outAxisInfo.closestPointValue = closestPointOriginalValue;
            outAxisInfo.normValue = normVal;

            return outAxisInfo;
        }

        public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
        {
            Vector3 P = x * Vector3.Normalize(B - A) + A;
            return P;
        }

        private Vector3 ClosestPoint(Vector3 limit1, Vector3 limit2, Vector3 point)
        {
            Vector3 lineVector = limit2 - limit1;

            float lineVectorSqrMag = lineVector.sqrMagnitude;

            // Trivial case where limit1 == limit2
            if (lineVectorSqrMag < 1e-3f)
                return limit1;

            float dotProduct = Vector3.Dot(lineVector, limit1 - point);

            float t = -dotProduct / lineVectorSqrMag;

            return limit1 + Mathf.Clamp01(t) * lineVector;
        }

    }
}

public class AxisInfo
{
    public int axisDirection;
    public object axisLocation;
    public object closestPointValue;
    public float normValue;

    public string labelText()
    {
        string axisLabelText = "Position: ";
        axisLabelText += axisLocation.ToString();
        axisLabelText += ", \nClosest point on this axis: ";
        axisLabelText += closestPointValue.ToString();

        return axisLabelText;
    }

}

//Debug.LogFormat(GlobalVariables.cAlert + "xAxis.AttributeFilter.maxFilter: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}." + GlobalVariables.endColor + " {8}: {9} -> {10} -> {11}", "0", "1", "2", "3", "4", "5", "6", "7", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());


//HelperFunctions.getJson(xAxis, "xAxis");
//HelperFunctions.getJson(dataSource, "dataSource");
//HelperFunctions.getJson(dataSource[xAxis.AttributeName], "dataSource[xAxis.AttributeName]");
//HelperFunctions.getJson(xMetaData, "xMetaData");