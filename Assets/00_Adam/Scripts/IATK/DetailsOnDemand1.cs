using UnityEngine;
using IATK;

namespace Photon_IATK
{
    public class DetailsOnDemand1 : MonoBehaviour
    {
        private GameObject vis;
        private VisWrapperClass visWrapperClass;
        private CSVDataSource csv;

        private Axis xAxis;
        private Axis yAxis;
        private Axis zAxis;

        private AxisInfo xAxisInfo;
        private AxisInfo yAxisInfo;
        private AxisInfo zAxisInfo;

        private Vector3[] csvItems;

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
                switch (axis.AxisDirection)
                {
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

        private void OnEnable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync registering OnEvent, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate += UpdatedView;
        }

        private void OnDisable()
        {
            Debug.LogFormat(GlobalVariables.cRegister + "GenericTransformSync unregistering OnEvent, RPCvisualisationUpdatedDelegate.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

            VisualizationEvent_Calls.RPCvisualisationUpdatedDelegate -= UpdatedView;
        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
        {
            matchToCurrentVis();
        }

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

            Vector3 pointToSearchFrom = new Vector3(xAxisInfo.normValue, yAxisInfo.normValue, zAxisInfo.normValue);

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


            //closestPointText.text = "Closest Point\n";
            //closestPointText.text += "X: " + closestPoint.x + "\n";
            //closestPointText.text += "Y: " + closestPoint.y + "\n";
            //closestPointText.text += "Z: " + closestPoint.z + "\n";

            closestPointText.text = "Closest Point\n";
            closestPointText.text += "X: " + clostestPointX + "\n";
            closestPointText.text += "Y: " + clostestPointY + "\n";
            closestPointText.text += "Z: " + clostestPointZ + "\n";

            Vector3 closestPointWorldLocationX = Vector3.MoveTowards(xAxis.minNormaliserObject.position, xAxis.maxNormaliserObject.position, closestPoint.x * Vector3.Distance(xAxis.minNormaliserObject.position, xAxis.maxNormaliserObject.position));

            Vector3 closestPointWorldLocationY = Vector3.MoveTowards(yAxis.minNormaliserObject.position, yAxis.maxNormaliserObject.position, closestPoint.y * Vector3.Distance(yAxis.minNormaliserObject.position, yAxis.maxNormaliserObject.position));

            Vector3 closestPointWorldLocationZ = Vector3.MoveTowards(zAxis.minNormaliserObject.position, zAxis.maxNormaliserObject.position, closestPoint.z * Vector3.Distance(zAxis.minNormaliserObject.position, zAxis.maxNormaliserObject.position));

            closestPointIndicator.transform.position = getIntersectionOfThreeAxis(closestPointWorldLocationX, closestPointWorldLocationY, closestPointWorldLocationZ);

        }

        public Vector3 getIntersectionOfThreeAxis(Vector3 x, Vector3 y, Vector3 z)
        {
            //First intersection
            Vector3 xyIntersection;
            ClosestPointsOnTwoLines(out xyIntersection, x, xIndicator.transform.right, y, yIndicator.transform.right);

            //perpendicular to first intersection
            var xySide1 = x - y;
            var xySide2 = xyIntersection - x;
            var xyPerpandicularDirection = Vector3.Cross(xySide1, xySide2);

            //Second Intersection
            Vector3 yzIntersection;
            ClosestPointsOnTwoLines(out yzIntersection, z, zIndicator.transform.forward, y, yIndicator.transform.forward);

            //perpendicular to second intersection
            var yzSide1 = y - z;
            var yzSide2 = yzIntersection - z;
            var yzPerpandicularDirection = Vector3.Cross(yzSide1, yzSide2);

            //third intersection
            Vector3 xyzclosestPoint;
            ClosestPointsOnTwoLines(out xyzclosestPoint, xyIntersection, xyPerpandicularDirection, yzIntersection, yzPerpandicularDirection);
            return xyzclosestPoint;
        }

        //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        //to each other. This function finds those two points. If the lines are not parallel, the function 
        //outputs true, otherwise false.
        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            Vector3 closestPointLine1 = Vector3.zero;
            Vector3 closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            //lines are not parallel
            if (d != 0.0f)
            {

                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;
                closestPointLine = (closestPointLine1 + closestPointLine2) / 2f;
                return true;
            }

            else
            {
                closestPointLine = Vector3.zero;
                return false;
            }
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

            indicator.transform.rotation = axis.transform.rotation;
            indicator.transform.position = ClosestPoint(axis.minNormaliserObject.position, axis.maxNormaliserObject.position, this.transform.position);

            Vector3 minDelta = axis.minNormaliserObject.position - indicator.transform.position;
            minDelta = Vector3.Scale(minDelta, divideVectorValues(Vector3.one, vis.transform.localScale));

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

        private Vector3 divideVectorValues(Vector3 numerator, Vector3 demoninator)
        {

            Vector3 output = Vector3.zero;
            output.x = numerator.x / demoninator.x;
            output.y = numerator.y / demoninator.y;
            output.z = numerator.z / demoninator.z;
            return output;
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