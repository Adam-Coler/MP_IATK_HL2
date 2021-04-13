using UnityEngine;
using IATK;
using System.Collections.Generic;
using System.Linq;

namespace Photon_IATK
{

    public class VisDataInterface : MonoBehaviour
    {
        public GameObject vis;
        public VisWrapperClass visWrapperClass;
        public CSVDataSource csv;

        public Axis xAxis;
        public Axis yAxis;
        public Axis zAxis;

        private Vector3[] csvItems;

        private GameObject obj;

        public float eps = .01f;

        public Quaternion xAxisRotation { get { return xAxis.transform.rotation; } }
        public Quaternion yAxisRotation { get { return yAxis.transform.rotation; } }
        public Quaternion zAxisRotation { get { return zAxis.transform.rotation; } }

        public bool isSetUp = false;

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

        private void Awake()
        {
            Invoke("matchToCurrentVis", .5f);
        }

        private void UpdatedView(AbstractVisualisation.PropertyType propertyType)
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

            csvItems = getListOfPoints();

            isSetUp = true;

        }

        public string GetAxisName(int axis)
        {
            if (!isSetUp) { return ""; }

            string output = "";
            switch (axis)
            {
                case 1:
                    output = xAxis.AttributeName;
                    break;
                case 2:
                    output = yAxis.AttributeName;
                    break;
                case 3:
                    output = zAxis.AttributeName;
                    break;
            }

            return output;
        }

        public void getCentralityMetricLocation(int axis, out object actualValue, out Vector3 location, bool isMean = true)
        {

            if (axis >= 4 || axis < 0 || !isSetUp) {
                actualValue = 0f;
                location = Vector3.zero;
                return;
            }

            axis += 1;
            Axis axisClass = GetAxisFromInt(axis);

            string axisName = "";
            if (axisClass != null)
            {
                axisName = axisClass.AttributeName;
            }
            
            var points = csv[axisName].Data;

            float metric = 0;

            if (isMean)
            {
                //normalized mean
                metric = points.Average();
            }
            else
            {
                //normalized median
                metric = HelperFunctions.getMedian(points);
            }


            //actual mean
            actualValue = csv.getOriginalValuePrecise(metric, axisName);

            //direction
            location = Vector3.zero;

            switch (axis)
            {
                case 1:
                    location = new Vector3(metric, .5f, .5f);
                    break;
                case 2:
                    location = new Vector3(.5f, metric, .5f);
                    break;
                case 3:
                    location = new Vector3(.5f, .5f, metric);
                    break;
            }
            location = GetVisPointWorldLocation(location);
        }

        public Vector3 GetVisScale()
        {
            if (!isSetUp) { return Vector3.one; }
            //if (vis == null) { return Vector3.one; }

            return vis.transform.localScale;
        }

        public Vector3 GetVisRotation()
        {
            if (!isSetUp) { return Vector3.zero; }
            //if (vis == null) { return Vector3.zero; }

            return vis.transform.rotation.eulerAngles;
        }

        public Vector3 GetNormalizedPoint(Vector3 point)
        {
            Vector3 output = Vector3.zero;
            output.x = GetSingleAxisNormalizedValue(1, point);
            output.y = GetSingleAxisNormalizedValue(2, point);
            output.z = GetSingleAxisNormalizedValue(3, point);
            return output;
        }

        /// <summary>
        /// Axis values 1=X, 2=Y, 3=Z
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetSingleAxisNormalizedValue(int axisDirection, Vector3 point)
        {
            Axis axis = GetAxisFromInt(axisDirection);
            DataSource.DimensionData.Metadata metaData = csv[axis.AttributeName].MetaData;

            //Vector3 closestPoint = ClosestPoint(axis.minNormaliserObject.position, axis.maxNormaliserObject.position, point);

            Vector3 closestPoint = GetClosestPointOnAxis(axisDirection, point);

            Vector3 minDelta = axis.minNormaliserObject.position - closestPoint;
            minDelta = Vector3.Scale(minDelta, divideVectorValues(Vector3.one, vis.transform.localScale));

            float minDistance = Mathf.Sqrt(minDelta.x * minDelta.x + minDelta.y * minDelta.y + minDelta.z * minDelta.z);
            float axisValue = (metaData.maxValue - metaData.minValue) * minDistance + metaData.minValue;
            var normVal = csv.normaliseValue(axisValue, metaData.minValue, metaData.maxValue, 0f, 1f);

            return normVal;
        }

