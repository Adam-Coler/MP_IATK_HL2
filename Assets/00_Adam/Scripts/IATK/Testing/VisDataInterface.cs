using UnityEngine;
using IATK;
using System.Collections.Generic;

namespace Photon_IATK
{
    [ExecuteInEditMode]
    public class VisDataInterface : MonoBehaviour
    {
        public GameObject vis;
        public VisWrapperClass visWrapperClass;
        private CSVDataSource csv;

        public Axis xAxis;
        public Axis yAxis;
        public Axis zAxis;

        private Vector3[] csvItems;

        private GameObject obj;

        public float eps = .01f;

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

        //private void OnDrawGizmos()
        //{
        //    float radiusSmall = .015f;
        //    float radiusMed = .025f;
        //    float radiusBig = .035f;

        //    foreach (Vector3 point in islastmesh)
        //    {
        //        //var point = csvItems[5];

        //        Gizmos.color = Color.cyan;
        //        Gizmos.DrawWireSphere(point, radiusSmall);

        //    }


        //    foreach (Vector3 point in getListOfWorldLocationPoints())
        //    {
        //        if (IsInsideMesh(point))
        //        {
        //            Gizmos.color = Color.blue;
        //            Gizmos.DrawWireSphere(point, radiusSmall);
        //        } else
        //        {
        //            Gizmos.color = Color.green;
        //            Gizmos.DrawWireSphere(point, radiusSmall);
        //        }
        //    }

        //}

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

        public Vector3 closest1 = Vector3.zero;
        public Vector3 closest2 = Vector3.zero;
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

                closest1 = closestPointLine1;
                closest2 = closestPointLine2;
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
