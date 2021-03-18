using UnityEngine;
using IATK;

namespace Photon_IATK
{
    public class DetailsOnDemand : MonoBehaviour
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

        public Vector3[] csvItems;
        public string[] csvItemsString;

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


        Vector3 closestPointLine1;
        Vector3 closestPointLine2;
        private void setClosestPoint()
        {
            this.transform.rotation = vis.transform.rotation;


            //populate array of normalized points in vis
            csvItems = new Vector3[csv.DataCount - 1];
            csvItemsString = new string[csv.DataCount - 1];
            for (int i = 0; i < csv.DataCount - 1; i++)
            {
                float x = csv[xAxis.AttributeName].Data[i];
                float y = csv[yAxis.AttributeName].Data[i];
                float z = csv[zAxis.AttributeName].Data[i];
                csvItems[i] = new Vector3(x, y, z);
            }

            Vector3 pointToSearchFrom = new Vector3 (xAxisInfo.normValue, yAxisInfo.normValue, zAxisInfo.normValue);
            //Vector3 pointToSearchFrom = Vector3.Scale(new Vector3(xAxisInfo.normValue, yAxisInfo.normValue, zAxisInfo.normValue), vis.transform.localScale);

            HelperFunctions.getJson(pointToSearchFrom, "pointToSearchFrom");

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

            //for (var i = 0; i < 8; i++)
            //{
            //    var item = csv[xAxis.AttributeName].Data[i];               
            //    Debug.Log(item + " = " + csv.getOriginalValue(item, xAxis.AttributeName).ToString());
            //    var normVal = csv.normaliseValue(item, csv[xAxis.AttributeName].MetaData.minValue, csv[xAxis.AttributeName].MetaData.maxValue, 0f, 1f);
            //    var tehirNrom = csv.normaliseValue(csv.valueClosestTo(csv[xAxis.AttributeName].Data, item), 0f, 1f, csv[xAxis.AttributeName].MetaData.minValue, csv[xAxis.AttributeName].MetaData.maxValue);
            //    Debug.Log(tehirNrom + " :NORM: " + normVal + ", " + csv[xAxis.AttributeName].MetaData.minValue + " : " +  csv[xAxis.AttributeName].MetaData.maxValue);
            //}

            //var actualItem = csv.TextualDimensionsList[xAxis.AttributeName];
            //var counter = 0;
            //foreach (var key in actualItem)
            //{
            //    Debug.Log("int key: " + key.Key + ", String value: " + key.Value + ", returned value: " + actualItem[key.Key]);
            //    if (counter == 8)
            //    {
            //        break;
            //    }
            //    counter++;
            //}



            var clostestPointX = csv.getOriginalValuePrecise(closestPoint.x, xAxis.AttributeName);
            var clostestPointY = csv.getOriginalValuePrecise(closestPoint.y, yAxis.AttributeName);
            var clostestPointZ = csv.getOriginalValuePrecise(closestPoint.z, zAxis.AttributeName);


            closestPointText.text = "Closest Point\n";
            closestPointText.text += "X: " + clostestPointX + "\n";
            closestPointText.text += "Y: " + clostestPointY + "\n";
            closestPointText.text += "Z: " + clostestPointZ + "\n";

            var test = "test Point\n";
            test += "X: " + csv.getOriginalValuePrecise(pointToSearchFrom.x, xAxis.AttributeName) + "\n";
            test += "Y: " + csv.getOriginalValuePrecise(pointToSearchFrom.y, yAxis.AttributeName) + "\n";
            test += "Z: " + csv.getOriginalValuePrecise(pointToSearchFrom.z, zAxis.AttributeName) + "\n";

            Debug.Log(test);

            //get location of new point
            Vector3 closestPointWorldLocation = Vector3.zero;

            closestPointWorldLocation.x = ClosestPoint(xAxis.minNormaliserObject.position, xAxis.maxNormaliserObject.position, closestPoint).x;
            closestPointWorldLocation.y = ClosestPoint(yAxis.minNormaliserObject.position, yAxis.maxNormaliserObject.position, closestPoint).y;
            closestPointWorldLocation.z = ClosestPoint(zAxis.minNormaliserObject.position, zAxis.maxNormaliserObject.position, closestPoint).z;

            closestPointWorldLocation.x = xIndicator.transform.position.x;
            closestPointWorldLocation.y = yIndicator.transform.position.y;
            closestPointWorldLocation.z = zIndicator.transform.position.z;

            var tmp2 = Vector3.Scale(closestPointWorldLocation, divideVectorValues(Vector3.one, vis.transform.localScale));
            HelperFunctions.getJson(tmp2, "tmp2");

            var tmp3 = Vector3.Scale(closestPointWorldLocation, divideVectorValues(vis.transform.localScale, Vector3.one));
            HelperFunctions.getJson(tmp3, "tmp3");

            var tmp1 = tmp3 - this.transform.position;
            HelperFunctions.getJson(tmp1, "tmp1");

            //First intersection

            Vector3 pos = xIndicator.transform.position;
            Vector3 dir = (xIndicator.transform.position + xIndicator.transform.right * -.5f);
            Debug.DrawLine(pos, dir, Color.red, 2f);

            pos = yIndicator.transform.position;
            dir = (yIndicator.transform.position + yIndicator.transform.right * .5f);
            Debug.DrawLine(pos, dir, Color.green, 2f);

            Vector3 xyIntersection;
            LineLineIntersection(out xyIntersection, xIndicator.transform.position, xIndicator.transform.right, yIndicator.transform.position, yIndicator.transform.right);

            //perpendicular to first intersection

            var xySide1 = xIndicator.transform.position - yIndicator.transform.position;
            var xySide2 = xyIntersection - yIndicator.transform.position;
            var xyPerpandicularDirection = Vector3.Cross(xySide1, xySide2);
            Debug.DrawRay(xyIntersection, xyPerpandicularDirection, Color.cyan, 2f);


            //Second Intersection

            pos = zIndicator.transform.position;
            dir = (zIndicator.transform.position + zIndicator.transform.forward * -.5f);
            Debug.DrawLine(pos, dir, Color.blue, 2f);

            pos = yIndicator.transform.position;
            dir = (yIndicator.transform.position + yIndicator.transform.forward * .5f);
            Debug.DrawLine(pos, dir, Color.green, 2f);

            Vector3 yzIntersection;
            LineLineIntersection(out yzIntersection, zIndicator.transform.position, zIndicator.transform.forward, yIndicator.transform.position, yIndicator.transform.forward);

            //perpendicular to first intersection

            var yzSide1 = yIndicator.transform.position - zIndicator.transform.position;
            var yzSide2 = yzIntersection - zIndicator.transform.position;
            var yzPerpandicularDirection = Vector3.Cross(yzSide1, yzSide2);
            Debug.DrawRay(yzIntersection, yzPerpandicularDirection, Color.cyan, 2f);

            //third intersection

            pos = xyIntersection;
            dir = (xyIntersection + xyPerpandicularDirection * .5f);
            Debug.DrawLine(pos, dir, Color.yellow, 2f);

            pos = yzIntersection;
            dir = (yzIntersection + yzPerpandicularDirection * .5f);
            Debug.DrawLine(pos, dir, Color.yellow, 2f);


            Vector3 xyzIntersection;
            LineLineIntersection(out xyzIntersection, xyIntersection, xyPerpandicularDirection, yzIntersection, yzPerpandicularDirection);

            closestPointIndicator.transform.position = xyzIntersection;


            ////pos = closestPointIndicator.transform.position;
            ////dir = (closestPointIndicator.transform.position + zIndicator.transform.forward * .5f);
            ////Debug.DrawLine(pos, dir, Color.white, 2f);

            ClosestPointsOnTwoLines(out closestPointLine1, out closestPointLine2, xyIntersection, xyPerpandicularDirection, yzIntersection, yzPerpandicularDirection);

            closestPointIndicator.transform.position = (closestPointLine1 + closestPointLine2) / 2f;
            //pos = zIndicator.transform.position;
            //dir = (zIndicator.transform.position + zIndicator.transform.right * .5f);
            //Debug.DrawLine(pos, dir, Color.blue, 2f);


            //xIndicator.transform.position = this.transform.position;
            //yIndicator.transform.position = tmp2;
            //zIndicator.transform.position = tmp3;

            //HelperFunctions.getJson(closestPointWorldLocation, "closestPointWorldLocation");

            //HelperFunctions.getJson(transform.position, "transform.position");

            //HelperFunctions.getJson(transform.InverseTransformPoint(tmp3), "ttransform.InverseTransformPoint(tmp3)");

        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(closestPointLine1, .025f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(closestPointLine2, .025f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere((closestPointLine2 + closestPointLine1) / 2f, .025f);
        }

        //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        //to each other. This function finds those two points. If the lines are not parallel, the function 
        //outputs true, otherwise false.
        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

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

                return true;
            }

            else
            {
                return false;
            }
        }

        //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
        //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
        //same plane, use ClosestPointsOnTwoLines() instead.
        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parrallel
            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        private Vector3 center(GameObject[] points)
        {
            Vector3 center = new Vector3(0, 0, 0);
            float count = 0;
            foreach (var item in points)
            {
                center += item.transform.position;
                count++;
            }
            var theCenter = center / count;
            return theCenter;
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

        private Vector3 divideVectorValues(Vector3 numerator, Vector3 demoninator) {

            Vector3 output = Vector3.zero;
            output.x = numerator.x / demoninator.x;
            output.y = numerator.y / demoninator.y;
            output.z = numerator.z / demoninator.z;
            return output;
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