        private Axis GetAxisFromInt(int axisDirection)
        {
            Axis axis;
            switch (axisDirection)
            {
                case 1:
                    axis = xAxis;
                    break;
                case 2:
                    axis = yAxis;
                    break;
                case 3:
                    axis = zAxis;
                    break;
                default:
                    axis = xAxis;
                    break;
            }
            return axis;
        }

        public object[] GetLabelsForAxisLocations(Vector3 point)
        {
            object[] output = new object[3];

            float normVal = GetSingleAxisNormalizedValue(1, point);
            output[0] = csv.getOriginalValuePrecise(normVal, xAxis.AttributeName);

            normVal = GetSingleAxisNormalizedValue(2, point);
            output[1] = csv.getOriginalValuePrecise(normVal, yAxis.AttributeName);

            normVal = GetSingleAxisNormalizedValue(3, point);
            output[2] = csv.getOriginalValuePrecise(normVal, zAxis.AttributeName);

            return output;
        }

        public Vector3 GetClosestPointOnAxis(int axisDirection, Vector3 point)
        {
            Axis axis = GetAxisFromInt(axisDirection);
            return ClosestPoint(axis.minNormaliserObject.position, axis.maxNormaliserObject.position, point);
        }

        public object[] GetLabelsForClosestPoint(Vector3 point)
        {
            object[] output = new object[3];

            var closestPoint = GetClosestPoint(point, true);
            output[0] = csv.getOriginalValuePrecise(closestPoint.x, xAxis.AttributeName);
            output[1] = csv.getOriginalValuePrecise(closestPoint.y, yAxis.AttributeName);
            output[2] = csv.getOriginalValuePrecise(closestPoint.z, zAxis.AttributeName);

            return output;
        }

        public Vector3 GetClosestPoint(Vector3 worldLocationToSearchFrom, bool isNormalized = false)
        {
            Vector3 pointToSearchFrom = GetNormalizedPoint(worldLocationToSearchFrom);
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

            if (isNormalized)
            {
                return closestPoint;
            }
            return GetVisPointWorldLocation(closestPoint);
        }

        private Vector3 divideVectorValues(Vector3 numerator, Vector3 demoninator)
        {

            Vector3 output = Vector3.zero;
            output.x = numerator.x / demoninator.x;
            output.y = numerator.y / demoninator.y;
            output.z = numerator.z / demoninator.z;
            return output;
        }

        public Vector3[] getListOfPoints()
        {
            Vector3[] csvArrayOfDataPoints = new Vector3[csv.DataCount];
            for (int i = 0; i < csv.DataCount; i++)
            {
                float x = csv[xAxis.AttributeName].Data[i];
                float y = csv[yAxis.AttributeName].Data[i];
                float z = csv[zAxis.AttributeName].Data[i];
                csvArrayOfDataPoints[i] = new Vector3(x, y, z);
            }

            return csvArrayOfDataPoints;
        }

        public Vector3[] getListOfWorldLocationPoints()
        {
            if (csv == null)
            {
                Debug.LogFormat(GlobalVariables.cError + "No CSV set.{0}" + GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());
                return new Vector3[0];
            }

            Vector3[] returnedPoints = new Vector3[csv.DataCount];
            Vector3[] points = getListOfPoints();
            for (int i = 0; i < csv.DataCount; i++)
            {
                returnedPoints[i] = GetVisPointWorldLocation(points[i]);
            }

            return returnedPoints;
        }

        public List<Vector3> IsInsideMesh(Collider mesh)
        {
            List<Vector3> encapsalatedPoints = new List<Vector3>();

            foreach(Vector3 point in getListOfWorldLocationPoints())
            {
                if (Vector3.Distance(point, mesh.ClosestPoint(point)) < eps)
                {
                    encapsalatedPoints.Add(point);
                }
            }
            return encapsalatedPoints;
        }

        public Vector3 GetVisPointWorldLocation(Vector3 normalizedAxisValues)
        {
            if (normalizedAxisValues.x == 0)
            {
                normalizedAxisValues.x += eps;
            }
            if (normalizedAxisValues.x == 1)
            {
                normalizedAxisValues.x -= eps;
            }

            if (normalizedAxisValues.y == 0)
            {
                normalizedAxisValues.y += eps;
            }
            if (normalizedAxisValues.y == 1)
            {
                normalizedAxisValues.y -= eps;
            }

            if (normalizedAxisValues.z == 0)
            {
                normalizedAxisValues.z += eps;
            }
            if (normalizedAxisValues.z == 1)
            {
                normalizedAxisValues.z -= eps;
            }

            Vector3 closestPointWorldLocationX = Vector3.MoveTowards(xAxis.minNormaliserObject.position, xAxis.maxNormaliserObject.position, normalizedAxisValues.x * Vector3.Distance(xAxis.minNormaliserObject.position, xAxis.maxNormaliserObject.position));

            Vector3 closestPointWorldLocationY = Vector3.MoveTowards(yAxis.minNormaliserObject.position, yAxis.maxNormaliserObject.position, normalizedAxisValues.y * Vector3.Distance(yAxis.minNormaliserObject.position, yAxis.maxNormaliserObject.position));

            Vector3 closestPointWorldLocationZ = Vector3.MoveTowards(zAxis.minNormaliserObject.position, zAxis.maxNormaliserObject.position, normalizedAxisValues.z * Vector3.Distance(zAxis.minNormaliserObject.position, zAxis.maxNormaliserObject.position));

            Vector3 pointIndc = getIntersectionOfThreeAxis(closestPointWorldLocationX, closestPointWorldLocationY, closestPointWorldLocationZ);

            return pointIndc;
        }

        public Vector3 getIntersectionOfThreeAxis(Vector3 x, Vector3 y, Vector3 z)
        {

            Transform xAxisT = xAxis.maxNormaliserObject.transform;
            Transform yAxisT = yAxis.maxNormaliserObject.transform;
            Transform zAxisT = zAxis.maxNormaliserObject.transform;

            //First intersection
            Vector3 xyIntersection;
            ClosestPointsOnTwoLines(out xyIntersection, x, xAxisT.up, y, yAxisT.up);

            //perpendicular to first intersection
            var xySide1 = x - y;
            var xySide2 = xyIntersection - x;
            var xyPerpandicularDirection = Vector3.Cross(xySide1, xySide2);

            //Second Intersection
            Vector3 yzIntersection;
            ClosestPointsOnTwoLines(out yzIntersection, z, zAxisT.right, y, -yAxisT.right);

            //perpendicular to second intersection
            var yzSide1 = y - z;
            var yzSide2 = yzIntersection - z;
            var yzPerpandicularDirection = Vector3.Cross(yzSide1, yzSide2);

            //third intersection
            Vector3 xyzclosestPoint;
            ClosestPointsOnTwoLines(out xyzclosestPoint, xyIntersection, xyPerpandicularDirection, yzIntersection, yzPerpandicularDirection);

            return xyzclosestPoint;
        }

        public Vector3 ClosestPoint(Vector3 limit1, Vector3 limit2, Vector3 point)
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

        public bool ClosestPointsOnTwoLines(out Vector3 closestPointLine, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
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
                Debug.Log("ERROR Closest point fail");
                closestPointLine = Vector3.zero;
                return false;
            }
        }
    }
}
